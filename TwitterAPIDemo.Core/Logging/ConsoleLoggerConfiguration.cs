using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace TwitterAPIDemo.Core.Logging
{
    public class ConsoleLoggerConfiguration
    {
        public int EventId { get; set; }

        public Dictionary<LogLevel, ConsoleColor> LogLevels { get; set; } = new()
        {
            [LogLevel.Information] = ConsoleColor.Green,
            [LogLevel.Warning] = ConsoleColor.DarkMagenta,
            [LogLevel.Error] = ConsoleColor.Red
        };
    }
}
