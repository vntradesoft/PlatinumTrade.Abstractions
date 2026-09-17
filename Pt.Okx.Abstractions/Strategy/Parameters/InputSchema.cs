using Pt.Okx.Shared.Strategy.Parameters;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace Pt.Okx.Abstractions.Strategy.Parameters
{
    /// <summary>
    /// Specifies a specialized data type for parameters when property type alone is insufficient.
    /// </summary>
    public enum InputParamDataType
    {
        /// <summary>Infer from property type.</summary>
        Auto = 0,
        /// <summary>Treat a string property as a file path.</summary>
        FilePath = 1,
    }

    /// <summary>
    /// Declares metadata for a property that should be exposed as an input parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class InputParamAttribute : Attribute
    {
        /// <summary>Parameter key stored in the JSON payload.</summary>
        public string? Key { get; set; }

        /// <summary>Display description shown in configuration UI.</summary>
        public string? Description { get; set; }

        /// <summary>Minimum numeric bound for the parameter.</summary>
        public double Min { get; set; }

        /// <summary>Maximum numeric bound for the parameter.</summary>
        public double Max { get; set; }

        /// <summary>Section index used to group parameters in UI.</summary>
        public int Section { get; set; }

        /// <summary>Section title used when rendering grouped parameters.</summary>
        public string? SectionTitle { get; set; }

        /// <summary>Order within a section.</summary>
        public int Order { get; set; }

        /// <summary>Specialized data type hint for rendering and validation.</summary>
        public InputParamDataType DataType { get; set; } = InputParamDataType.Auto;
    }

    /// <summary>
    /// Marks properties that should not be treated as persisted input parameters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class InputParamIgnoreAttribute : Attribute
    {
    }

    /// <summary>
    /// Metadata describing a schema property annotated with <see cref="InputParamAttribute"/>.
    /// </summary>
    public sealed class InputParamMetadata
    {
        /// <summary>The reflected property.</summary>
        public required PropertyInfo Property { get; init; }

        /// <summary>The persisted parameter key.</summary>
        public required string Key { get; init; }

        /// <summary>The annotated description.</summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>The normalized value range.</summary>
        public ValueRange Range { get; init; } = new();

        /// <summary>The section index, or 0 when not grouped.</summary>
        public int Section { get; init; }

        /// <summary>The optional section title.</summary>
        public string? SectionTitle { get; init; }

        /// <summary>The display order within the section.</summary>
        public int Order { get; init; }

        /// <summary>The specialized data type hint.</summary>
        public InputParamDataType DataType { get; init; }
    }

    /// <summary>
    /// Provides typed access to schema metadata without requiring string keys.
    /// </summary>
    public static class InputSchemaMetadata
    {
        /// <summary>
        /// Gets metadata for a schema property selected by expression.
        /// </summary>
        public static InputParamMetadata Get<TSchema, TValue>(Expression<Func<TSchema, TValue>> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);

            var property = GetProperty(selector);
            if (property.GetCustomAttribute<InputParamIgnoreAttribute>() is not null)
            {
                throw new InvalidOperationException(
                    $"Property '{property.DeclaringType?.FullName}.{property.Name}' is marked with {nameof(InputParamIgnoreAttribute)} and is not an input parameter.");
            }

            var attribute = property.GetCustomAttribute<InputParamAttribute>();
            var key = string.IsNullOrWhiteSpace(attribute?.Key)
                ? ToCamelCase(property.Name)
                : attribute!.Key!;

            return new InputParamMetadata
            {
                Property = property,
                Key = key,
                Description = attribute?.Description ?? string.Empty,
                Range = BuildRange(attribute),
                Section = attribute?.Section ?? 0,
                SectionTitle = attribute?.SectionTitle,
                Order = attribute?.Order ?? 0,
                DataType = attribute?.DataType ?? InputParamDataType.Auto,
            };
        }

        /// <summary>
        /// Gets the normalized range for a schema property selected by expression.
        /// </summary>
        public static ValueRange GetRange<TSchema, TValue>(Expression<Func<TSchema, TValue>> selector)
            => Get(selector).Range;

        private static PropertyInfo GetProperty<TSchema, TValue>(Expression<Func<TSchema, TValue>> selector)
        {
            Expression body = selector.Body;
            if (body is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
            {
                body = unary.Operand;
            }

            if (body is not MemberExpression member || member.Member is not PropertyInfo property)
            {
                throw new ArgumentException("Selector must target a schema property.", nameof(selector));
            }

            return property;
        }

        private static ValueRange BuildRange(InputParamAttribute? attribute)
        {
            if (attribute is null)
                return new ValueRange();

            if (attribute.Min == 0 && attribute.Max == 0)
                return new ValueRange();

            return new ValueRange
            {
                Min = attribute.Min,
                Max = attribute.Max,
            };
        }

        private static string ToCamelCase(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            // Count consecutive leading uppercase chars
            int upper = 0;
            while (upper < value.Length && char.IsUpper(value[upper])) upper++;

            if (upper == 0) return value;
            if (upper == 1) return char.ToLowerInvariant(value[0]) + value[1..];

            // For acronyms like RRLevels → rrLevels: lowercase all but last uppercase
            // when last uppercase is followed by a lowercase (start of next word).
            // If all chars are uppercase (e.g. URL), lowercase everything.
            int toLower = upper < value.Length ? upper - 1 : upper;
            var chars = value.ToCharArray();
            for (int i = 0; i < toLower; i++) chars[i] = char.ToLowerInvariant(chars[i]);
            return new string(chars);
        }
    }

    /// <summary>
    /// Builds an <see cref="InputParameter"/> dictionary from an annotated schema type.
    /// </summary>
    public static class InputSchemaBuilder
    {
        /// <summary>
        /// Builds defaults from a schema type.
        /// </summary>
        public static IReadOnlyDictionary<string, InputParameter> BuildDefaults<TSchema>() where TSchema : new()
            => BuildDefaults(typeof(TSchema));

        /// <summary>
        /// Builds defaults from a schema type.
        /// </summary>
        public static IReadOnlyDictionary<string, InputParameter> BuildDefaults(Type schemaType)
        {
            ArgumentNullException.ThrowIfNull(schemaType);

            var schema = Activator.CreateInstance(schemaType)
                ?? throw new InvalidOperationException($"Could not instantiate schema type '{schemaType.FullName}'.");

            var properties = schemaType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0)
                .Where(p => p.GetCustomAttribute<InputParamIgnoreAttribute>() is null)
                .Select(p =>
                {
                    var attr = p.GetCustomAttribute<InputParamAttribute>();
                    var key = string.IsNullOrWhiteSpace(attr?.Key)
                        ? ToCamelCase(p.Name)
                        : attr!.Key!;

                    return new SchemaEntry(
                        key,
                        p,
                        attr,
                        p.GetValue(schema));
                })
                .ToList();

            var duplicate = properties
                .GroupBy(x => x.Key, StringComparer.Ordinal)
                .FirstOrDefault(g => g.Count() > 1);

            if (duplicate is not null)
                throw new InvalidOperationException($"Duplicate input key '{duplicate.Key}' in schema '{schemaType.FullName}'.");

            var dictionary = new Dictionary<string, InputParameter>(StringComparer.Ordinal);

            var sections = properties
                .Where(x => x.Attribute is not null && x.Attribute.Section > 0)
                .GroupBy(x => x.Attribute!.Section)
                .OrderBy(g => g.Key)
                .ToList();

            foreach (var section in sections)
            {
                var title = section
                    .Select(x => x.Attribute?.SectionTitle)
                    .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x))
                    ?.Trim();

                if (!string.IsNullOrWhiteSpace(title))
                {
                    var headerText = $"---------- {section.Key}. {title} ----------";
                    dictionary[$"section{section.Key}"] = new HeaderParameter(headerText)
                    {
                        Value = headerText,
                        Description = headerText
                    };
                }

                foreach (var entry in section.OrderBy(x => x.Attribute?.Order ?? int.MaxValue))
                {
                    dictionary[entry.Key] = CreateParameter(entry);
                }
            }

            foreach (var entry in properties.Where(x => x.Attribute is null || x.Attribute.Section <= 0).OrderBy(x => x.Attribute?.Order ?? int.MaxValue))
            {
                if (!dictionary.ContainsKey(entry.Key))
                    dictionary[entry.Key] = CreateParameter(entry);
            }

            return dictionary;
        }

        private static InputParameter CreateParameter(SchemaEntry entry)
        {
            var propType = entry.Property.PropertyType;
            var type = Nullable.GetUnderlyingType(propType) ?? propType;
            var value = entry.DefaultValue;

            InputParameter parameter;

            // Check for List<T> before checking individual types
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var elementType = type.GetGenericArguments()[0];
                parameter = CreateListParameter(elementType, value);
            }
            else if (type == typeof(int))
            {
                parameter = new IntParameter { Value = Convert.ToInt32(value ?? 0, CultureInfo.InvariantCulture) };
            }
            else if (type == typeof(decimal))
            {
                parameter = new DecimalParameter { Value = Convert.ToDecimal(value ?? 0m, CultureInfo.InvariantCulture) };
            }
            else if (type == typeof(double) || type == typeof(float))
            {
                parameter = new DoubleParameter { Value = Convert.ToDouble(value ?? 0d, CultureInfo.InvariantCulture) };
            }
            else if (type == typeof(TimeSpan))
            {
                parameter = new TimeSpanParameter { Value = value as TimeSpan? ?? TimeSpan.Zero };
            }
            else if (type == typeof(bool))
            {
                parameter = new BoolParameter { Value = Convert.ToBoolean(value ?? false, CultureInfo.InvariantCulture) };
            }
            else if (type == typeof(string))
            {
                if (entry.Attribute?.DataType == InputParamDataType.FilePath)
                {
                    parameter = new FilePathParameter { Value = Convert.ToString(value ?? string.Empty, CultureInfo.InvariantCulture) ?? string.Empty };
                }
                else
                {
                    parameter = new StringParameter { Value = Convert.ToString(value ?? string.Empty, CultureInfo.InvariantCulture) ?? string.Empty };
                }
            }
            else if (type == typeof(DateTime))
            {
                parameter = new DateTimeParameter { Value = value as DateTime? ?? DateTime.MinValue };
            }
            else if (type.IsEnum)
            {
                var enumValue = value?.ToString() ?? Enum.GetNames(type).First();
                parameter = new EnumParameter
                {
                    Type = $"enum[{type.Name}]",
                    Value = enumValue,
                };

                foreach (var name in Enum.GetNames(type))
                {
                    parameter.Options.Add(name);
                }
            }
            else
            {
                throw new InvalidOperationException(
                    $"Unsupported input parameter property type '{entry.Property.PropertyType.FullName}' on '{entry.Property.DeclaringType?.FullName}.{entry.Property.Name}'.");
            }

            parameter.Description = entry.Attribute?.Description ?? string.Empty;
            parameter.Range = BuildRange(entry.Attribute);

            return parameter;
        }

        private static InputParameter CreateListParameter(Type elementType, object? defaultValue)
        {
            if (elementType == typeof(string))
            {
                var listParam = new ListParameter<string>();
                if (defaultValue is IEnumerable<string> stringList)
                {
                    listParam.Value = new List<string>(stringList);
                }
                return listParam;
            }

            if (elementType == typeof(int))
            {
                var listParam = new ListParameter<int>();
                if (defaultValue is IEnumerable<int> intList)
                {
                    listParam.Value = new List<int>(intList);
                }
                return listParam;
            }

            if (elementType == typeof(decimal))
            {
                var listParam = new ListParameter<decimal>();
                if (defaultValue is IEnumerable<decimal> decimalList)
                {
                    listParam.Value = new List<decimal>(decimalList);
                }
                return listParam;
            }

            if (elementType == typeof(double))
            {
                var listParam = new ListParameter<double>();
                if (defaultValue is IEnumerable<double> doubleList)
                {
                    listParam.Value = new List<double>(doubleList);
                }
                return listParam;
            }

            if (elementType == typeof(float))
            {
                var listParam = new ListParameter<float>();
                if (defaultValue is IEnumerable<float> floatList)
                {
                    listParam.Value = new List<float>(floatList);
                }
                return listParam;
            }

            if (elementType == typeof(bool))
            {
                var listParam = new ListParameter<bool>();
                if (defaultValue is IEnumerable<bool> boolList)
                {
                    listParam.Value = new List<bool>(boolList);
                }
                return listParam;
            }

            if (elementType == typeof(long))
            {
                var listParam = new ListParameter<long>();
                if (defaultValue is IEnumerable<long> longList)
                {
                    listParam.Value = new List<long>(longList);
                }
                return listParam;
            }

            throw new InvalidOperationException(
                $"Unsupported list element type: {elementType.Name}. Supported types: string, int, decimal, double, float, bool, long.");
        }

        private static ValueRange BuildRange(InputParamAttribute? attribute)
        {
            if (attribute is null)
                return new ValueRange();

            // In attributes, omitted numeric values default to 0.
            // We only treat range as explicitly provided when at least one side is non-zero.
            if (attribute.Min == 0 && attribute.Max == 0)
                return new ValueRange();

            return new ValueRange
            {
                Min = attribute.Min,
                Max = attribute.Max,
            };
        }

        private static string ToCamelCase(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            // Count consecutive leading uppercase chars
            int upper = 0;
            while (upper < value.Length && char.IsUpper(value[upper])) upper++;

            if (upper == 0) return value;
            if (upper == 1) return char.ToLowerInvariant(value[0]) + value[1..];

            // For acronyms like RRLevels → rrLevels: lowercase all but last uppercase
            // when last uppercase is followed by a lowercase (start of next word).
            // If all chars are uppercase (e.g. URL), lowercase everything.
            int toLower = upper < value.Length ? upper - 1 : upper;
            var chars = value.ToCharArray();
            for (int i = 0; i < toLower; i++) chars[i] = char.ToLowerInvariant(chars[i]);
            return new string(chars);
        }

        private sealed record SchemaEntry(string Key, PropertyInfo Property, InputParamAttribute? Attribute, object? DefaultValue);
    }

    /// <summary>
    /// Binds an annotated schema object from values currently stored in <see cref="IInputParamManager"/>.
    /// </summary>
    public static class InputParamBindingExtensions
    {
        /// <summary>
        /// Creates and populates a schema object from manager values.
        /// </summary>
        public static TSchema BindSchema<TSchema>(this IInputParamManager manager)
            where TSchema : new()
        {
            ArgumentNullException.ThrowIfNull(manager);

            var schema = new TSchema();
            var schemaType = typeof(TSchema);
            var properties = schemaType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0)
                .Where(p => p.GetCustomAttribute<InputParamIgnoreAttribute>() is null);

            foreach (var property in properties)
            {
                var attr = property.GetCustomAttribute<InputParamAttribute>();
                var key = string.IsNullOrWhiteSpace(attr?.Key)
                    ? char.ToLowerInvariant(property.Name[0]) + property.Name[1..]
                    : attr!.Key!;

                var currentDefault = property.GetValue(schema);
                var value = GetValue(manager, property.PropertyType, key, currentDefault);
                property.SetValue(schema, value);
            }

            return schema;
        }

        private static object? GetValue(IInputParamManager manager, Type targetType, string key, object? defaultValue)
        {
            var method = typeof(IInputParamManager)
                .GetMethods()
                .First(m => m.Name == nameof(IInputParamManager.GetValue) && m.IsGenericMethod)
                .MakeGenericMethod(targetType);

            return method.Invoke(manager, new[] { key, defaultValue ?? GetDefault(targetType)! });
        }

        private static object? GetDefault(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}
