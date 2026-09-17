using Pt.Okx.Shared.Clients.Trading.Enums;
using Pt.Okx.Shared.Clients.Trading.Models;
using Pt.Okx.Shared.Common;
using Pt.Okx.Shared.Enums;

namespace Pt.Okx.Abstractions.Clients.Trading
{

    /// <summary>
    /// Provides trading operations including order placement,
    /// order management, position management, and trade history retrieval.
    /// </summary>
    /// <remarks>
    /// Currently supports OKX USDT-margined perpetual swap instruments only.
    /// Spot, Futures, Options, and other instrument types are not supported.
    /// </remarks>
    public interface ITradeClient
    {
        /// <summary>
        /// Places a new trading order (Limit/Market, etc.).
        /// </summary>
        /// <param name="symbol">The trading symbol.</param>
        /// <param name="side">The order side (buy/sell).</param>
        /// <param name="type">The order type (limit, market, etc.).</param>
        /// <param name="quantity">The order quantity.</param>
        /// <param name="price">The order price (optional).</param>
        /// <param name="attachedAlgoOrder">Optional attached algo order.</param>
        /// <param name="reduceOnly">Reduce-only flag (optional).</param>
        /// <param name="tag">Order tag (optional).</param>
        /// <param name="ct">Cancellation token (optional).</param>
        /// <returns>The result of the order placement.</returns>
        Task<ApiResult<OrderPlaceResponse>> PlaceOrderAsync(string symbol, OrderSide side, OrderType type, decimal quantity, decimal? price = null, AttachedAlgoOrder? attachedAlgoOrder = null, bool? reduceOnly = null, string? tag = null, CancellationToken ct = default);

        /// <summary>
        /// Places an Algo order (Take Profit/Stop Loss, Trailing Stop, Trigger Order, etc.).
        /// </summary>
        /// <param name="symbol">The trading symbol.</param>
        /// <param name="orderSide">The order side.</param>
        /// <param name="algoOrderType">The algo order type.</param>
        /// <param name="quantity">Order quantity (optional).</param>
        /// <param name="reduceOnly">Reduce-only flag (optional).</param>
        /// <param name="positionSide">Position side (optional).</param>
        /// <param name="tpTriggerPxType">Take profit trigger price type (optional).</param>
        /// <param name="tpTriggerPrice">Take profit trigger price (optional).</param>
        /// <param name="tpOrderPrice">Take profit order price (optional).</param>
        /// <param name="slTriggerPxType">Stop loss trigger price type (optional).</param>
        /// <param name="slTriggerPrice">Stop loss trigger price (optional).</param>
        /// <param name="slOrderPrice">Stop loss order price (optional).</param>
        /// <param name="closeFraction">Close fraction (optional).</param>
        /// <param name="cancelOnClose">Cancel on close flag (optional).</param>
        /// <param name="tag">Order tag (optional).</param>
        /// <param name="ct">Cancellation token (optional).</param>
        /// <returns>The result of the algo order placement.</returns>
        Task<ApiResult<AlgoOrderResponse>> PlaceAlgoOrderAsync(string symbol, OrderSide orderSide, AlgoOrderType algoOrderType, decimal? quantity = null, bool? reduceOnly = null, PositionSide? positionSide = null, AlgoPriceType? tpTriggerPxType = null, decimal? tpTriggerPrice = null, decimal? tpOrderPrice = null, AlgoPriceType? slTriggerPxType = null, decimal? slTriggerPrice = null, decimal? slOrderPrice = null, decimal? closeFraction = null, bool? cancelOnClose = null, string? tag = null, CancellationToken ct = default);

        /// <summary>
        /// Closes an open position.
        /// </summary>
        /// <param name="symbol">The trading symbol.</param>
        /// <param name="positionSide">Position side (optional).</param>
        /// <param name="asset">Asset (optional).</param>
        /// <param name="autoCancel">Auto-cancel flag (optional).</param>
        /// <param name="clientOrderId">Client order ID (optional).</param>
        /// <param name="ct">Cancellation token (optional).</param>
        /// <returns>The result of the close position operation.</returns>
        Task<ApiResult<ClosePositionResponse>> ClosePositionAsync(string symbol, PositionSide? positionSide = null, string? asset = null, bool? autoCancel = null, string? clientOrderId = null, CancellationToken ct = default);

