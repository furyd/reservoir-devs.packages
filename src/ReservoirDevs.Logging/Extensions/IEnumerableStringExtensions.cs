using System.Collections.Generic;

namespace ReservoirDevs.Logging.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static class IEnumerableStringExtensions
    {
        public static string ToString(this IEnumerable<string> values, string separator) => string.Join(separator, values);
    }
}