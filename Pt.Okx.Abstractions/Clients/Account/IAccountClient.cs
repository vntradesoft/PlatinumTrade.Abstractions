using Pt.Okx.Shared.Clients.Account.Enums;
using Pt.Okx.Shared.Clients.Account.Model;
using Pt.Okx.Shared.Common;

namespace Pt.Okx.Abstractions.Clients.Account
{
    /// <summary>
    /// Provides access to account information, balances, trading configuration,
    /// and account performance metrics.
    /// </summary>
    /// <remarks>
    /// Currently supports OKX USDT-margined perpetual swap accounts only.
    /// Spot, Futures, Options, and other instrument types are not supported.
    /// </remarks>
    public interface IAccountClient
    {
        #region Balances & Assets

        /// <summary>
        /// Retrieves the latest USDT account balance information from the exchange.
        /// </summary>
        /// <param name="ct">A cancellation token.</param>
        /// <returns>The latest USDT account balance wrapped in an ApiResult.</returns>
        Task<ApiResult<AccountBalance>> GetBalanceUsdtAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets the total wallet balance, excluding unrealized profit and loss.
        /// </summary>
        decimal WalletBalance { get; }

        /// <summary>
        /// Gets the available balance for opening new positions.
        /// </summary>
        decimal AvailableBalance { get; }

        /// <summary>
        /// Gets the current account equity, including unrealized profit and loss.
        /// </summary>
        decimal Equity { get; }

        /// <summary>
        /// Gets the total unrealized profit and loss across all open positions.
        /// </summary>
        decimal UnrealizedPnL { get; }

        /// <summary>
        /// Gets the total initial margin currently in use.
        /// </summary>
        decimal InitialMargin { get; }

        /// <summary>
        /// Gets the current margin ratio of the account.
        /// <para>
        /// Positions may be liquidated when this value reaches the exchange liquidation threshold.
        /// </para>
        /// </summary>
        decimal MarginRatio { get; }

        /// <summary>
        /// Gets a value indicating whether the account is currently operating in Hedge Mode.
        /// </summary>
        bool IsHedgeMode { get; }

        #endregion

        #region Trading Configuration

        /// <summary>
        /// Sets the leverage for the specified trading symbol.
        /// </summary>
        /// <param name="symbol">The trading symbol.</param>
        /// <param name="leverage">The leverage value to set.</param>
        /// <param name="ct">A cancellation token.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> SetInitialLeverageAsync(string symbol, int leverage, CancellationToken ct = default);

        /// <summary>
        /// Gets the configured leverage for the specified trading symbol.
        /// </summary>
        /// <param name="symbol">The trading symbol.</param>
        /// <returns>The leverage value.</returns>
        decimal GetLeverage(string symbol);

        /// <summary>
        /// Sets the account position mode.
        /// </summary>
        /// <remarks>
        /// Hedge Mode allows independent long and short positions.
        /// Netting Mode combines positions into a single net position.
        /// </remarks>
        /// <param name="hedge">True for Hedge Mode; false for Netting Mode.</param>
        /// <param name="ct">A cancellation token.</param>
        /// <returns>A tuple indicating success and an optional error message.</returns>
        Task<(bool Success, string? Error)> SetHedgeModeAsync(bool hedge, CancellationToken ct = default);

        /// <summary>
        /// Retrieves the account's current fee VIP level.
        /// </summary>
        /// <returns> The current fee VIP level wrapped in an ApiResult. </returns>
        Task<ApiResult<FeeVipLevel>> GetFeeLevelAsync();

        #endregion
    }


}