        /// <summary>
        /// Checks a potential order (pre-trade check) without actually placing it.
        /// </summary>
        /// <param name="symbol">The trading symbol.</param>
        /// <param name="side">Order side.</param>
        /// <param name="type">Order type.</param>
        /// <param name="quantity">Order quantity.</param>
        /// <param name="price">Order price (optional).</param>
        /// <param name="positionSide">Position side (optional).</param>
        /// <param name="tradeMode">Trade mode (optional).</param>
        /// <param name="takeProfitTriggerPrice">Take profit trigger price (optional).</param>
        /// <param name="stopLossTriggerPrice">Stop loss trigger price (optional).</param>
        /// <param name="takeProfitOrderPrice">Take profit order price (optional).</param>
        /// <param name="stopLossOrderPrice">Stop loss order price (optional).</param>
        /// <param name="takeProfitTriggerPriceType">Take profit trigger price type (optional).</param>
        /// <param name="stopLossTriggerPriceType">Stop loss price trigger type (optional).</param>
        /// <param name="quantityAsset">Quantity asset (optional).</param>
        /// <param name="reduceOnly">Reduce-only flag (optional).</param>
        /// <param name="ct">Cancellation token (optional).</param>
        /// <returns>The result of the order check.</returns>
        Task<ApiResult<CheckOrderResponse>> OrderCheckAsync(string symbol, OrderSide side, OrderType type, decimal quantity, decimal? price = null, PositionSide? positionSide = null, TradeMode? tradeMode = null, decimal? takeProfitTriggerPrice = null, decimal? stopLossTriggerPrice = null, decimal? takeProfitOrderPrice = null, decimal? stopLossOrderPrice = null, TriggerPriceType? takeProfitTriggerPriceType = null, TriggerPriceType? stopLossTriggerPriceType = null, QuantityAsset? quantityAsset = null, bool? reduceOnly = null, CancellationToken ct = default);

        /// <summary>
        /// Gets detailed information for a single order.
        /// </summary>
        /// <param name="symbol">The trading symbol.</param>
        /// <param name="orderId">Order ID (optional).</param>
        /// <param name="origClientOrderId">Original client order ID (optional).</param>
        /// <param name="ct">Cancellation token (optional).</param>
        /// <returns>The order details.</returns>
        Task<ApiResult<Order>> GetOrderAsync(string symbol, long? orderId = null, string? origClientOrderId = null, CancellationToken ct = default);

        /// <summary>
        /// Gets a list of all open (live/partially filled) orders.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="underlying">Underlying asset (optional).</param>
        /// <param name="orderType">Order type (optional).</param>
        /// <param name="state">Order status (optional).</param>
        /// <param name="instrumentFamily">Instrument family (optional).</param>
        /// <param name="ct">Cancellation token (optional).</param>
        /// <returns>The list of open orders.</returns>
        Task<ApiResult<Order[]>> GetOrdersAsync(string? symbol = null, string? underlying = null, OrderType? orderType = null, OrderStatus? state = null, string? instrumentFamily = null, CancellationToken ct = default);

        /// <summary>
        /// Gets a list of pending Algo orders.
        /// </summary>
        /// <param name="algoOrderType">Algo order type.</param>
        /// <param name="algoId">Algo order ID (optional).</param>
        /// <param name="symbol">Trading symbol (optional).</param>
        /// <param name="startTime">Start time (optional).</param>
        /// <param name="endTime">End time (optional).</param>
        /// <param name="ct">Cancellation token (optional).</param>
        /// <returns>The list of pending Algo orders.</returns>
        Task<ApiResult<AlgoOrder[]>> GetAlgoOrdersAsync(AlgoOrderType algoOrderType, string? algoId = null, string? symbol = null, DateTime? startTime = null, DateTime? endTime = null, CancellationToken ct = default);

        /// <summary>
        /// Gets a list of open positions.
        /// </summary>
        /// <param name="symbol">Trading symbol (optional).</param>
        /// <param name="positionId">Position ID (optional).</param>
        /// <param name="ct">Cancellation token (optional).</param>
        /// <returns>The list of open positions.</returns>
        Task<ApiResult<Position[]>> GetPositionsAsync(string? symbol = null, string? positionId = null, CancellationToken ct = default);

