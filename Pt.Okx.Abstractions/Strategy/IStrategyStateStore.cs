using Pt.Okx.Shared.Clients.Account.Model;
using Pt.Okx.Shared.Clients.Market.Models;
using Pt.Okx.Shared.Clients.Trading.Models;
using Pt.Okx.Shared.Notifier.Models;
using Pt.Okx.Shared.Strategy.Events;

namespace Pt.Okx.Abstractions.Strategy
{
    /// <summary>
    /// Represents a state container for a trading strategy,
    /// providing access to orders, positions, balances, and market data,
    /// as well as methods to apply updates from events.
    /// </summary>
    /// <remarks>
    /// This interface acts as the central runtime state owned by the host engine, allowing it to:
    /// <list type="bullet">
    /// <item>Track current trading state (orders, positions, balances).</item>
    /// <item>Apply incoming events via <see cref="Apply"/>.</item>
    /// <item>Rebuild from external snapshots (e.g., exchange sync).</item>
    /// </list>
    /// <para>
    /// Strategies should treat this store as internal host state and avoid depending on it from tick callbacks.
    /// </para>
    /// </remarks>
    public interface IStrategyStateStore
    {
        /// <summary>
        /// Gets the list of active orders.
        /// </summary>
        IReadOnlyList<Order> Orders { get; }

        /// <summary>
        /// Gets the list of active algorithmic orders (e.g., stop-loss, take-profit).
        /// </summary>
        IReadOnlyList<AlgoOrder> AlgoOrders { get; }

        /// <summary>
        /// Gets the current open positions.
        /// </summary>
        IReadOnlyList<Position> Positions { get; }

        /// <summary>
        /// Gets the account balances for asset USDT.
        /// </summary>
        IReadOnlyList<AccountBalance> Balances { get; }

        /// <summary>
        /// Gets the list of executed transactions.
        /// </summary>
        IReadOnlyList<Transaction> Transactions { get; }

        /// <summary>
        /// Gets the trading command interface used to place or modify orders.
        /// </summary>
        TradeCommand TradeCommand { get; }

        /// <summary>
        /// Gets the most recent candle (kline) data.
        /// </summary>
        CandleData? LastKline { get; }

        /// <summary>
        /// Gets a value indicating whether there is any open position.
        /// </summary>
        bool HasOpenPosition { get; }

        /// <summary>
        /// Gets a value indicating whether there are open orders.
        /// </summary>
        bool HasOpenOrders { get; }

        /// <summary>
        /// Gets a value indicating whether protective algorithmic orders
        /// (e.g., stop-loss or take-profit) exist.
        /// </summary>
        bool HasProtectiveAlgoOrders { get; }

        /// <summary>
        /// Gets the number of currently open orders.
        /// </summary>
        int OpenOrderCount { get; }

        /// <summary>
        /// Gets the number of active algorithmic orders.
        /// </summary>
        int AlgoOrderCount { get; }

        /// <summary>
        /// Applies a strategy event to update the internal state.
        /// </summary>
        /// <param name="evt">The incoming strategy event.</param>
        void Apply(StrategyEvent evt);

        /// <summary>
        /// Updates the latest candle data.
        /// </summary>
        /// <param name="data">The new candle (kline) data.</param>
        void ApplyKline(CandleData data);

        /// <summary>
        /// Rebuilds the entire state from external data sources.
        /// Typically used for synchronization with the exchange or recovery scenarios.
        /// </summary>
        /// <param name="balances">The account balances.</param>
        /// <param name="position">The current positions.</param>
        /// <param name="openOrders">The open orders.</param>
        /// <param name="algoOrders">The algorithmic orders.</param>
        void Rebuild(AccountBalance[] balances, Position[] position, Order[] openOrders, AlgoOrder[] algoOrders);
    }
}
