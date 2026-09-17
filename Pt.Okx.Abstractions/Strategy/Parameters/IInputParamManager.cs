using Pt.Okx.Shared.Strategy.Parameters;
namespace Pt.Okx.Abstractions.Strategy.Parameters
{
    /// <summary>
    /// Manages input parameters for a strategy or component,
    /// including loading, validation, and change notification.
    /// </summary>
    /// <remarks>
    /// This interface provides a centralized way to access and update
    /// runtime-configurable parameters, supporting persistence and validation.
    /// </remarks>
    public interface IInputParamManager
    {
        /// <summary>
        /// Occurs when a parameter value has been changed.
        /// </summary>
        event EventHandler<ParameterChangedEventArgs>? ParameterChanged;

        /// <summary>
        /// Occurs when parameters have been loaded from an external source.
        /// </summary>
        event EventHandler? ParametersLoaded;

        /// <summary>
        /// Gets the collection of all registered parameters.
        /// </summary>
        IReadOnlyDictionary<string, InputParameter> Parameters { get; }

        /// <summary>
        /// Gets the collection of parameter keys.
        /// </summary>
        IEnumerable<string> Keys { get; }

        /// <summary>
        /// Loads parameters from a file.
        /// </summary>
        /// <param name="filePath">The path to the parameter file.</param>
        void LoadFromFile(string filePath);

        /// <summary>Saves parameters to a file.</summary>
        /// <param name="filePath">The path to the output file.</param>
        void SaveToFile(string filePath);

        /// <summary>
        /// Serializes all current parameters (schema + values) to a compact JSON string.
        /// Use with <see cref="ImportFromJson"/> to transfer state to a child process.
        /// </summary>
        string ExportToJson();

        /// <summary>
        /// Restores parameters from a JSON string previously produced by <see cref="ExportToJson"/>.
        /// Replaces current parameters entirely.
        /// </summary>
        void ImportFromJson(string json);

        /// <summary>
        /// Gets the value of a parameter with the specified key.
        /// </summary>
        /// <typeparam name="TValue">The expected value type.</typeparam>
        /// <param name="key">The parameter key.</param>
        /// <param name="defaultValue">
        /// The default value to return if the parameter does not exist or cannot be converted.
        /// </param>
        /// <returns>The parameter value or the specified default value.</returns>
        TValue GetValue<TValue>(string key, TValue defaultValue = default!);

        /// <summary>
        /// Sets the value of a parameter.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="value">The value to set.</param>
        void SetValue(string key, object value);

        /// <summary>
        /// Determines whether a parameter with the specified key exists.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <returns>
        /// <c>true</c> if the parameter exists; otherwise <c>false</c>.
        /// </returns>
        bool HasParameter(string key);

        /// <summary>
        /// Gets the full parameter definition for the specified key.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <returns>
        /// The parameter definition if found; otherwise <c>null</c>.
        /// </returns>
        InputParameter? GetParameter(string key);

        /// <summary>
        /// Gets the value range constraint for the specified parameter.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <returns>
        /// The associated <see cref="ValueRange"/> if defined; otherwise <c>null</c>.
        /// </returns>
        ValueRange? GetValueRange(string key);

        /// <summary>
        /// Validates all parameters and returns validation errors.
        /// </summary>
        /// <returns>
        /// A dictionary of parameter keys and corresponding error messages.
        /// Empty if all parameters are valid.
        /// </returns>
        Dictionary<string, string> ValidateAll();

        /// <summary>
        /// Validates a single parameter value.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="value">The value to validate.</param>
        /// <param name="errorMessage">The validation error message if invalid.</param>
        /// <returns>
        /// <c>true</c> if the value is valid; otherwise <c>false</c>.
        /// </returns>
        bool ValidateParameter(string key, object value, out string errorMessage);

        /// <summary>
        /// Loads parameters from code-defined defaults, merging with an existing JSON file if present.
        /// If the JSON file exists, user-modified values are preserved for matching keys.
        /// If the JSON file does not exist, or the schema has changed (keys differ), 
        /// a new JSON file is written with merged data.
        /// </summary>
        /// <param name="defaults">The default parameters declared in plugin code.</param>
        /// <param name="filePath">The path to the JSON parameter file for persistence.</param>
        void LoadFromDefaults(IReadOnlyDictionary<string, InputParameter> defaults, string filePath);
    }
}