        /// <summary>
        /// Amends an open order.
        /// </summary>
        /// <param name="symbol">Trading symbol.</param>
        /// <param name="orderId">Order ID (optional).</param>
        /// <param name="clientOrderId">Client order ID (optional).</param>
        /// <param name="requestId">Request ID (optional).</param>
        /// <param name="cancelOnFail">Cancel on fail flag (optional).</param>
        /// <param name="newQuantity">New quantity (optional).</param>
        /// <param name="newPrice">New price (optional).</param>
        /// <param name="newTriggerPrice">New trigger price (optional).</param>
        /// <param name="newTakeProfitTriggerPrice">New take profit trigger price (optional).</param>
        /// <param name="newStopLossTriggerPrice">New stop loss trigger price (optional).</param>
        /// <param name="newTakeProfitOrderPrice">New take profit order price (optional).</param>
        /// <param name="newStopLossOrderPrice">New stop loss order price (optional).</param>
        /// <param name="newTakeProfitPriceTriggerType">New take profit price trigger type (optional).</param>
        /// <param name="newStopLossPriceTriggerType">New stop loss price trigger type (optional).</param>
        /// <param name="ct">Cancellation token (optional).</param>
        /// <returns>The result of the amend operation.</returns>
        Task<ApiResult<OrderAmendResponse>> AmendOrderAsync(string symbol, long? orderId = null, string? clientOrderId = null, string? requestId = null, bool? cancelOnFail = null, decimal? newQuantity = null, decimal? newPrice = null, decimal? newTriggerPrice = null, decimal? newTakeProfitTriggerPrice = null, decimal? newStopLossTriggerPrice = null, decimal? newTakeProfitOrderPrice = null, decimal? newStopLossOrderPrice = null, TriggerPriceType? newTakeProfitPriceTriggerType = null, TriggerPriceType? newStopLossPriceTriggerType = null, CancellationToken ct = default);

        /// <summary>
        /// Amends a pending Algo order.
        /// </summary>
        /// <param name="symbol">Trading symbol.</param>
        /// <param name="algoId">Algo order ID (optional).</param>
        /// <param name="clientAlgoId">Client Algo order ID (optional).</param>
        /// <param name="requestId">Request ID (optional).</param>
        /// <param name="cancelOnFail">Cancel on fail flag (optional).</param>
        /// <param name="newQuantity">New quantity (optional).</param>
        /// <param name="newTakeProfitTriggerPrice">New take profit trigger price (optional).</param>
        /// <param name="newStopLossTriggerPrice">New stop loss trigger price (optional).</param>
        /// <param name="newTakeProfitOrderPrice">New take profit order price (optional).</param>
        /// <param name="newStopLossOrderPrice">New stop loss order price (optional).</param>
        /// <param name="newTakeProfitPriceTriggerType">New take profit price trigger type (optional).</param>
        /// <param name="newStopLossPriceTriggerType">New stop loss price trigger type (optional).</param>
        /// <param name="ct">Cancellation token (optional).</param>
        /// <returns>The result of the amend operation.</returns>
        Task<ApiResult<AlgoOrderAmendResponse>> AmendAlgoOrderAsync(string symbol, string? algoId = null, string? clientAlgoId = null, string? requestId = null, bool? cancelOnFail = null, decimal? newQuantity = null, decimal? newTakeProfitTriggerPrice = null, decimal? newStopLossTriggerPrice = null, decimal? newTakeProfitOrderPrice = null, decimal? newStopLossOrderPrice = null, TriggerPriceType? newTakeProfitPriceTriggerType = null, TriggerPriceType? newStopLossPriceTriggerType = null, CancellationToken ct = default);

        /// <summary>
        /// Cancels an open order.
        /// </summary>
        /// <param name="symbol">Trading symbol.</param>
        /// <param name="orderId">Order ID (optional).</param>
        /// <param name="origClientOrderId">Original client order ID (optional).</param>
        /// <param name="ct">Cancellation token (optional).</param>
        /// <returns>The result of the cancel operation.</returns>
        Task<ApiResult<OrderCancelResponse>> CancelOrderAsync(string symbol, long? orderId = null, string? origClientOrderId = null, CancellationToken ct = default);

        /// <summary>
        /// Cancels multiple open orders at once.
        /// </summary>
        /// <param name="orders">The orders to cancel.</param>
        /// <param name="ct">Cancellation token (optional).</param>
        /// <returns>The result of the cancel operation for each order.</returns>
        Task<ApiResult<OrderCancelResponse[]>> CancelMultipleOrdersAsync(IEnumerable<OrderCancelRequest> orders, CancellationToken ct = default);

        /// <summary>
        /// Cancels one or more Algo orders.
        /// </summary>
        /// <param name="orders">The Algo orders to cancel.</param>
        /// <param name="ct">Cancellation token (optional).</param>
        /// <returns>The result of the cancel operation.</returns>
        Task<ApiResult<AlgoOrderResponse>> CancelAlgoOrderAsync(IEnumerable<AlgoOrderRequest> orders, CancellationToken ct = default);

        /// <summary>
        /// Gets detailed information for an Algo order.
        /// </summary>
        /// <param name="algoId">Algo order ID (optional).</param>
        /// <param name="clientAlgoId">Client Algo order ID (optional).</param>
        /// <param name="ct">Cancellation token (optional).</param>
        /// <returns>The Algo order details.</returns>
        Task<ApiResult<AlgoOrder>> GetAlgoOrderAsync(string? algoId = null, string? clientAlgoId = null, CancellationToken ct = default);


