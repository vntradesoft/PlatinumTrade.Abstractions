using Pt.Okx.Shared.Enums;

namespace Pt.Okx.Abstractions.Strategy
{
    /// <summary>
    /// Interface for logging strategy events, state changes, signals, and notifications in trading systems.
    /// </summary>
    public interface IStrategyLogger
    {
        /// <summary>Logs configuration key-value pairs with a title.</summary>
        /// <param name="title">The configuration section title.</param>
        /// <param name="data">The key-value pairs to log.</param>
        void LogConfig(string title, params (string key, string value)[] data);

        /// <summary>Logs an entry order event.</summary>
        /// <param name="symbol">The trading symbol.</param>
        /// <param name="side">The order side (buy/sell).</param>
        /// <param name="quantity">The order quantity.</param>
        /// <param name="price">The entry price.</param>
        /// <param name="sl">The stop-loss price.</param>
        /// <param name="tp">The take-profit price.</param>
        void LogEntry(string symbol, OrderSide side, decimal quantity, decimal price, decimal sl, decimal tp);

        /// <summary>Logs an exit order event.</summary>
        /// <param name="symbol">The trading symbol.</param>
        /// <param name="reason">The reason for exit.</param>
        /// <param name="side">The order side (buy/sell).</param>
        /// <param name="qtyFill">The filled quantity.</param>
        /// <param name="entryPrice">The entry price.</param>
        /// <param name="exitPrice">The exit price.</param>
        /// <param name="pnl">The profit or loss.</param>
        void LogExit(string symbol, string reason, OrderSide side, decimal qtyFill, decimal entryPrice, decimal exitPrice, decimal pnl);

        /// <summary>Logs a set of key-value pairs with a title and message.</summary>
        /// <param name="title">The log section title.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="data">The key-value pairs to log.</param>
        void LogKeyValues(string title, string message, params (string key, string value)[] data);

        /// <summary>Logs a recovery event with a message and log level.</summary>
        /// <param name="message">The recovery message.</param>
        /// <param name="level">The log level (default: Warning).</param>
        void LogRecovery(string message, PtLogLevel level = PtLogLevel.Warning);

        /// <summary>Logs a trading signal event.</summary>
        /// <param name="signalType">The type of signal.</param>
        /// <param name="details">Additional details about the signal.</param>
        void LogSignal(string signalType, string details = "");

        /// <summary>
        /// Logs a state change event with automatic emoji based on the state keyword.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <strong>GREEN emoji (🟢) - Active/Running states:</strong>
        /// <list type="bullet">
        /// <item><term>"active"</term><description>Bot is active</description></item>
        /// <item><term>"idle"</term><description>Bot is idle and ready</description></item>
        /// <item><term>"open"</term><description>Position is open</description></item>
        /// <item><term>"started"</term><description>Bot/process started</description></item>
        /// <item><term>"running"</term><description>Bot is running</description></item>
        /// <item><term>"position_open"</term><description>Position opened</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>RED emoji (🔴) - Inactive/Error states:</strong>
        /// <list type="bullet">
        /// <item><term>"closed"</term><description>Position/session closed</description></item>
        /// <item><term>"stopped"</term><description>Bot stopped</description></item>
        /// <item><term>"ended"</term><description>Process ended</description></item>
        /// <item><term>"error"</term><description>Error occurred</description></item>
        /// <item><term>"failed"</term><description>Action failed</description></item>
        /// <item><term>"blocked"</term><description>Bot blocked/paused</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>YELLOW/SPINNER emoji (🔄) - Pending/Transitional states:</strong>
        /// <list type="bullet">
        /// <item><term>"pending"</term><description>State pending</description></item>
        /// <item><term>"waiting"</term><description>Waiting for something</description></item>
        /// <item><term>"init"</term><description>Initialization</description></item>
        /// <item><term>"recovery"</term><description>Recovery process</description></item>
        /// <item><term>"entry_pending"</term><description>Waiting for entry order fill</description></item>
        /// <item><term>"reversing_pending"</term><description>Reversing position pending</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>DEFAULT emoji (➡️)</strong> - Unknown/other states (not in list above)
        /// </para>
        /// <para>
        /// <strong>Note:</strong> Use lowercase state names for consistent emoji mapping.
        /// </para>
        /// </remarks>
        /// <param name="fromState">The previous state (e.g., "idle", "position_open").</param>
        /// <param name="toState">The new state (use keywords from remarks above for emoji).</param>
        /// <param name="reason">The reason for the state change (optional, max ~100 chars recommended).</param>
        void LogStateChange(string fromState, string toState, string reason = "");

