using Pt.Okx.Shared.Enums;

namespace Pt.Okx.Abstractions.Strategy.Plugin
{
    /// <summary>
    /// Provides metadata information for a strategy plugin.
    /// </summary>
    /// <remarks>
    /// This interface defines basic identification information,
    /// as well as optional logging behavior preferences used by the host system.
    /// SDK compatibility is resolved by the host from the referenced Pt.Okx.Shared assembly version.
    /// </remarks>
    public interface IStrategyPluginMetadata
    {
        /// <summary>
        /// Gets the display name of the plugin.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the version of the plugin. Return <c>null</c> to let host infer display
        /// version from assembly informational/version attributes.
        /// </summary>
        string? PluginVersion { get; }

        /// <summary>
        /// Gets the author of the plugin. Return <c>null</c> if not specified.
        /// </summary>
        string Author { get; }

        /// <summary>
        /// Gets the description of the plugin. Return <c>null</c> if not specified.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the log levels that should be displayed in the host UI.
        /// Return <c>null</c> to use the host default configuration.
        /// </summary>
        /// <remarks>
        /// Default levels typically include Information, Warning, Error, Critical, and Success.
        /// </remarks>
        IReadOnlyList<PtLogLevel>? PluginDisplayLogLevels => null;

        /// <summary>
        /// Gets the log levels that should trigger external notifications
        /// (e.g., Telegram or other alerting systems).
        /// Return <c>null</c> to use the host default configuration.
        /// </summary>
        /// <remarks>
        /// Default levels typically include Warning, Error, Critical, and Success.
        /// Debug and Trace levels are always excluded from notifications.
        /// </remarks>
        IReadOnlyList<PtLogLevel>? PluginNotifyLevels => null;
    }
}
