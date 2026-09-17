using Pt.Okx.Shared.Indicators.Plugin;
using Pt.Okx.Shared.Enums;
using Pt.Okx.Abstractions.Indicators.Base;

namespace Pt.Okx.Abstractions.Indicators.Plugin
{
    /// <summary>
    /// Describes a parameter exposed by a custom indicator plugin.
    /// The GUI can use this metadata to generate an input dialog.
    /// </summary>
    /// <param name="Key">The parameter key used in configuration.</param>
    /// <param name="DisplayName">The user-friendly name for the parameter.</param>
    /// <param name="ValueType">The <see cref="Type"/> of the parameter value.</param>
    /// <param name="DefaultValue">The default value of the parameter.</param>
    /// <param name="MinValue">The optional minimum value for numeric parameters.</param>
    /// <param name="MaxValue">The optional maximum value for numeric parameters.</param>
    public record IndicatorParameterInfo(
        string Key,
        string DisplayName,
        Type ValueType,
        object DefaultValue,
        object? MinValue = null,
        object? MaxValue = null);

    /// <summary>
    /// Registration context passed to indicator plugins.
    /// Plugins register indicators through this context instead of calling the factory directly.
    /// </summary>
    public interface IIndicatorRegistrationContext
    {
        /// <summary>
        /// Registers an indicator creator by name.
        /// </summary>
        /// <param name="name">The name of the indicator.</param>
        /// <param name="creator">The <see cref="IndicatorCreator"/> delegate.</param>
        void Register(string name, IndicatorCreator creator);

        /// <summary>
        /// Registers an indicator creator with parameter metadata.
        /// </summary>
        /// <param name="name">The name of the indicator.</param>
        /// <param name="creator">The <see cref="IndicatorCreator"/> delegate.</param>
        /// <param name="parameterDefs">The list of <see cref="IndicatorParameterInfo"/> for this indicator.</param>
        void Register(string name, IndicatorCreator creator,
                      IReadOnlyList<IndicatorParameterInfo> parameterDefs);
    }

    /// <summary>
    /// Metadata exposed by an indicator plugin assembly.
    /// </summary>
    public interface IIndicatorPluginMetadata
    {
        /// <summary>Plugin display name, for example "My Custom Indicators".</summary>
        string Name { get; }

        /// <summary>
        /// Plugin version, for example "1.0.0". Return <c>null</c> to let host infer display
        /// version from assembly informational/version attributes.
        /// </summary>
        string? PluginVersion { get; }

        /// <summary>Short plugin description.</summary>
        string Description { get; }

        /// <summary>The author or publisher of the plugin.</summary>
        string Author { get; }
    }

    /// <summary>
    /// Contract implemented by custom indicator plugin assemblies.
    /// </summary>
    public interface IIndicatorPlugin : IIndicatorPluginMetadata
    {
        /// <summary>
        /// Registers all custom indicators provided by the plugin.
        /// </summary>
        /// <param name="context">The <see cref="IIndicatorRegistrationContext"/> used for registration.</param>
        void RegisterIndicators(IIndicatorRegistrationContext context);
    }

    /// <summary>
    /// Loads custom indicator plugins from external assemblies.
    /// Strategy and GUI components use this interface instead of implementing plugin loading themselves.
    /// </summary>
    public interface IIndicatorPluginLoader
    {
        /// <summary>
        /// Loads a plugin from a specific DLL path, when provided, and scans the default plugins directory.
        /// </summary>
        /// <param name="customDllPath">Optional DLL path. When null, only the default plugin directory is scanned.</param>
        void LoadAll(string? customDllPath = null);

        /// <summary>
        /// Loads one plugin from a DLL path.
        /// </summary>
        /// <param name="dllPath">The path to the plugin DLL.</param>
        /// <returns>A <see cref="IndicatorLoadedPlugin"/> instance if successfully loaded; otherwise, <c>null</c>.</returns>
        IndicatorLoadedPlugin? LoadPlugin(string dllPath);

        /// <summary>
        /// Loads all plugins from a directory.
        /// </summary>
        /// <param name="directory">The directory path to scan for plugin DLLs.</param>
        /// <returns>A list of <see cref="IndicatorLoadedPlugin"/> instances.</returns>
        IReadOnlyList<IndicatorLoadedPlugin> LoadPluginsFromDirectory(string directory);

        /// <summary>
        /// Gets the loaded plugins.
        /// </summary>
        /// <returns>A read-only list of <see cref="IndicatorLoadedPlugin"/> instances.</returns>
        IReadOnlyList<IndicatorLoadedPlugin> LoadedPlugins { get; }

        /// <summary>
        /// Unloads a loaded plugin, unregistering its indicators.
        /// </summary>
        /// <param name="dllPath">The path to the plugin DLL.</param>
        /// <returns><c>true</c> if successfully unloaded; otherwise, <c>false</c>.</returns>
        UnloadPluginResult UnloadPlugin(string dllPath);

        /// <summary>
        /// Verifies whether a DLL is a valid, compatible indicator plugin.
        /// </summary>
        /// <param name="dllPath">The path to the plugin DLL.</param>
        /// <param name="errorMessage">When validation fails, contains the detailed error description.</param>
        /// <returns><c>true</c> if valid and compatible; otherwise, <c>false</c>.</returns>
        bool VerifyIndicatorPlugin(string dllPath, out string? errorMessage);
    }
}
