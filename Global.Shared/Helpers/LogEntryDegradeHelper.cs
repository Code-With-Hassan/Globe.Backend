using Globe.Shared.Models;
using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace Globe.Shared.Helpers
{
    public class LogEntryDegradeHelper
    {
        private readonly ILogger<LogEntryDegradeHelper> _logger;
        private readonly LoggingSettingsModel _loggingSettings;

        public LogEntryDegradeHelper(ILogger<LogEntryDegradeHelper> logger, LoggingSettingsModel loggingSettings)
        {
            _logger = logger;
            _loggingSettings = loggingSettings;
        }

        public LogEventLevel DegradeLogEntry(string requestPath, LogEventLevel logLevel)
        {
            try
            {
                
                if (_loggingSettings.DebugDegrade.Any(x => requestPath.ToLower().StartsWith(x.ToLower())) && logLevel == LogEventLevel.Information)
                {
                    logLevel = LogEventLevel.Debug;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Exception caught while attempting to degrade log entry for request {requestPath}. Will not degrade. Exception: {ex}", requestPath, ex);
            }
            return logLevel;
        }
    }
}
