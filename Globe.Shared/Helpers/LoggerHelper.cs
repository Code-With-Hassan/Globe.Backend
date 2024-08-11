using Globe.Shared.Constants;
using Globe.Shared.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System.Diagnostics;
using System.Reflection;

namespace Globe.Shared.Helpers
{
    /// <summary>
    /// Read Logger Configuration from SystemSettings.json file and Set Logger.
    /// </summary>
    public class LoggerHelper
    {
        private readonly string defaultPath = "./logs/";
        private readonly int defaultFileSizeLimitBytes = 2000000;
        private readonly int defaultRetainedFileCountLimit = 100;
        private readonly string defaultFormat = "text";
        private readonly string defaultOutputTemplate = "{Timestamp:o} [{Level:u3}] {CorrelationId} ({MachineName}/{Application}/{Username}/{ThreadId}) {Message}{NewLine}{Exception}";
        private readonly string defaultExpressionTemplate = "{@t:o} [{@l:u3}] {@m}\n{@x}";
        private readonly string defaultTableName = "Logger";
        private readonly int defaultEventBodyLimitBytes = 262144;
        private AppLoggingConfigurationModel AppLoggingConfiguration;
        private string ServiceName;

        public LoggerHelper()
        {
            AppLoggingConfiguration = new AppLoggingConfigurationModel();
            ServiceName = string.Empty;
        }

        public void Configure(LoggerConfiguration loggerConfiguration, IConfiguration configuration)
        {
            ServiceName = configuration.GetSection("ServiceInformation").GetValue<string>("ServiceName");
            ReadLoggingConfig(configuration);

            InitLogger(loggerConfiguration);

            SetGlobalMinimumLevel(loggerConfiguration, configuration);

            ToFile(loggerConfiguration);
            ToDebug(loggerConfiguration);
            ToConsole(loggerConfiguration);
            ToSeq(loggerConfiguration);
            SetSerilogSelfLogging(configuration, "AppLogging:EnableSerilogSelfLogging");
        }

        private void ReadLoggingConfig(IConfiguration configuration)
        {
            //We do not use it here; we read from the configuration again when we need it
            AppLoggingConfiguration.Enabled = SetEnabled(configuration, AppLoggingConfigurationKeys.Enabled);
            AppLoggingConfiguration.ToFile.Path = SetPathToFile(configuration);
            AppLoggingConfiguration.ToFile.FileSizeLimitBytes = SetFileSizeLimitBytesToFile(configuration);
            AppLoggingConfiguration.ToFile.RetainedFileCountLimit = SetRetainedFileCountLimitToFile(configuration);
            AppLoggingConfiguration.ToFile.OutputTextTemplate = SetOutputTemplate(configuration, AppLoggingConfigurationKeys.ToFile.OutputTextTemplate);
            AppLoggingConfiguration.ToFile.Format = SetFormatToFile(configuration);
            AppLoggingConfiguration.ToFile.Enabled = SetEnabled(configuration, AppLoggingConfigurationKeys.ToFile.Enabled);
            AppLoggingConfiguration.ToFile.MinimumLevelSerilog = SetMinimumLevel(configuration, AppLoggingConfigurationKeys.ToFile.MinimumLevelSerilog);
            AppLoggingConfiguration.ToFile.ExpressionTemplate = SetExpressionTemplate(configuration, AppLoggingConfigurationKeys.ToFile.ExpressionTemplate);
            AppLoggingConfiguration.ToFile.UseExpressions = SetUseExpressions(configuration, AppLoggingConfigurationKeys.ToFile.UseExpressions);

            AppLoggingConfiguration.ToDebug.OutputTextTemplate = SetOutputTemplate(configuration, AppLoggingConfigurationKeys.ToDebug.OutputTextTemplate);
            AppLoggingConfiguration.ToDebug.Enabled = SetEnabled(configuration, AppLoggingConfigurationKeys.ToDebug.Enabled);
            AppLoggingConfiguration.ToDebug.MinimumLevelSerilog = SetMinimumLevel(configuration, AppLoggingConfigurationKeys.ToDebug.MinimumLevelSerilog);

            AppLoggingConfiguration.ToConsole.OutputTextTemplate = SetOutputTemplate(configuration, AppLoggingConfigurationKeys.ToConsole.OutputTextTemplate);
            AppLoggingConfiguration.ToConsole.Enabled = SetEnabled(configuration, AppLoggingConfigurationKeys.ToConsole.Enabled);
            AppLoggingConfiguration.ToConsole.MinimumLevelSerilog = SetMinimumLevel(configuration, AppLoggingConfigurationKeys.ToConsole.MinimumLevelSerilog);

            AppLoggingConfiguration.ToSeq.SeqUrl = SetToSeqUrl(configuration);
            AppLoggingConfiguration.ToSeq.Enabled = SetEnabled(configuration, AppLoggingConfigurationKeys.ToSeq.Enabled);
            AppLoggingConfiguration.ToSeq.ApiKey = SetToSeqApiKey(configuration);
            AppLoggingConfiguration.ToSeq.MinimumLevelSerilog = SetMinimumLevel(configuration, AppLoggingConfigurationKeys.ToSeq.MinimumLevelSerilog);
            AppLoggingConfiguration.ToSeq.EventBodyLimitBytes = SetEventBodyLimitBytesToSeq(configuration, AppLoggingConfigurationKeys.ToSeq.EventBodyLimitBytes);
        }

