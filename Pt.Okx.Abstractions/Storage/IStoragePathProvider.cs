using Pt.Okx.Shared.Storage.Enums;

namespace Pt.Okx.Abstractions.Storage
{
    /// <summary>
    /// Provides runtime storage directories for history, logs, state and cache.
    /// End-users can implement this interface and register it in DI to customize storage layout.
    /// </summary>
    public interface IStoragePathProvider
    {
        /// <summary>
        /// Gets a storage path by logical scope.
        /// </summary>
        /// <param name="scope">
        /// Target scope. If omitted, returns the highest runtime data root.
        /// </param>
        string GetPath(StoragePathScope scope = StoragePathScope.RuntimeDataRoot)
            => scope switch
            {
                StoragePathScope.RuntimeDataRoot => GetRuntimeDataRoot(),
                StoragePathScope.Histories => GetHistoryRoot(),
                StoragePathScope.Logs => GetLogsRoot(),
                StoragePathScope.Backtest => GetBacktestRoot(),
                StoragePathScope.Live => GetLiveRoot(),
                StoragePathScope.BacktestLogs => GetBacktestLogsRoot(),
                StoragePathScope.LiveLogs => GetLiveLogsRoot(),
                StoragePathScope.State => GetStateRoot(),
                StoragePathScope.Cache => GetCacheRoot(),
                StoragePathScope.Exports => GetExportsRoot(),
                StoragePathScope.IndicatorPlugins => GetIndicatorPluginsRoot(),
                StoragePathScope.StrategyPlugins => GetStrategyPluginsRoot(),
                _ => GetRuntimeDataRoot(),
            };

        /// <summary>Gets the root directory used for runtime data.</summary>
        string GetRuntimeDataRoot();

        /// <summary>Gets the history storage root.</summary>
        string GetHistoryRoot();

        /// <summary>Gets the logs storage root.</summary>
        /// <remarks>
        /// This path is for runtime application logs (for example Serilog all-/error- rolling files).
        /// It is not intended for mode-specific trade/export logs.
        /// </remarks>
        string GetLogsRoot();

        /// <summary>Gets the backtest storage root.</summary>
        string GetBacktestRoot();

        /// <summary>Gets the live storage root.</summary>
        string GetLiveRoot();

        /// <summary>Gets the backtest logs storage root.</summary>
        /// <remarks>
        /// Intended for backtest-specific logs/artifacts (for example exported trade logs),
        /// separate from runtime Serilog files in <see cref="GetLogsRoot"/>.
        /// </remarks>
        string GetBacktestLogsRoot();

        /// <summary>Gets the live logs storage root.</summary>
        /// <remarks>
        /// Intended for live-mode logs/artifacts, separate from runtime Serilog files
        /// in <see cref="GetLogsRoot"/>.
        /// </remarks>
        string GetLiveLogsRoot();

        /// <summary>Gets the state storage root.</summary>
        string GetStateRoot();

        /// <summary>Gets the cache storage root.</summary>
        string GetCacheRoot();

        /// <summary>Gets the export artifacts storage root.</summary>
        string GetExportsRoot();

        /// <summary>Gets the folder containing custom technical indicators plugins.</summary>
        string GetIndicatorPluginsRoot();

        /// <summary>Gets the folder containing custom trading strategies plugins.</summary>
        string GetStrategyPluginsRoot();
    }
}
