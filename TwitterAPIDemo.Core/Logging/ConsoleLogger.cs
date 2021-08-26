using Microsoft.Extensions.Logging;
using System;

namespace TwitterAPIDemo.Core.Logging
{
    public class ConsoleLogger : ILogger
    {
        private readonly string _name;
        private readonly Func<ConsoleLoggerConfiguration> _getCurrentConfig;

        public ConsoleLogger(string name, Func<ConsoleLoggerConfiguration> getCurrentConfig) =>
            (_name, _getCurrentConfig) = (name, getCurrentConfig);

        public IDisposable BeginScope<TState>(TState state) => default;

        public bool IsEnabled(LogLevel logLevel) =>
            _getCurrentConfig().LogLevels.ContainsKey(logLevel);

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var config = _getCurrentConfig();
            //if (config.EventId == 0 || config.EventId == eventId.Id)
            if (config.EventId == 0 && config.EventId == eventId.Id)
            {
                var originalColor = Console.ForegroundColor;

                Console.ForegroundColor = config.LogLevels[logLevel];
                Console.WriteLine($"[{eventId.Id,2}: {logLevel,-12}]");

                Console.ForegroundColor = originalColor;
                Console.WriteLine($"     {_name} - {formatter(state, exception)}");
            }
        }
    }
}