        private void InitLogger(LoggerConfiguration loggerConfiguration)
        {
            loggerConfiguration
                         //.ReadFrom.Services(services)
                         // .Enrich.WithCorrelationId()   //DO NOT use this, we add CorrelationId manually, see Startup class
                         .Enrich.FromLogContext()
                         .Enrich.WithMachineName()
                         .Enrich.WithThreadId()
                         .Enrich.WithMethod()            // <-- custom enricher --<<<
                         .Enrich.WithProperty("Application", ServiceName)
                         .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                         .MinimumLevel.Override("System", LogEventLevel.Information);
        }
        private void ToFile(LoggerConfiguration loggerConfiguration)
        {
            if (AppLoggingConfiguration.ToFile.Enabled)
            {
                if (AppLoggingConfiguration.ToFile.Format == "text")
                {
                    if (!AppLoggingConfiguration.ToFile.UseExpressions)
                    {
                        loggerConfiguration.WriteTo.File(
                            path: AppLoggingConfiguration.ToFile.Path,
                            rollingInterval: RollingInterval.Day,
                            rollOnFileSizeLimit: true,
                            fileSizeLimitBytes: AppLoggingConfiguration.ToFile.FileSizeLimitBytes,
                            retainedFileCountLimit: AppLoggingConfiguration.ToFile.RetainedFileCountLimit,
                            outputTemplate: AppLoggingConfiguration.ToFile.OutputTextTemplate,
                            restrictedToMinimumLevel: AppLoggingConfiguration.ToFile.MinimumLevelSerilog
                    );
                    }
                    else
                    {
                        loggerConfiguration.WriteTo.File(
                            path: AppLoggingConfiguration.ToFile.Path,
                            rollingInterval: RollingInterval.Day,
                            rollOnFileSizeLimit: true,
                            fileSizeLimitBytes: AppLoggingConfiguration.ToFile.FileSizeLimitBytes,
                            retainedFileCountLimit: AppLoggingConfiguration.ToFile.RetainedFileCountLimit,
                            formatter: new Serilog.Templates.ExpressionTemplate(AppLoggingConfiguration.ToFile.ExpressionTemplate),
                            restrictedToMinimumLevel: AppLoggingConfiguration.ToFile.MinimumLevelSerilog
                    );
                    }
                }
                else if (AppLoggingConfiguration.ToFile.Format == "json")
                {
                    loggerConfiguration.WriteTo.File(
                            formatter: new CompactJsonFormatter(),
                            path: AppLoggingConfiguration.ToFile.Path,
                            rollingInterval: RollingInterval.Day,
                            rollOnFileSizeLimit: true,
                            fileSizeLimitBytes: AppLoggingConfiguration.ToFile.FileSizeLimitBytes,
                            retainedFileCountLimit: AppLoggingConfiguration.ToFile.RetainedFileCountLimit,
                            restrictedToMinimumLevel: AppLoggingConfiguration.ToFile.MinimumLevelSerilog
                    );
                }
                else
                {
                    throw new Exception("Invalide toText logger file formar");
                }
            }
        }