        /// <summary>Sends a notification with a document attachment.</summary>
        /// <param name="title">The notification title.</param>
        /// <param name="filePath">The path to the document file.</param>
        void NotifyDocument(string title, string filePath);

        /// <summary>Sends a notification for an error event.</summary>
        /// <param name="title">The notification title.</param>
        /// <param name="ex">The exception to notify about.</param>
        void NotifyError(string title, Exception ex);

        /// <summary>Sends a notification with key-value pairs.</summary>
        /// <param name="title">The notification title.</param>
        /// <param name="data">The key-value pairs to notify.</param>
        void NotifyKeyValue(string title, params (string key, string value)[] data);

        /// <summary>Sends a trace notification with a message and log level.</summary>
        /// <param name="title">The notification title.</param>
        /// <param name="message">The trace message.</param>
        /// <param name="level">The log level (default: Information).</param>
        void NotifyTrace(string title, string message, PtLogLevel level = PtLogLevel.Information);

        /// <summary>Logs a debug-level message.</summary>
        /// <param name="title">The log entry title.</param>
        /// <param name="detailsTemplate">The message template (optional).</param>
        /// <param name="detailsArgs">Arguments for the message template.</param>
        void LogDebug(string title, string? detailsTemplate = null, params object?[] detailsArgs);

        /// <summary>Logs an informational message.</summary>
        /// <param name="title">The log entry title.</param>
        /// <param name="detailsTemplate">The message template (optional).</param>
        /// <param name="detailsArgs">Arguments for the message template.</param>
        void LogInformation(string title, string? detailsTemplate = null, params object?[] detailsArgs);

        /// <summary>Logs a success message.</summary>
        /// <param name="title">The log entry title.</param>
        /// <param name="detailsTemplate">The message template (optional).</param>
        /// <param name="detailsArgs">Arguments for the message template.</param>
        void LogSuccess(string title, string? detailsTemplate = null, params object?[] detailsArgs);

        /// <summary>Logs a warning message.</summary>
        /// <param name="title">The log entry title.</param>
        /// <param name="detailsTemplate">The message template (optional).</param>
        /// <param name="detailsArgs">Arguments for the message template.</param>
        void LogWarning(string title, string? detailsTemplate = null, params object?[] detailsArgs);

        /// <summary>Logs a critical error message.</summary>
        /// <param name="title">The log entry title.</param>
        /// <param name="detailsTemplate">The message template (optional).</param>
        /// <param name="detailsArgs">Arguments for the message template.</param>
        void LogCritical(string title, string? detailsTemplate = null, params object?[] detailsArgs);

        /// <summary>Logs an error with an exception.</summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="title">The log entry title.</param>
        /// <param name="detailsTemplate">The message template (optional).</param>
        /// <param name="detailsArgs">Arguments for the message template.</param>
        void LogError(Exception? ex, string title, string? detailsTemplate = null, params object?[] detailsArgs);

        /// <summary>Logs a critical error with an exception.</summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="title">The log entry title.</param>
        /// <param name="detailsTemplate">The message template (optional).</param>
        /// <param name="detailsArgs">Arguments for the message template.</param>
        void LogCritical(Exception? ex, string title, string? detailsTemplate = null, params object?[] detailsArgs);

        /// <summary>Logs a message to the console at a specific log level.</summary>
        /// <param name="level">The log level.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="detailsArgs">Arguments for the message template.</param>
        void LogConsole(PtLogLevel level, string message, params object?[] detailsArgs);

        /// <summary>Logs a smart message at a specific log level.</summary>
        /// <param name="level">The log level.</param>
        /// <param name="title">The log entry title.</param>
        /// <param name="detailsTemplate">The message template (optional).</param>
        /// <param name="detailsArgs">Arguments for the message template.</param>
        void LogSmart(PtLogLevel level, string title, string? detailsTemplate = null, params object?[] detailsArgs);

        /// <summary>Logs a smart message with an exception at a specific log level.</summary>
        /// <param name="level">The log level.</param>
        /// <param name="ex">The exception to log.</param>
        /// <param name="title">The log entry title.</param>
        /// <param name="detailsTemplate">The message template (optional).</param>
        /// <param name="detailsArgs">Arguments for the message template.</param>
        void LogSmart(PtLogLevel level, Exception ex, string title, string? detailsTemplate = null, params object?[] detailsArgs);

        /// <summary>Logs a message to the console at a specific log level, with an optional exception.</summary>
        /// <param name="level">The log level.</param>
        /// <param name="exception">The exception to log (optional).</param>
        /// <param name="message">The message to log.</param>
        /// <param name="detailsArgs">Arguments for the message template.</param>
        void LogConsole(PtLogLevel level, Exception? exception, string message, params object?[] detailsArgs);
    }
}
