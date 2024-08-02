
namespace Globe.Shared.Constants
{
    public static class AppLoggingConfigurationKeys
    {
        public const string Enabled = "AppLogging:Enabled";
        public static class ToFile
        {
            public const string Path = "AppLogging:ToFile:Path";
            public const string FileSizeLimitBytes = "AppLogging:ToFile:FileSizeLimitBytes";
            public const string RetainedFileCountLimit = "AppLogging:ToFile:RetainedFileCountLimit";
            public const string OutputTextTemplate = "AppLogging:ToFile:OutputTextTemplate";
            public const string Format = "AppLogging:ToFile:Format";
            public const string Enabled = "AppLogging:ToFile:Enabled";
            public const string MinimumLevelSerilog = "AppLogging:ToFile:MinimumLevel";
            public const string ExpressionTemplate = "AppLogging:ToFile:ExpressionTemplate";
            public const string UseExpressions = "AppLogging:ToFile:UseExpressions";
        }

        public static class ToDebug
        {
            public const string Enabled = "AppLogging:ToDebug:Enabled";
            public const string OutputTextTemplate = "AppLogging:ToDebug:OutputTextTemplate";
            public const string MinimumLevelSerilog = "AppLogging:ToDebug:MinimumLevel";
        }

        public static class ToConsole
        {
            public const string Enabled = "AppLogging:ToConsole:Enabled";
            public const string OutputTextTemplate = "AppLogging:ToConsole:OutputTextTemplate";
            public const string MinimumLevelSerilog = "AppLogging:ToConsole:MinimumLevel";
        }

        public static class ToSeq
        {
            public const string SeqUrl = "AppLogging:ToSeq:SeqUrl";
            public const string Enabled = "AppLogging:ToSeq:Enabled";
            public const string OutputTextTemplate = "AppLogging:ToSeq:OutputTextTemplate";
            public const string MinimumLevelSerilog = "AppLogging:ToSeq:MinimumLevel";
            public const string EventBodyLimitBytes = "AppLogging:ToSeq:EventBodyLimitBytes";
        }
    }
}