        private void ToDebug(LoggerConfiguration loggerConfiguration)
        {
            if (AppLoggingConfiguration.ToDebug.Enabled)
            {
                loggerConfiguration.WriteTo.Debug(
                            outputTemplate: AppLoggingConfiguration.ToDebug.OutputTextTemplate,
                            restrictedToMinimumLevel: AppLoggingConfiguration.ToDebug.MinimumLevelSerilog
                            );
            }
        }

        private void ToConsole(LoggerConfiguration loggerConfiguration)
        {
            if (AppLoggingConfiguration.ToConsole.Enabled)
            {
                loggerConfiguration.WriteTo.Console(
                            outputTemplate: AppLoggingConfiguration.ToDebug.OutputTextTemplate,
                            restrictedToMinimumLevel: AppLoggingConfiguration.ToDebug.MinimumLevelSerilog
                            );
            }
        }

        private void ToSeq(LoggerConfiguration loggerConfiguration)
        {
            if (AppLoggingConfiguration.ToSeq.Enabled)
            {
                if (AppLoggingConfiguration.ToSeq.SeqUrl == null) throw new Exception("Invalid Seq Url");
                loggerConfiguration.WriteTo.Seq(
                            AppLoggingConfiguration.ToSeq.SeqUrl,
                            apiKey: AppLoggingConfiguration.ToSeq.ApiKey,
                            restrictedToMinimumLevel: AppLoggingConfiguration.ToSeq.MinimumLevelSerilog,
                            eventBodyLimitBytes: AppLoggingConfiguration.ToSeq.EventBodyLimitBytes
                            );
            }
        }

        private string SetServiceName(IConfiguration configuration)
        {
            string serviceName = configuration["System:ServiceName"];
            if (string.IsNullOrWhiteSpace(serviceName)) return Assembly.GetEntryAssembly().GetName().Name;
            return serviceName.Trim();
        }

        private string SetPathToFile(IConfiguration configuration)
        {
            string path = configuration[AppLoggingConfigurationKeys.ToFile.Path];
            if (string.IsNullOrWhiteSpace(path)) path = defaultPath;
            path = path.Trim();

            return path;
        }

        private int SetFileSizeLimitBytesToFile(IConfiguration configuration)
        {
            string conf = configuration["AppLogging:ToFile:FileSizeLimitBytes"];
            if (string.IsNullOrWhiteSpace(conf)) return defaultFileSizeLimitBytes;
            return Convert.ToInt32(conf);
        }

        private int SetRetainedFileCountLimitToFile(IConfiguration configuration)
        {
            string conf = configuration["AppLogging:ToFile:RetainedFileCountLimit"];
            if (string.IsNullOrWhiteSpace(conf)) return defaultRetainedFileCountLimit;
            return Convert.ToInt32(conf);
        }

        private string SetOutputTemplate(IConfiguration configuration, string key)
        {
            string outputTemplate = configuration[key];
            if (string.IsNullOrWhiteSpace(outputTemplate)) return defaultOutputTemplate;
            return outputTemplate.Trim();
        }

        private string SetFormatToFile(IConfiguration configuration)
        {
            string format = configuration["AppLogging:ToFile:Format"];
            if (string.IsNullOrWhiteSpace(format)) return defaultFormat;
            return format.Trim().ToLower();
        }

        private bool SetEnabled(IConfiguration configuration, string key)
        {
            string enabled = configuration[key];
            if (string.IsNullOrWhiteSpace(enabled)) return false;
            return Convert.ToBoolean(enabled.Trim().ToLower());
        }

        private bool SetRequestsLogging(IConfiguration configuration)
        {
            string request = configuration["AppLogging:AddRequestsLogging"];
            if (string.IsNullOrWhiteSpace(request)) return false;
            return Convert.ToBoolean(request.Trim().ToLower());
        }