        /// <summary>
        /// Gets the history of closed or canceled orders.
        /// Retrieves up to 7 days of data. For longer history, use the GetHistoryOrdersArchiveAsync endpoint for archived data.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="underlying">The underlying asset (optional).</param>
        /// <param name="orderType">The order type (optional).</param>
        /// <param name="state">The order status (optional).</param>
        /// <param name="startTime">The start time for the query (optional).</param>
        /// <param name="endTime">The end time for the query (optional).</param>
        /// <param name="ct">Cancellation token (optional).</param>
        /// <returns>The list of closed or canceled orders.</returns>
        Task<ApiResult<Order[]>> GetHistoryOrdersAsync(string? symbol = null, string? underlying = null, OrderType? orderType = null, OrderStatus? state = null, DateTime? startTime = null, DateTime? endTime = null, CancellationToken ct = default);

        /// <summary>
        /// Gets the history of closed or canceled orders from archived data. Use when more than 3 months of data is needed.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="underlying">The underlying asset (optional).</param>
        /// <param name="orderType">The order type (optional).</param>
        /// <param name="state">The order status (optional).</param>
        /// <param name="startTime">The start time for the query (optional).</param>
        /// <param name="endTime">The end time for the query (optional).</param>
        /// <param name="ct">Cancellation token (optional).</param>
        /// <returns>The list of closed or canceled orders from archive.</returns>
        Task<ApiResult<Order[]>> GetOrdersArchiveAsync(string? symbol = null, string? underlying = null, OrderType? orderType = null, OrderStatus? state = null, DateTime? startTime = null, DateTime? endTime = null, CancellationToken ct = default);

        /// <summary>
        /// Gets the history of closed positions.
        /// Retrieves up to 3 months of data. For longer history, use the GetPositionArchiveAsync endpoint for archived data.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="marginMode">The margin mode (optional).</param>
        /// <param name="type">The closing position type (optional).</param>
        /// <param name="positionId">The position ID (optional).</param>
        /// <param name="endTime">The end time for the query (optional).</param>
        /// <param name="startTime">The start time for the query (optional).</param>
        /// <param name="ct">Cancellation token (optional).</param>
        /// <returns>The list of closed positions.</returns>
        Task<ApiResult<ClosingPosition[]>> GetClosePositionsAsync(string? symbol = null, MarginMode? marginMode = null, ClosingPositionType? type = null, string? positionId = null, DateTime? endTime = null, DateTime? startTime = null, CancellationToken ct = default);

        /// <summary>
        /// Gets the history of matched trades (fills).
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="underlying">The underlying asset (optional).</param>
        /// <param name="orderId">The order ID (optional).</param>
        /// <param name="startTime">The start time for the query (optional).</param>
        /// <param name="endTime">The end time for the query (optional).</param>
        /// <param name="ct">Cancellation token (optional).</param>
        /// <returns>The list of matched trades (fills).</returns>
        Task<ApiResult<Transaction[]>> GetUserTradesAsync(string? symbol = null, string? underlying = null, long? orderId = null, DateTime? startTime = null, DateTime? endTime = null, CancellationToken ct = default);

        /// <summary>
        /// Gets the history of matched trades (fills) from archived data. Use when more than 3 months of data is needed.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="underlying">The underlying asset (optional).</param>
        /// <param name="orderId">The order ID (optional).</param>
        /// <param name="startTime">The start time for the query (optional).</param>
        /// <param name="endTime">The end time for the query (optional).</param>
        /// <param name="ct">Cancellation token (optional).</param>
        /// <returns>The list of matched trades (fills) from archive.</returns>
        Task<ApiResult<Transaction[]>> GetUserTradesArchiveAsync(string? symbol = null, string? underlying = null, long? orderId = null, DateTime? startTime = null, DateTime? endTime = null, CancellationToken ct = default);

        /// <summary>
        /// Sets the default source identifier prefix used when generating client order IDs (clOrdId).
        /// </summary>
        /// <param name="sourceIdPrefix">
        /// A unique prefix used to distinguish orders generated by different bots,
        /// strategies, or trading systems.
        /// </param>
        /// <returns>
        /// A tuple indicating whether the operation succeeded and an optional error message.
        /// </returns>
        (bool Success, string ErrorMsg) SetOrderSourceIdPrefix(string sourceIdPrefix);

        /// <summary>
        /// Disables logging for specified API endpoints.
        /// </summary>
        /// <param name="apiNames">The API endpoints to disable logging for.</param>
        void DisableLogApiEndPoint(IEnumerable<ApiName> apiNames);

    }
}
