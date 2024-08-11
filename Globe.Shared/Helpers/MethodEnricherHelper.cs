using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Globe.Shared.Helpers
{
    public class MethodEnricherHelper : ILogEventEnricher
    {/// <summary>
     /// Excluded Assemblies from search
     /// </summary>
        List<string> assemExcl = new List<string>() { "Serilog", "Microsoft.Extensions.Logging" };

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var skip = 3;
            while (true)
            {
                var stack = new StackFrame(skip);
                if (!stack.HasMethod())
                {
                    logEvent.AddPropertyIfAbsent(new LogEventProperty("Method", new ScalarValue("<unknown method>")));
                    return;
                }

                var method = stack.GetMethod();
                var assemFullName = method.DeclaringType?.Assembly.FullName;

                if (assemFullName != null && assemExcl.Count(x => assemFullName.StartsWith(x)) == 0)
                {
                    var caller = $"{method.DeclaringType.FullName}.{method.Name}";
                    logEvent.AddPropertyIfAbsent(new LogEventProperty("Method", new ScalarValue(caller)));
                    return;
                }

                skip++;
            }
        }
    }

    /// <summary>
    /// Enrich with Full MethodName. Fill parameter 'Method'
    /// </summary>
    static class LoggerMathodEnrichmentConfiguration
    {
        /// <summary>
        /// Custom Enricher: Enrich with Full MethodName. Fill parameter 'Method'
        /// </summary>
        public static LoggerConfiguration WithMethod(this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            return enrichmentConfiguration.With<MethodEnricherHelper>();
        }
    }
}