        private void SetGlobalMinimumLevel(LoggerConfiguration loggerConfiguration, IConfiguration configuration)
        {
            string conf = configuration["AppLogging:GlobalMinimumLevel"];

            switch (conf)
            {
                case "Debug":
                    loggerConfiguration.MinimumLevel.Debug();
                    break;

                case "Error":
                    loggerConfiguration.MinimumLevel.Error();
                    break;

                case "Fatal":
                    loggerConfiguration.MinimumLevel.Fatal();
                    break;

                case "Information":
                    loggerConfiguration.MinimumLevel.Information();
                    break;

                case "Verbose":
                    loggerConfiguration.MinimumLevel.Verbose();
                    break;

                case "Warning":
                    loggerConfiguration.MinimumLevel.Warning();
                    break;

                default:
                    throw new Exception("Wrong Logging Global Minimum Level");//loggerConfiguration.MinimumLevel.Information();
                                                                              // break;
            }

        }
        private string SetToSeqApiKey(IConfiguration configuration)
        {
            string format = configuration["AppLogging:ToSeq:ApiKey"];
            if (string.IsNullOrWhiteSpace(format)) return null;
            return format.Trim();
        }

        private string SetToSeqUrl(IConfiguration configuration)
        {
            string url = configuration["AppLogging:ToSeq:SeqUrl"];
            if (string.IsNullOrWhiteSpace(url)) return null;
            return url.Trim();
        }


        private LogEventLevel SetMinimumLevel(IConfiguration configuration, string key)
        {

            switch (configuration[key])
            {
                case "Debug":
                    return LogEventLevel.Debug;
                // break;

                case "Error":
                    return LogEventLevel.Error;
                // break;

                case "Fatal":
                    return LogEventLevel.Fatal;
                // break;

                case "Information":
                    return LogEventLevel.Information;
                // break;

                case "Verbose":
                    return LogEventLevel.Verbose;
                //  break;

                case "Warning":
                    return LogEventLevel.Warning;
                // break;

                default:
                    throw new Exception("Wrong Logging Minimum Level");//loggerConfiguration.MinimumLevel.Information();
                                                                       // break;
            }
        }

        private string SetExpressionTemplate(IConfiguration configuration, string key)
        {
            string expressionTemplate = configuration[key];
            if (string.IsNullOrWhiteSpace(expressionTemplate)) return defaultExpressionTemplate;
            return expressionTemplate.Trim();
        }

        private bool SetUseExpressions(IConfiguration configuration, string key)
        {
            string useExpressions = configuration[key];
            if (string.IsNullOrWhiteSpace(useExpressions)) return false;
            return Convert.ToBoolean(useExpressions.Trim().ToLower());
        }

        private string SetLoggingDatabaseConfigurationStringValue(IConfiguration configuration, string key)
        {
            string configValue = configuration[key];
            if (string.IsNullOrWhiteSpace(configValue)) return string.Empty;
            return configValue.Trim();
        }

        private bool SetLoggingDatabaseIntegratedSecurity(IConfiguration configuration, string key)
        {
            string integratedSecurity = configuration[key];
            if (string.IsNullOrWhiteSpace(integratedSecurity)) return false;
            return Convert.ToBoolean(integratedSecurity.Trim().ToLower());
        }

        private string SetTableName(IConfiguration configuration, string key)
        {
            string configValue = configuration[key];
            if (string.IsNullOrWhiteSpace(configValue)) return defaultTableName;
            return configValue.Trim();
        }

        private int SetEventBodyLimitBytesToSeq(IConfiguration configuration, string key)
        {
            string conf = configuration[key];
            if (string.IsNullOrWhiteSpace(conf)) return defaultEventBodyLimitBytes;
            return Convert.ToInt32(conf);
        }

        private void SetSerilogSelfLogging(IConfiguration configuration, string key)
        {
            if (Convert.ToBoolean(configuration[key]))
                Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine($"{DateTime.Now}: Serilog self log message: {msg}"));
        }
    }
}
