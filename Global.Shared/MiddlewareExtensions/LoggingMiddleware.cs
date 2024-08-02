using Globe.Shared.Helpers;
using Globe.Shared.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Serilog.Events;
using System.Diagnostics;
using System.Text;
using Globe.Shared.Exceptions;
using System.Data.SqlClient;
using System.Text.Json;

namespace Globe.Shared.MiddlewareExtensions
{
    /// <summary>
    /// Log every Rest call. 
    /// Important Note: for invalide model states the ValidateModelStateAttribute is mandatory
    /// </summary>
    public class LoggingMiddleware
    {
        readonly RequestDelegate _next;

        readonly ILogger<LoggingMiddleware> _logger;
        private readonly IConfiguration _configuration;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger, IConfiguration configuration)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger;
            _configuration = configuration;
        }


        public async Task Invoke(HttpContext httpContext, LogMaskingHelper logMaskingHelper, LogEntryDegradeHelper logEntryDegradeHelper)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));
            string requestBody = null;
            long start = Stopwatch.GetTimestamp();
            ErrorResponseModel errorResponse = null;
            string exAsString = null;
            dynamic exception = null;
            //used for error responses from underlying microservices where there is no exception thrown
            bool isErrorResponseWithoutException = false;

            var buffer = new MemoryStream();
            var stream = httpContext.Response.Body;
            httpContext.Response.Body = buffer;
            try
            {
                // Below code is needed to set UserName, ClientIP and CorrelationId for Logging (Serilog). 
                // Gets user Name from user Identity. 
                var userName = httpContext.User.Identity.IsAuthenticated ? httpContext.User.Identity.Name : "Guest";
                // Push user in LogContext; 
                LogContext.PushProperty("Username", userName);
                // Push client IP
                LogContext.PushProperty("IPAddress", httpContext.Connection.RemoteIpAddress.ToString());
                #region Read request body
                httpContext.Request.EnableBuffering();
                var requestBodyReader = new StreamReader(httpContext.Request.Body);
                var requestBodyText = await requestBodyReader.ReadToEndAsync();
                string maskedRequestBody = string.IsNullOrWhiteSpace(requestBodyText) ? null : logMaskingHelper.MaskLog(httpContext.Request.Path.ToString(), requestBodyText);
                string requestBodyTextToBeLogged = string.Empty;
                if (!string.IsNullOrWhiteSpace(maskedRequestBody))
                {
                    if (maskedRequestBody.Length > _configuration.GetSection("AppLogging").GetValue<int>("MaximumLogPropertySize"))
                        requestBodyTextToBeLogged = maskedRequestBody.Substring(0, _configuration.GetSection("AppLogging").GetValue<int>("MaximumLogPropertySize")) + "<Log Truncated>";
                    else
                        requestBodyTextToBeLogged = maskedRequestBody;
                }
                LogContext.PushProperty("RequestBody", requestBodyTextToBeLogged);
                httpContext.Request.Body.Position = 0;
                #endregion                

                //Push custom CorrelationId in LogContext.
                //bodyReader.Dispose(); this or using statement throw a System.ObjectDisposedException later! Possible memory leak???? 
                string guid = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 6);
                LogContext.PushProperty("CorrelationId", guid);

                if (_configuration.GetSection("AppLogging").GetValue<bool>("OnErrorLogRequestBody") && httpContext.Request.Method == "POST")
                    requestBody = await GetRequestBodyAsync(httpContext);

                await _next(httpContext);

                if (httpContext.Response.StatusCode == 404)
                {
                    throw new BusinessException("The file or page that the client is requesting wasn't found by the server (404)");
                }
                if (httpContext.Response.StatusCode == 405)
                {
                    throw new BusinessException("Method Not Allowed (405). The server knows the request method, but the target resource doesn't support this method");
                }
            }
            catch (SqlException sqlex)
            {
                //for SqlException, log as error the full exception and return to client 'Database Error'
                string mes = null;
                if (sqlex.Message.StartsWith("Cannot insert duplicate key row"))
                    mes = "Id or Unique Value already exists to DataBase";
                else if (sqlex.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                    mes = "Unable to delete this entity because it is linked to other entities. " +
                        "Please remove the linked entities and retry.";
                else
                    mes = "Database Error";
                errorResponse = new ErrorResponseModel() { ErrorDescription = mes, ErrorType = sqlex.GetType().Name };
                exAsString = sqlex.ToString();
                exception = sqlex;
            }
            catch (BusinessException bex)
            {
                //for BussinessException, log as warning only the exception's Message or SecondaryMessage (without stack trace) and return to client the exception's Message
                exAsString = string.IsNullOrWhiteSpace(bex.SecondaryMessage) ? bex.Message : bex.SecondaryMessage;
                errorResponse = new ErrorResponseModel() { ErrorDescription = bex.Message, ErrorType = bex.GetType().Name };
                exception = bex;
            }
            catch (UnauthorisedException uex)
            {
                //for BussinessException, log as warning only the exception's message (without stack trace) and return to client the exception's message
                exAsString = uex.Message;
                errorResponse = new ErrorResponseModel() { ErrorDescription = uex.Message, ErrorType = uex.GetType().Name };
                exception = uex;
            }
            catch (Exception ex)
            {
                //for Exception, log as error the full sqlex and return to client the exception's message
                exAsString = ex.ToString();
                errorResponse = new ErrorResponseModel() { ErrorDescription = ex.Message, ErrorType = ex.GetType().Name };
                exception = ex;
            }
            finally
            {
                #region Read response body
                //reset the buffer and read out the contents
                buffer.Seek(0, SeekOrigin.Begin);
                //using (var bufferReader = new StreamReader(buffer))
                //{
                var bufferReader = new StreamReader(buffer);
                string responseBodyText = await bufferReader.ReadToEndAsync();

                //reset to start of stream
                buffer.Seek(0, SeekOrigin.Begin);

                //copy our content to the original stream and put it back
                await buffer.CopyToAsync(stream);
                httpContext.Response.Body = stream;

                //check if the services have returned an error response
                if (httpContext.Response.StatusCode > 299)
                {
                    errorResponse = new ErrorResponseModel
                    {
                        ErrorDescription = $"Error {httpContext.Response.StatusCode}",
                        ErrorType = $"Service Response Error",
                        ModelState = new ModelStateDictionary()
                    };
                    isErrorResponseWithoutException = true;
                }

                string maskedResponseBody = string.IsNullOrWhiteSpace(responseBodyText) ? null : logMaskingHelper.MaskLog(httpContext.Request.Path.ToString(), responseBodyText);
                string responseBodyTextToBeLogged = string.Empty;
                if (!string.IsNullOrWhiteSpace(maskedResponseBody))
                {
                    if (maskedResponseBody.Length > _configuration.GetSection("AppLogging").GetValue<int>("MaximumLogPropertySize"))
                        responseBodyTextToBeLogged = maskedResponseBody.Substring(0, _configuration.GetSection("AppLogging").GetValue<int>("MaximumLogPropertySize")) + "<Log Truncated>";
                    else
                        responseBodyTextToBeLogged = maskedResponseBody;
                }
                LogContext.PushProperty("ResponseBody", responseBodyTextToBeLogged);
                //}
                #endregion

                FreeResources(buffer, stream, bufferReader);

                double elapsedMs = GetElapsedMilliseconds(start, Stopwatch.GetTimestamp());
                if (errorResponse == null && (httpContext.Response.StatusCode == 200 || httpContext.Response.StatusCode == 204))
                {
                    RestLogger(httpContext, elapsedMs, LogEventLevel.Information, requestBody, logEntryDegradeHelper);
                }
                else if (errorResponse == null && httpContext.Response.Headers["MODELSTATE"] == "UNVALID")
                {
                    RestLogger(httpContext, elapsedMs, LogEventLevel.Warning, requestBody, logEntryDegradeHelper, "Invalid Model State");
                }
                else if (errorResponse == null && httpContext.Response.Headers.ContainsKey("Sec-WebSocket-Accept"))
                {
                    RestLogger(httpContext, elapsedMs, LogEventLevel.Information, requestBody, logEntryDegradeHelper, "SignalR Connection Closed");
                }
                else if (errorResponse != null &&
                        (errorResponse.ErrorType == "BussinessException" ||
                        errorResponse.ErrorType == "UnAuthorisedException" ||
                        errorResponse.ErrorType == "ModelStateError"))
                {
                    ExceptionHandler(httpContext, errorResponse, elapsedMs, LogEventLevel.Warning, exception, requestBody, logEntryDegradeHelper);
                }
                else if (errorResponse != null && errorResponse.ErrorType.EndsWith("Exception"))
                {
                    ExceptionHandler(httpContext, errorResponse, elapsedMs, LogEventLevel.Error, exception, requestBody, logEntryDegradeHelper);
                }
                else if (errorResponse != null && isErrorResponseWithoutException)
                {
                    RestLogger(httpContext, elapsedMs, LogEventLevel.Warning, requestBody, logEntryDegradeHelper);
                }
            }
        }



        /// <summary>
        /// log exception and return the proper message to client with response code 400
        /// </summary>
        /// <param name="httpContext">HttpContext</param>
        /// <param name="errorRespose">what client gets</param>
        /// <param name="elapsedMs">rest call's elapsed time in ms</param>
        /// <param name="logLevel">log Level</param>
        /// <param name="ex">exception as string</param>
        /// <param name="addInfo">additional info for logging in case of BussinessException</param>
        private void ExceptionHandler(HttpContext httpContext, ErrorResponseModel errorResponse, double elapsedMs, LogEventLevel logLevel, Exception ex, string requestBody, LogEntryDegradeHelper logEntryDegradeHelper)
        {
            httpContext.Response.StatusCode = 400;
            RestLogger(httpContext, elapsedMs, logLevel, requestBody, logEntryDegradeHelper, null, ex);

            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(errorResponse));
            httpContext.Response.ContentType = "application/json";
            httpContext.Request.EnableBuffering();
            _ = httpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
            //This is an alternative to getting bytes and writing on the stream
            //httpContext.Response.WriteAsJsonAsync(errorResponse);
        }

        /// <summary>
        /// Log the Rest Call
        /// </summary>
        /// <param name="httpContext">HttpContext</param>
        /// <param name="elapsedMs">elapsed time in ms</param>
        /// <param name="logLevel">log Level</param>
        /// <param name="error">exception  as string or exception's message</param>
        /// <param name="addInfo">additional info in case of BussinessException</param>
        private void RestLogger(HttpContext httpContext, double elapsedMs, LogEventLevel logLevel, string requestBody,
            LogEntryDegradeHelper logEntryDegradeHelper, string error = null, Exception fullException = null)
        {
            string requestMethod = httpContext.Request.Method;
            string requestPath = httpContext.Request.Path.ToString();
            int status = httpContext.Response.StatusCode;

            string nl = logLevel == LogEventLevel.Error ? "\r\n" : "";

            string descr = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {ElapsedMs} ms ";

            //We only have an error message which is passed from the calling function, without an exception
            if (!string.IsNullOrWhiteSpace(error) && fullException == null)
                descr += $"{nl} {{ErrorMessage}}";

            //degrade log entries from hangfire's dashboard!!!
            //now we can degrade any log entry that is info and defined in the corresponding section of the SystemSettings.json file
            logLevel = logEntryDegradeHelper.DegradeLogEntry(requestPath, logLevel);

            switch (logLevel)
            {
                case LogEventLevel.Information:
                    if (_configuration.GetSection("AppLogging").GetValue<bool>("AddRequestsLogging"))
                        if (fullException == null)
                        {
                            if (string.IsNullOrWhiteSpace(error))
                                _logger.LogInformation(descr, requestMethod, requestPath, status, elapsedMs);
                            else
                                _logger.LogInformation(descr, requestMethod, requestPath, status, elapsedMs, error);
                        }
                        else
                            _logger.LogWarning(fullException, descr, requestMethod, requestPath, status, elapsedMs);
                    break;

                case LogEventLevel.Error:
                    _logger.LogError(fullException, descr, requestMethod, requestPath, status, elapsedMs);
                    break;

                case LogEventLevel.Warning:
                    if (fullException == null)
                    {
                        if (string.IsNullOrWhiteSpace(error))
                            _logger.LogWarning(descr, requestMethod, requestPath, status, elapsedMs);
                        else
                            _logger.LogWarning(descr, requestMethod, requestPath, status, elapsedMs, error);
                    }
                    else
                        _logger.LogWarning(fullException, descr, requestMethod, requestPath, status, elapsedMs);
                    break;

                case LogEventLevel.Fatal:
                    _logger.LogCritical(fullException, descr, requestMethod, requestPath, status, elapsedMs, error);
                    break;

                case LogEventLevel.Verbose:
                    if (_configuration.GetSection("AppLogging").GetValue<bool>("AddRequestsLogging"))
                        if (string.IsNullOrWhiteSpace(error))
                            _logger.LogTrace(descr, requestMethod, requestPath, status, elapsedMs);
                        else
                            _logger.LogWarning(descr, requestMethod, requestPath, status, elapsedMs, error);
                    break;

                case LogEventLevel.Debug:
                    if (_configuration.GetSection("AppLogging").GetValue<bool>("AddRequestsLogging"))
                        if (string.IsNullOrWhiteSpace(error))
                            _logger.LogDebug(descr, requestMethod, requestPath, status, elapsedMs);
                        else
                            _logger.LogWarning(descr, requestMethod, requestPath, status, elapsedMs, error);
                    break;

            }
        }

        private double GetElapsedMilliseconds(long start, long stop)
        {
            return (stop - start) * 1000 / (double)Stopwatch.Frequency;
        }


        private async Task<string> GetRequestBodyAsync(HttpContext httpContext)
        {
            string body = null;

            httpContext.Request.EnableBuffering();

            // Leave the body open so the next middleware can read it.
            using (var requestReader = new StreamReader(
                httpContext.Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 2024,
                leaveOpen: true))
            {
                body = await requestReader.ReadToEndAsync();
                httpContext.Request.Body.Position = 0;
            }
            return body;
        }

        private void FreeResources(MemoryStream buffer, Stream stream, StreamReader bufferReader)
        {
            buffer.Close();
            buffer.Dispose();
            buffer = null;

            stream.Close();
            stream.Dispose();
            stream = null;

            bufferReader.Close();
            bufferReader.Dispose();
            bufferReader = null;
        }
    }




    /// <summary>
    /// Extension method used to add the middleware to the HTTP request pipeline.
    /// </summary>
    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }
}
