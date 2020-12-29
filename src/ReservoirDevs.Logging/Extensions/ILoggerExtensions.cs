using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace ReservoirDevs.Logging.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static class ILoggerExtensions
    {
        public static IDisposable CreateScope<T>(this ILogger<T> logger, string methodName)
        {
            var dictionary = new Dictionary<string, string> {{"type", typeof(T).FullName}, {"method", methodName}};
            return CreateScope(logger, dictionary);
        }

        public static IDisposable CreateScope<T>(this ILogger<T> logger, string methodName, params KeyValuePair<string, string>[] state)
        {
            var dictionary = new Dictionary<string, string> { { "type", typeof(T).FullName }, { "method", methodName } };

            foreach (var (key, value) in state)
            {
                dictionary.Add(key, value);
            }

            return CreateScope(logger, dictionary);
        }

        public static IDisposable CreateScope<T>(this ILogger<T> logger, string methodName, IDictionary<string, string> state)
        {
            var dictionary = new Dictionary<string, string> { { "type", typeof(T).FullName }, { "method", methodName } };

            foreach (var (key, value) in state)
            {
                dictionary.Add(key, value);
            }
            
            return CreateScope(logger, dictionary);
        }

        private static IDisposable CreateScope(this ILogger logger, IDictionary<string, string> state)
        {
            return logger.BeginScope(state);
        }
    }
}