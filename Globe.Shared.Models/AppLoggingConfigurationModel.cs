using Serilog.Events;

namespace Globe.Shared.Models
{
    public class AppLoggingConfigurationModel
    {
        /// <summary>
        /// Enables or disables the logging middleware
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// MinimumLevel from json file
        /// </summary>
        public string GlobalMinimumLevel { get; set; }
        /// <summary>
        /// MinimumLevel destined for Serilog
        /// </summary>
        public LogEventLevel GlobalMinimumLevelSerilog { get; set; }


        /// <summary>
        /// if true then the middleware UseLoggerMiddleware is activated.
        /// </summary>
        public bool AddRequestsLogging { get; set; }

        /// <summary>
        /// If AddRequestsLogging == true then in case of Exception and POST call, the RequestBody is logged.
        /// </summary>
        public bool OnErrorLogRequestBody { get; set; }
        /// <summary>
        /// Log to file configuration
        /// </summary>
        public ToFileSettings ToFile { get; set; }
        /// <summary>
        /// Log to debug configuration
        /// </summary>
        public ToDebugSettings ToDebug { get; set; }

        /// <summary>
        /// Log to console configuration
        /// </summary>
        public ToConsoleSettings ToConsole { get; set; }
        /// <summary>
        /// Log to seq configuration
        /// </summary>
        public ToSeqSettings ToSeq { get; set; }

        /// <summary>
        /// The maximum size of the request or response body that can be logged (in bytes).
        /// If the body exceeds this size, it will be truncated to this length.
        /// </summary>
        public int MaximumLogPropertySize { get; set; }

        /// <summary>
        /// Enable or disable Serilog's self logging. Messages are outputted to debug console.
        /// </summary>
        public bool EnableSerilogSelfLogging { get; set; }

        public AppLoggingConfigurationModel()
        {
            Enabled = false;
            GlobalMinimumLevel = "Debug";
            GlobalMinimumLevelSerilog = LogEventLevel.Debug;
            AddRequestsLogging = true;
            OnErrorLogRequestBody = true;
            ToFile = new ToFileSettings();
            ToDebug = new ToDebugSettings();
            ToConsole = new ToConsoleSettings();
            ToSeq = new ToSeqSettings();
            MaximumLogPropertySize = 100000;
            EnableSerilogSelfLogging = false;
        }
    }

    public class ToFileSettings
    {
        public bool Enabled { get; set; }
        public string Format { get; set; }
        public string Path { get; set; }
        public int FileSizeLimitBytes { get; set; }
        public int RetainedFileCountLimit { get; set; }
        public string OutputTextTemplate { get; set; }
        /// <summary>
        /// MinimumLevel from json file
        /// </summary>
        public string MinimumLevel { get; set; }
        /// <summary>
        /// MinimumLevel destined for Serilog
        /// </summary>
        public LogEventLevel MinimumLevelSerilog { get; set; }
        /// <summary>
        /// Used for Serilog expressions. Cannot be used alongside an output template.
        /// Requires the "Serilog.Expressions" package to be installed.
        /// </summary>
        public string ExpressionTemplate { get; set; }
        /// <summary>
        /// If true, then the expression template will be used
        /// instead of the OutputTextTemplate.
        /// </summary>
        public bool UseExpressions { get; set; }
    }

    public class ToDebugSettings
    {
        public bool Enabled { get; set; }
        public string OutputTextTemplate { get; set; }
        /// <summary>
        /// MinimumLevel from json file
        /// </summary>
        public string MinimumLevel { get; set; }
        /// <summary>
        /// MinimumLevel destined for Serilog
        /// </summary>
        public LogEventLevel MinimumLevelSerilog { get; set; }
    }

    public class ToConsoleSettings
    {
        public bool Enabled { get; set; }
        public string OutputTextTemplate { get; set; }
        /// <summary>
        /// MinimumLevel from json file
        /// </summary>
        public string MinimumLevel { get; set; }
        /// <summary>
        /// MinimumLevel destined for Serilog
        /// </summary>
        public LogEventLevel MinimumLevelSerilog { get; set; }
    }

    public class ToSeqSettings
    {
        public bool Enabled { get; set; }
        public string SeqUrl { get; set; }
        public string ApiKey { get; set; }
        /// <summary>
        /// MinimumLevel from json file
        /// </summary>
        public string MinimumLevel { get; set; }
        /// <summary>
        /// MinimumLevel destined for Serilog
        /// </summary>
        public LogEventLevel MinimumLevelSerilog { get; set; }

        /// <summary>
        /// The maximum event body size (in bytes).
        /// If the event body is larger, an error is shown in Seq.
        /// </summary>
        public int EventBodyLimitBytes { get; set; }
    }

}
