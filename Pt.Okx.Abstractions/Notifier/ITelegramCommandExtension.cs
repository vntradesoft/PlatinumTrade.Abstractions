using Pt.Okx.Shared.Notifier.Models;

namespace Pt.Okx.Abstractions.Notifier
{
    /// <summary>
    /// Interface for Telegram command extension plugins.
    /// Implement this interface to register custom Telegram commands.
    /// TelegramCommandHandler will automatically discover implementations via dependency injection (IEnumerable&lt;T&gt;).
    /// </summary>
    public interface ITelegramCommandExtension
    {
        /// <summary>
        /// Attempts to parse a Telegram command for this extension.
        /// Returns null if the command does not belong to this extension.
        /// </summary>
        /// <param name="action">The command action (without the leading '/').</param>
        /// <param name="args">The command arguments (parts[1..]).</param>
        /// <returns>A <see cref="TradeCommand"/> if the command is recognized; otherwise, null.</returns>
        TradeCommand? TryParse(string action, string[] args);

        /// <summary>
        /// The help text block to display under /help. Null means this extension does not provide help text.
        /// </summary>
        string? HelpText { get; }
    }
}
