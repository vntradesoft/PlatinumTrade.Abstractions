using Pt.Okx.Abstractions.Strategy;
using Pt.Okx.Shared.Strategy.Parameters;
using System.Reflection;

namespace Pt.Okx.Abstractions.Strategy.Parameters
{
    /// <summary>
    /// Provides helper methods for logging typed input schema objects.
    /// </summary>
    public static class InputParamLoggingExtensions
    {
        /// <summary>
        /// Logs public readable properties from a schema object as config key-value pairs.
        /// Properties marked with <see cref="InputParamIgnoreAttribute"/> are skipped.
        /// </summary>
        public static void LogInputParams<TSchema>(this IStrategyLogger logger, TSchema input, string title = "Input Parameters")
            where TSchema : class
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(input);

            var properties = typeof(TSchema)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
                .Where(p => p.GetCustomAttribute<InputParamIgnoreAttribute>() is null)
                .ToList();

            var entries = new (string key, string value)[properties.Count];
            for (var i = 0; i < properties.Count; i++)
            {
                var property = properties[i];
                var attr = property.GetCustomAttribute<InputParamAttribute>();
                var key = string.IsNullOrWhiteSpace(attr?.Key)
                    ? ToCamelCase(property.Name)
                    : attr!.Key!;

                var value = property.GetValue(input);
                entries[i] = (key, value?.ToString() ?? string.Empty);
            }

            logger.LogConfig(title, entries);
        }

        private static string ToCamelCase(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return char.ToLowerInvariant(value[0]) + value[1..];
        }
    }
}
