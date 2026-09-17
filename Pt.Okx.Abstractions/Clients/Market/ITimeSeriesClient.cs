using Pt.Okx.Shared.Clients.Market.Models;
using Pt.Okx.Shared.Enums;
using Pt.Okx.Abstractions.Indicators.Base;
using Pt.Okx.Abstractions.Indicators.BuiltIn;
using Pt.Okx.Shared.Indicators.Enums;
using Pt.Okx.Shared.Indicators.Models;
using Pt.Okx.Abstractions.Indicators.Services;

namespace Pt.Okx.Abstractions.Clients.Market
{
    /// <summary>
    /// Client interface providing methods for querying time series (OHLCV) data and initializing technical indicators.
    /// </summary>
    public interface ITimeSeriesClient
    {
        #region Configuration and Status
        /// <summary>
        /// Gets the current timeframe (Kline interval) in use.
        /// </summary>
        Timeframe PeriodCurrent { get; }

        /// <summary>
        /// Gets the timestamp at which the warmup data period begins.
        /// </summary>
        /// <remarks>
        /// This time is earlier than the strategy start time and is used to load
        /// enough historical candles for indicators before live or simulated processing starts.
        /// </remarks>
        DateTime BeginTime { get; }

        /// <summary>
        /// Gets the timestamp at which the strategy starts processing trading logic.
        /// </summary>
        /// <remarks>
        /// This is the effective strategy start time after the warmup period has completed.
        /// </remarks>
        DateTime? StartTime { get; }

        /// <summary>
        /// Gets the timestamp when the session ends.
        /// </summary>
        /// <remarks>
        /// Returns <c>null</c> when running in real-time mode.
        /// </remarks>
        DateTime? EndTime { get; }

        /// <summary>
        /// Gets the maximum number of bars (candles) stored in the cache.
        /// </summary>
        int MaxBars { get; }

        /// <summary>
        /// Gets the manager for initialized technical indicators.
        /// </summary>
        IIndicatorManager Indicator { get; }

        /// <summary>
        /// Gets the distinct symbol/timeframe pairs scoped to this strategy instance
        /// (primary pair + indicator pairs). Not the global singleton.
        /// </summary>
        HashSet<(string Symbol, Timeframe Timeframe)> SymbolsTimeframes { get; }

        /// <summary>
        /// Gets the most recent tick price of the asset as received from the market data feed.
        /// </summary>
        decimal CurrentTickPrice { get; }

        /// <summary>
        /// Gets the most recent tick data received from the market data feed.
        /// </summary>
        TickData CurrentTick { get; }

        /// <summary>
        /// Gets the current time based on the specified timeframe.
        /// </summary>
        /// <param name="timeframe">The timeframe to check.</param>
        /// <returns>The current candle time for the specified timeframe.</returns>
        DateTime GetCurrentCandleTime(Timeframe timeframe = Timeframe.OneMinute);

        /// <summary>
        /// Gets the current system time.
        /// </summary>
        /// <returns>The current system time.</returns>
        DateTime GetCurrentTime();

        /// <summary>
        /// Gets the number of bars calculated for the specified indicator.
        /// </summary>
        /// <param name="indicatorId">The unique ID of the indicator.</param>
        /// <exception cref="ArgumentException">Thrown if the indicator ID is not found.</exception>
        /// <returns>The number of bars calculated.</returns>
        int BarsCalculated(string indicatorId);

        /// <summary>
        /// Gets the total number of available bars for the specified trading symbol and timeframe.
        /// </summary>
        /// <param name="symbol">The trading symbol. When <c>null</c>, the current symbol is used.</param>
        /// <param name="timeframe">The timeframe. When <c>null</c>, the current timeframe is used.</param>
        /// <returns>The total number of available bars.</returns>
        int Bars(string? symbol = null, Timeframe? timeframe = null);

        #endregion

        #region OHLCV Data Access

        /// <summary>
        /// Gets the OHLCV data at a specific index (0 = current forming candle, 1 = previous closed candle, etc.).
        /// </summary>
        /// <param name="symbol">The trading symbol.</param>
        /// <param name="timeframe">The timeframe.</param>
        /// <param name="shift">The candle index to retrieve.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The corresponding candle data.</returns>
        Task<CandleData> GetOHLCVAsync(string? symbol = null, Timeframe? timeframe = null, int shift = 0, CancellationToken ct = default);

        /// <summary>
        /// Gets the most recent closed candle data asynchronously.
        /// </summary>
        /// <param name="symbol">The trading symbol. When <c>null</c>, the current symbol is used.</param>
        /// <param name="timeframe">The timeframe. When <c>null</c>, the current timeframe is used.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The most recent closed candle data.</returns>
        /// <remarks>
        /// Shift <c>1</c> usually represents the most recent closed candle,
        /// while shift <c>0</c> represents the currently forming candle.
        /// </remarks>
        Task<CandleData> GetLastClosedCandleAsync(string? symbol = null, Timeframe? timeframe = null, CancellationToken ct = default);

        /// <summary>
        /// Gets the timestamp of the candle at the specified shift for a given timeframe. Shift 0 corresponds to the current forming candle, shift 1 is the last closed candle, and so on.
        /// </summary>
        /// <param name="timeframe">The timeframe for the candle.</param>
        /// <param name="shift">The shift index (0 = current forming candle, 1 = last closed candle, etc.).</param>
        /// <returns>The timestamp of the candle at the specified shift.</returns>
        DateTime GetTime(Timeframe timeframe, int shift);

        /// <summary>
        /// Gets the currently forming candle for the specified trading symbol and timeframe.
        /// </summary>
        /// <param name="symbol">The trading symbol. When <c>null</c>, the current symbol is used.</param>
        /// <param name="timeframe">The timeframe. When <c>null</c>, the current timeframe is used.</param>
        /// <returns>The currently forming candle data.</returns>
        /// <remarks>
        /// The returned candle may still be updated until the timeframe interval is closed.
        /// </remarks>
        CandleData GetOpenCandle(string? symbol = null, Timeframe? timeframe = null);

        /// <summary>
        /// On-demand: recalculate indicators using the current open (forming) candle data.
        /// Follows a lazy evaluation pattern — indicators are only recalculated
        /// when explicitly requested, not automatically on every tick.
        ///
        /// After calling this method, indicator buffers' last element reflects the open candle values.
        /// When the bar closes, the normal close flow overwrites it with the closed candle automatically.
        /// </summary>
        /// <param name="symbol">The trading symbol. When <c>null</c>, the current symbol is used.</param>
        /// <param name="timeframe">The timeframe. When <c>null</c>, the current timeframe is used.</param>
        void UpdateOpenCandleIndicators(string? symbol = null, Timeframe? timeframe = null);


        /// <summary>
        /// Gets the most recent closed candle for the specified trading symbol and timeframe.
        /// </summary>
        /// <param name="symbol">The trading symbol. When <c>null</c>, the current symbol is used.</param>
        /// <param name="timeframe">The timeframe. When <c>null</c>, the current timeframe is used.</param>
        /// <returns>The most recent closed candle data.</returns>
        /// <remarks>
        /// Unlike <see cref="GetOpenCandle(string?, Timeframe?)"/>, this method returns a finalized candle.
        /// </remarks>
        CandleData GetLastClosedCandle(string? symbol = null, Timeframe? timeframe = null);


        #endregion

        #region Copy Series Data
        /// <summary>
        /// Copies an array of OHLCV data within a specified time range.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="timeframe">The timeframe for the data (optional).</param>
        /// <param name="startTime"> The start time of the range. When <c>null</c>, warmup time is used.</param>
        /// <param name="endTime">
        /// The end time of the range. When <c>null</c>, uses <see cref="GetCurrentCandleTime(Timeframe)"/> for the resolved timeframe.
        /// </param>
        /// <returns>An array of OHLCV candle data within the specified time range.</returns>
        /// <remarks>
        /// Time boundaries are normalized to the requested timeframe.
        /// In backtest mode, if the current open candle timestamp falls inside the requested window,
        /// that open candle can be appended as the last item.
        ///
        /// Strict behavior:
        /// - Empty result when start or end boundary is not available in loaded data.
        /// - Returns data only when both normalized boundaries exist in the returned window.
        ///
        /// The same strict boundary rule is inherited by component-copy APIs that rely on this method
        /// (for example <see cref="CopyTimes(string?, Timeframe?, DateTime?, DateTime?)"/>,
        /// <see cref="CopyOpens(string?, Timeframe?, DateTime?, DateTime?)"/>,
        /// <see cref="CopyHighs(string?, Timeframe?, DateTime?, DateTime?)"/>,
        /// <see cref="CopyLows(string?, Timeframe?, DateTime?, DateTime?)"/>,
        /// <see cref="CopyCloses(string?, Timeframe?, DateTime?, DateTime?)"/>,
        /// and <see cref="CopyVolumes(string?, Timeframe?, DateTime?, DateTime?)"/>).
        /// </remarks>
        Task<CandleData[]> CopySeries(string? symbol = null, Timeframe? timeframe = null, DateTime? startTime = null, DateTime? endTime = null);

        /// <summary>
        /// Copies an array of OHLCV data by start position and number of candles.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="timeframe">The timeframe for the data (optional).</param>
        /// <param name="startPos">The starting position (index) in the series.</param>
        /// <param name="count">The number of candles to copy.</param>
        /// <returns>An array of OHLCV candle data starting from the specified position.</returns>
        /// <remarks>
        /// Time boundaries are normalized to the requested timeframe.
        /// In backtest mode, if the current open candle timestamp falls inside the requested window,
        /// that open candle can be appended as the last item.
        /// 
        /// Strict behavior:
        /// - Empty result when start or end boundary is not available in loaded data.
        /// - Returns data only when both normalized boundaries exist in the returned window.
        ///
        /// The same strict boundary rule is inherited by component-copy APIs that rely on this method
        /// (for example <see cref="CopyTimes(string?, Timeframe?, int, int)"/>,
        /// <see cref="CopyOpens(string?, Timeframe?, int, int)"/>,
        /// <see cref="CopyHighs(string?, Timeframe?, int, int)"/>,
        /// <see cref="CopyLows(string?, Timeframe?, int, int)"/>,
        /// <see cref="CopyCloses(string?, Timeframe?, int, int)"/>,
        /// and <see cref="CopyVolumes(string?, Timeframe?, int, int)"/>).
        /// </remarks>
        Task<CandleData[]> CopySeries(string? symbol = null, Timeframe? timeframe = null, int startPos = 0, int count = 1);

        /// <summary>
        /// Copies an array of OHLCV data from a specific time with a defined number of candles.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="timeframe">The timeframe for the data (optional).</param>
        /// <param name="startTime">The start time for copying data.</param>
        /// <param name="count">The number of candles to copy.</param>
        /// <returns>An array of OHLCV candle data starting from the specified time.</returns>
        /// <remarks>
        /// Time boundaries are normalized to the requested timeframe.
        /// In backtest mode, if the current open candle timestamp falls inside the requested window,
        /// that open candle can be appended as the last item.
        /// 
        /// Strict behavior:
        /// - Empty result when start or end boundary is not available in loaded data.
        /// - Returns data only when both normalized boundaries exist in the returned window.
        ///
        /// The same strict boundary rule is inherited by component-copy APIs that rely on this method
        /// (for example <see cref="CopyTimes(string?, Timeframe?, DateTime?, int)"/>,
        /// <see cref="CopyOpens(string?, Timeframe?, DateTime?, int)"/>,
        /// <see cref="CopyHighs(string?, Timeframe?, DateTime?, int)"/>,
        /// <see cref="CopyLows(string?, Timeframe?, DateTime?, int)"/>,
        /// <see cref="CopyCloses(string?, Timeframe?, DateTime?, int)"/>,
        /// and <see cref="CopyVolumes(string?, Timeframe?, DateTime?, int)"/>).
        /// </remarks>
        Task<CandleData[]> CopySeries(string? symbol = null, Timeframe? timeframe = null, DateTime? startTime = null, int count = 1);

        #endregion

        #region Copy Component Data
        /// <summary>
        /// Copies an array of times by position and count.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="timeframe">The timeframe (optional).</param>
        /// <param name="startPos">The starting position (index) in the series (optional, default is 0).</param>
        /// <param name="count">The number of times to copy (optional, default is 1).</param>
        /// <returns>An array of DateTime values representing candle times.</returns>
        /// <remarks>
        /// Applies the same strict boundary behavior as <see cref="CopySeries(string?, Timeframe?, int, int)"/>.
        /// This rule also applies to all <c>CopyTimes</c> overloads.
        /// </remarks>
        Task<DateTime[]> CopyTimes(string? symbol = null, Timeframe? timeframe = null, int startPos = 0, int count = 1);

        /// <summary>
        /// Copies an array of times from a start time with a defined count.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="timeframe">The timeframe (optional).</param>
        /// <param name="startTime">The start time for copying data (optional).</param>
        /// <param name="count">The number of times to copy (optional, default is 1).</param>
        /// <returns>An array of DateTime values representing candle times.</returns>
        /// <remarks>
        /// Applies the same strict boundary behavior as <see cref="CopySeries(string?, Timeframe?, DateTime?, int)"/>.
        /// This rule also applies to all <c>CopyTimes</c> overloads.
        /// </remarks>
        Task<DateTime[]> CopyTimes(string? symbol = null, Timeframe? timeframe = null, DateTime? startTime = null, int count = 1);

        /// <summary>
        /// Copies an array of times within a specified time range.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="timeframe">The timeframe (optional).</param>
        /// <param name="startTime">The start time of the range (optional).</param>
        /// <param name="endTime">The end time of the range (optional).</param>
        /// <returns>An array of DateTime values representing candle times within the specified range.</returns>
        /// <remarks>
        /// Applies the same strict boundary behavior as <see cref="CopySeries(string?, Timeframe?, DateTime?, DateTime?)"/>.
        /// This rule also applies to all <c>CopyTimes</c> overloads.
        /// </remarks>
        Task<DateTime[]> CopyTimes(string? symbol = null, Timeframe? timeframe = null, DateTime? startTime = null, DateTime? endTime = null);

        /// <summary>
        /// Copies an array of open prices by position and count.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="timeframe">The timeframe (optional).</param>
        /// <param name="startPos">The starting position (index) in the series (optional, default is 0).</param>
        /// <param name="count">The number of open prices to copy (optional, default is 1).</param>
        /// <returns>An array of open prices.</returns>
        /// <remarks>
        /// Applies the same strict boundary behavior as <see cref="CopySeries(string?, Timeframe?, int, int)"/>.
        /// This rule also applies to all <c>CopyOpens</c> overloads.
        /// </remarks>
        Task<decimal[]> CopyOpens(string? symbol = null, Timeframe? timeframe = null, int startPos = 0, int count = 1);

        /// <summary>
        /// Copies an array of open prices from a start time with a defined count.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="timeframe">The timeframe (optional).</param>
        /// <param name="startTime">The start time for copying data (optional).</param>
        /// <param name="count">The number of open prices to copy (optional, default is 1).</param>
        /// <returns>An array of open prices.</returns>
        /// <remarks>
        /// Applies the same strict boundary behavior as <see cref="CopySeries(string?, Timeframe?, DateTime?, int)"/>.
        /// This rule also applies to all <c>CopyOpens</c> overloads.
        /// </remarks>
        Task<decimal[]> CopyOpens(string? symbol = null, Timeframe? timeframe = null, DateTime? startTime = null, int count = 1);

        /// <summary>
        /// Copies an array of open prices within a specified time range.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="timeframe">The timeframe (optional).</param>
        /// <param name="startTime">The start time of the range (optional).</param>
        /// <param name="endTime">The end time of the range (optional).</param>
        /// <returns>An array of open prices within the specified range.</returns>
        /// <remarks>
        /// Applies the same strict boundary behavior as <see cref="CopySeries(string?, Timeframe?, DateTime?, DateTime?)"/>.
        /// This rule also applies to all <c>CopyOpens</c> overloads.
        /// </remarks>
        Task<decimal[]> CopyOpens(string? symbol = null, Timeframe? timeframe = null, DateTime? startTime = null, DateTime? endTime = null);

        /// <summary>
        /// Copies an array of high prices by position and count.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="timeframe">The timeframe (optional).</param>
        /// <param name="startPos">The starting position (index) in the series (optional, default is 0).</param>
        /// <param name="count">The number of high prices to copy (optional, default is 1).</param>
        /// <returns>An array of high prices.</returns>
        /// <remarks>
        /// Applies the same strict boundary behavior as <see cref="CopySeries(string?, Timeframe?, int, int)"/>.
        /// This rule also applies to all <c>CopyHighs</c> overloads.
        /// </remarks>
        Task<decimal[]> CopyHighs(string? symbol = null, Timeframe? timeframe = null, int startPos = 0, int count = 1);

        /// <summary>
        /// Copies an array of high prices from a start time with a defined count.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="timeframe">The timeframe (optional).</param>
        /// <param name="startTime">The start time for copying data (optional).</param>
        /// <param name="count">The number of high prices to copy (optional, default is 1).</param>
        /// <returns>An array of high prices.</returns>
        /// <remarks>
        /// Applies the same strict boundary behavior as <see cref="CopySeries(string?, Timeframe?, DateTime?, int)"/>.
        /// This rule also applies to all <c>CopyHighs</c> overloads.
        /// </remarks>
        Task<decimal[]> CopyHighs(string? symbol = null, Timeframe? timeframe = null, DateTime? startTime = null, int count = 1);

        /// <summary>
        /// Copies an array of high prices within a specified time range.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="timeframe">The timeframe (optional).</param>
        /// <param name="startTime">The start time of the range (optional).</param>
        /// <param name="endTime">The end time of the range (optional).</param>
        /// <returns>An array of high prices within the specified range.</returns>
        /// <remarks>
        /// Applies the same strict boundary behavior as <see cref="CopySeries(string?, Timeframe?, DateTime?, DateTime?)"/>.
        /// This rule also applies to all <c>CopyHighs</c> overloads.
        /// </remarks>
        Task<decimal[]> CopyHighs(string? symbol = null, Timeframe? timeframe = null, DateTime? startTime = null, DateTime? endTime = null);

        /// <summary>
        /// Copies an array of low prices by position and count.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="timeframe">The timeframe (optional).</param>
        /// <param name="startPos">The starting position (index) in the series (optional, default is 0).</param>
        /// <param name="count">The number of low prices to copy (optional, default is 1).</param>
        /// <returns>An array of low prices.</returns>
        /// <remarks>
        /// Applies the same strict boundary behavior as <see cref="CopySeries(string?, Timeframe?, int, int)"/>.
        /// This rule also applies to all <c>CopyLows</c> overloads.
        /// </remarks>
        Task<decimal[]> CopyLows(string? symbol = null, Timeframe? timeframe = null, int startPos = 0, int count = 1);

        /// <summary>
        /// Copies an array of low prices from a start time with a defined count.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="timeframe">The timeframe (optional).</param>
        /// <param name="startTime">The start time for copying data (optional).</param>
        /// <param name="count">The number of low prices to copy (optional, default is 1).</param>
        /// <returns>An array of low prices.</returns>
        /// <remarks>
        /// Applies the same strict boundary behavior as <see cref="CopySeries(string?, Timeframe?, DateTime?, int)"/>.
        /// This rule also applies to all <c>CopyLows</c> overloads.
        /// </remarks>
        Task<decimal[]> CopyLows(string? symbol = null, Timeframe? timeframe = null, DateTime? startTime = null, int count = 1);

        /// <summary>
        /// Copies an array of low prices within a specified time range.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="timeframe">The timeframe (optional).</param>
        /// <param name="startTime">The start time of the range (optional).</param>
        /// <param name="endTime">The end time of the range (optional).</param>
        /// <returns>An array of low prices within the specified range.</returns>
        /// <remarks>
        /// Applies the same strict boundary behavior as <see cref="CopySeries(string?, Timeframe?, DateTime?, DateTime?)"/>.
        /// This rule also applies to all <c>CopyLows</c> overloads.
        /// </remarks>
        Task<decimal[]> CopyLows(string? symbol = null, Timeframe? timeframe = null, DateTime? startTime = null, DateTime? endTime = null);

        /// <summary>
        /// Copies an array of close prices by position and count.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="timeframe">The timeframe (optional).</param>
        /// <param name="startPos">The starting position (index) in the series (optional, default is 0).</param>
        /// <param name="count">The number of close prices to copy (optional, default is 1).</param>
        /// <returns>An array of close prices.</returns>
        /// <remarks>
        /// Applies the same strict boundary behavior as <see cref="CopySeries(string?, Timeframe?, DateTime?, DateTime?)"/>.
        /// This rule also applies to all <c>CopyCloses</c> overloads.
        /// </remarks>
        Task<decimal[]> CopyCloses(string? symbol = null, Timeframe? timeframe = null, int startPos = 0, int count = 1);

        /// <summary>
        /// Copies an array of close prices from a start time with a defined count.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="timeframe">The timeframe (optional).</param>
        /// <param name="startTime">The start time for copying data (optional).</param>
        /// <param name="count">The number of close prices to copy (optional, default is 1).</param>
        /// <returns>An array of close prices.</returns>
        /// <remarks>
        /// Applies the same strict boundary behavior as <see cref="CopySeries(string?, Timeframe?, DateTime?, DateTime?)"/>.
        /// This rule also applies to all <c>CopyCloses</c> overloads.
        /// </remarks>
        Task<decimal[]> CopyCloses(string? symbol = null, Timeframe? timeframe = null, DateTime? startTime = null, int count = 1);

        /// <summary>
        /// Copies an array of close prices within a specified time range.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="timeframe">The timeframe (optional).</param>
        /// <param name="startTime">The start time of the range (optional).</param>
        /// <param name="endTime">The end time of the range (optional).</param>
        /// <returns>An array of close prices within the specified range.</returns>
        /// <remarks>
        /// Applies the same strict boundary behavior as <see cref="CopySeries(string?, Timeframe?, DateTime?, DateTime?)"/>.
        /// This rule also applies to all <c>CopyCloses</c> overloads.
        /// </remarks>
        Task<decimal[]> CopyCloses(string? symbol = null, Timeframe? timeframe = null, DateTime? startTime = null, DateTime? endTime = null);

        /// <summary>
        /// Copies an array of volumes by position and count.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="timeframe">The timeframe (optional).</param>
        /// <param name="startPos">The starting position (index) in the series (optional, default is 0).</param>
        /// <param name="count">The number of volumes to copy (optional, default is 1).</param>
        /// <returns>An array of volume values.</returns>
        /// <remarks>
        /// Applies the same strict boundary behavior as <see cref="CopySeries(string?, Timeframe?, DateTime?, DateTime?)"/>.
        /// This rule also applies to all <c>CopyVolumes</c> overloads.
        /// </remarks>
        Task<decimal[]> CopyVolumes(string? symbol = null, Timeframe? timeframe = null, int startPos = 0, int count = 1);

        /// <summary>
        /// Copies an array of volumes from a start time with a defined count.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="timeframe">The timeframe (optional).</param>
        /// <param name="startTime">The start time for copying data (optional).</param>
        /// <param name="count">The number of volumes to copy (optional, default is 1).</param>
        /// <returns>An array of volume values.</returns>
        /// <remarks>
        /// Applies the same strict boundary behavior as <see cref="CopySeries(string?, Timeframe?, DateTime?, DateTime?)"/>.
        /// This rule also applies to all <c>CopyVolumes</c> overloads.
        /// </remarks>
        Task<decimal[]> CopyVolumes(string? symbol = null, Timeframe? timeframe = null, DateTime? startTime = null, int count = 1);

        /// <summary>
        /// Copies an array of volumes within a specified time range.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="timeframe">The timeframe (optional).</param>
        /// <param name="startTime">The start time of the range (optional).</param>
        /// <param name="endTime">The end time of the range (optional).</param>
        /// <returns>An array of volume values within the specified range.</returns>
        /// <remarks>
        /// Applies the same strict boundary behavior as <see cref="CopySeries(string?, Timeframe?, DateTime?, DateTime?)"/>.
        /// This rule also applies to all <c>CopyVolumes</c> overloads.
        /// </remarks>
        Task<decimal[]> CopyVolumes(string? symbol = null, Timeframe? timeframe = null, DateTime? startTime = null, DateTime? endTime = null);

        #endregion

        #region Copy Prices by Type

        /// <summary>
        /// Copies prices by AppliedPrice type within a specified time range.
        /// </summary>
        /// <param name="appliedPrice">The type of price to extract (e.g., close, open, high, low) (optional, defaults to close).</param>
        /// <remarks>Parameters <c>symbol</c> and <c>timeframe</c> are optional and default to the current settings if not provided.</remarks>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="timeframe">The timeframe for the data (optional).</param>
        /// <param name="start">The start time of the range (optional).</param>
        /// <param name="endTime">The end time of the range (optional).</param>
        /// <returns>An enumerable of (DateTime, price) tuples within the specified time range.</returns>
        /// <remarks>
        /// Applies the same strict boundary behavior as <see cref="CopySeries(string?, Timeframe?, DateTime?, DateTime?)"/>.
        /// This rule also applies to all <c>CopyPrices</c> overloads that retrieve candles by range/count.
        /// </remarks>
        Task<IEnumerable<PriceValue>> CopyPrices(AppliedPrice appliedPrice = AppliedPrice.Close, string? symbol = null, Timeframe? timeframe = null, DateTime? start = null, DateTime? endTime = null);

        /// <summary>
        /// Copies prices by AppliedPrice type from a start time with a defined count.
        /// </summary>
        /// <param name="appliedPrice">The type of price to extract (e.g., close, open, high, low) (optional, defaults to close).</param>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="timeframe">The timeframe for the data (optional).</param>
        /// <param name="start">The start time for copying data (optional).</param>
        /// <param name="count">The number of prices to copy (optional, defaults to 1).</param>
        /// <returns>An enumerable of (DateTime, price) tuples starting from the specified time.</returns>
        /// <remarks>
        /// Applies the same strict boundary behavior as <see cref="CopySeries(string?, Timeframe?, DateTime?, int)"/>.
        /// This rule also applies to all <c>CopyPrices</c> overloads.
        /// </remarks>
        Task<IEnumerable<PriceValue>> CopyPrices(AppliedPrice appliedPrice = AppliedPrice.Close, string? symbol = null, Timeframe? timeframe = null, DateTime? start = null, int count = 1);

        /// <summary>
        /// Copies prices by AppliedPrice type by index and count.
        /// </summary>
        /// <param name="appliedPrice">The type of price to extract (e.g., close, open, high, low) (optional, defaults to close).</param>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="timeframe">The timeframe for the data (optional).</param>
        /// <param name="startPos">The starting position (index) in the series (optional, defaults to 0).</param>
        /// <param name="count">The number of prices to copy (optional, defaults to 1).</param>
        /// <returns>An enumerable of (DateTime, price) tuples starting from the specified position.</returns>
        /// <remarks>
        /// Applies the same strict boundary behavior as <see cref="CopySeries(string?, Timeframe?, int, int)"/>.
        /// This rule also applies to all <c>CopyPrices</c> overloads.
        /// </remarks>
        Task<IEnumerable<PriceValue>> CopyPrices(AppliedPrice appliedPrice = AppliedPrice.Close, string? symbol = null, Timeframe? timeframe = null, int startPos = 0, int count = 1);

        /// <summary>
        /// Extracts a list of prices from an existing OHLCV data series.
        /// </summary>
        /// <param name="appliedPrice">The type of price to extract (e.g., close, open, high, low).</param>
        /// <param name="ohlcvs">The OHLCV data series.</param>
        /// <returns>An enumerable of price values extracted from the OHLCV series.</returns>
        /// <remarks>
        /// This method does not query market data. It only extracts values from the supplied candle collection.
        /// </remarks>
        IEnumerable<PriceValue> CopyPrices(AppliedPrice appliedPrice, IEnumerable<CandleData> ohlcvs);

        /// <summary>
        /// Extracts a specific price value from a candle based on AppliedPrice.
        /// </summary>
        /// <param name="appliedPrice">The type of price to extract (e.g., close, open, high, low).</param>
        /// <param name="ohlcv">The candle data to extract the price from.</param>
        /// <returns>The extracted price value from the candle.</returns>
        /// <remarks>
        /// This method does not query market data. It only extracts values from the supplied candle collection.
        /// </remarks>
        PriceValue CopyPrice(AppliedPrice appliedPrice, CandleData ohlcv);

        #endregion

        #region Indicator Buffer Management

        /// <summary>
        /// Copies indicator buffer values by index and count.
        /// </summary>
        /// <param name="indicatorHandle">The unique handle of the indicator.</param>
        /// <param name="bufferNumber">The buffer number to copy from.</param>
        /// <param name="startIndex">The starting index in the buffer.</param>
        /// <param name="count">The number of values to copy.</param>
        /// <param name="buffers">The output enumerable of copied indicator values.</param>
        /// <remarks>
        /// The indicator handle must refer to an indicator that has already been created and registered
        /// in the indicator manager.
        /// </remarks>
        /// <returns>The number of values copied.</returns>
        int CopyBuffer(string indicatorHandle, int bufferNumber, int startIndex, int count, out IEnumerable<IndicatorValue> buffers);
        /// <summary>
        /// Copies indicator buffer values from a start time with a defined count.
        /// </summary>
        /// <param name="indicatorHandle">The unique handle of the indicator.</param>
        /// <param name="bufferNumber">The buffer number to copy from.</param>
        /// <param name="startTime">The start time for copying data.</param>
        /// <param name="count">The number of values to copy.</param>
        /// <param name="buffers">The output enumerable of copied indicator values.</param>
        /// <remarks>
        /// The indicator handle must refer to an indicator that has already been created and registered
        /// in the indicator manager.
        /// </remarks>
        /// <returns>The number of values copied.</returns>
        int CopyBuffer(string indicatorHandle, int bufferNumber, DateTime startTime, int count, out IEnumerable<IndicatorValue> buffers);
        /// <summary>
        /// Copies indicator buffer values within a specified time range.
        /// </summary>
        /// <param name="indicatorHandle">The unique handle of the indicator.</param>
        /// <param name="bufferNumber">The buffer number to copy from.</param>
        /// <param name="startTime">The start time of the range.</param>
        /// <param name="endTime">The end time of the range.</param>
        /// <param name="buffers">The output enumerable of copied indicator values.</param>
        /// <remarks>
        /// The indicator handle must refer to an indicator that has already been created and registered
        /// in the indicator manager.
        /// </remarks>
        /// <returns>The number of values copied.</returns>
        int CopyBuffer(string indicatorHandle, int bufferNumber, DateTime startTime, DateTime endTime, out IEnumerable<IndicatorValue> buffers);

        #endregion

        #region Specific Technical Indicators

        /// <summary>
        /// Creates a Moving Average (MA) indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="period">Moving average period. Default: 14.</param>
        /// <param name="method">Moving average method. Default: <see cref="MaMethod.SMA"/>.</param>
        /// <param name="appliedPrice">Source price. Default: <see cref="AppliedPrice.Close"/>.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created Moving Average indicator instance.</returns>
        IIndicatorMA CreateIndicatorMA(string? symbol = null, Timeframe? timeframe = null, int? period = null, MaMethod? method = null, AppliedPrice? appliedPrice = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a Relative Strength Index (RSI) indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="period">RSI period. Default: 14.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created RSI indicator instance.</returns>
        IIndicatorRSI CreateIndicatorRSI(string? symbol = null, Timeframe? timeframe = null, int? period = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a Stochastic Oscillator indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="kPeriod">%K period. Default: 14.</param>
        /// <param name="dPeriod">%D period. Default: 3.</param>
        /// <param name="kSlow">%K slowing factor. Default: 5.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created Stochastic Oscillator indicator instance.</returns>
        IIndicatorStochastic CreateIndicatorStochastic(string? symbol = null, Timeframe? timeframe = null, int? kPeriod = null, int? dPeriod = null, int? kSlow = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a Moving Average Convergence Divergence (MACD) indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="fastPeriod">Fast EMA period. Default: 12.</param>
        /// <param name="slowPeriod">Slow EMA period. Default: 26.</param>
        /// <param name="signalPeriod">Signal EMA period. Default: 9.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created MACD indicator instance.</returns>
        IIndicatorMACD CreateIndicatorMACD(string? symbol = null, Timeframe? timeframe = null, int? fastPeriod = null, int? slowPeriod = null, int? signalPeriod = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates an Average True Range (ATR) indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="period">ATR period. Default: 14.</param>
        /// <param name="method">Smoothing method. Default: <see cref="MaMethod.SMMA"/>.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created ATR indicator instance.</returns>
        IIndicatorATR CreateIndicatorATR(string? symbol = null, Timeframe? timeframe = null, int? period = null, MaMethod? method = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a SuperTrend indicator for trend identification.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="period">ATR period used by SuperTrend. Default: 14.</param>
        /// <param name="multiplier">ATR multiplier. Default: 3.0.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created SuperTrend indicator instance.</returns>
        IIndicatorSuperTrend CreateIndicatorSuperTrend(string? symbol = null, Timeframe? timeframe = null, int? period = null, double? multiplier = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates an Ichimoku indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="tenkanPeriod">Tenkan-sen period. Default: 9.</param>
        /// <param name="kijunPeriod">Kijun-sen period. Default: 26.</param>
        /// <param name="senkouBPeriod">Senkou Span B period. Default: 52.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created Ichimoku indicator instance.</returns>
        IIndicatorIchimoku CreateIndicatorIchimoku(string? symbol = null, Timeframe? timeframe = null, int? tenkanPeriod = null, int? kijunPeriod = null, int? senkouBPeriod = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a Volume Weighted Average Price (VWAP) indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="resetDaily">Whether VWAP resets each day. Default: <c>true</c>.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created VWAP indicator instance.</returns>
        IIndicatorVWAP CreateIndicatorVWAP(string? symbol = null, Timeframe? timeframe = null, bool? resetDaily = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a Volumes indicator that renders raw volume as a histogram.
        /// </summary>
        /// <param name="symbol">The trading symbol (optional).</param>
        /// <param name="timeframe">The timeframe for the indicator (optional).</param>
        /// <param name="indicatorAlias">An alias for the indicator instance (optional).</param>
        /// <param name="propertyOptions">Optional action to configure indicator properties.</param>
        /// <returns>The created Volumes indicator instance.</returns>
        IIndicatorVolumes CreateIndicatorVolumes(string? symbol = null, Timeframe? timeframe = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a Volume Spike indicator for detecting volume surges.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="period">Volume average period. Default: 20.</param>
        /// <param name="spikeThreshold">Spike multiplier. Default: 1.5.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created Volume Spike indicator instance.</returns>
        IIndicatorVolumeSpike CreateIndicatorVolumeSpike(string? symbol = null, Timeframe? timeframe = null, int? period = null, double? spikeThreshold = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates an Accumulation/Distribution (AD) indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created AD indicator instance.</returns>
        IIndicatorAD CreateIndicatorAD(string? symbol = null, Timeframe? timeframe = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates an Average Directional Index Wilder (ADXW) indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="period">ADXW period. Default: 14.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created ADXW indicator instance.</returns>
        IIndicatorADXW CreateIndicatorADXW(string? symbol = null, Timeframe? timeframe = null, int? period = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a Bollinger Bands Width/Money Flow Index (BWMFI) indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="multiplier">BWMFI multiplier. Default: 1.0.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created BWMFI indicator instance.</returns>
        IIndicatorBWMFI CreateIndicatorBWMFI(string? symbol = null, Timeframe? timeframe = null, double? multiplier = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a DeMarker indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="period">DeMarker period. Default: 14.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created DeMarker indicator instance.</returns>
        IIndicatorDeMarker CreateIndicatorDeMarker(string? symbol = null, Timeframe? timeframe = null, int? period = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates an On Balance Volume (OBV) indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created OBV indicator instance.</returns>
        IIndicatorOBV CreateIndicatorOBV(string? symbol = null, Timeframe? timeframe = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates an Oscillator of Moving Average (OsMA) indicator from two source indicator IDs.
        /// </summary>
        /// <param name="sourceId1">First source indicator ID.</param>
        /// <param name="sourceId2">Second source indicator ID.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created OsMA indicator instance.</returns>
        IIndicatorOsMA CreateIndicatorOsMA(string sourceId1, string sourceId2, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a Standard Deviation (StdDev) indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="period">StdDev period. Default: 20.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created StdDev indicator instance.</returns>
        IIndicatorStdDev CreateIndicatorStdDev(string? symbol = null, Timeframe? timeframe = null, int? period = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a TRIX indicator for momentum analysis.
        /// </summary>
        /// <param name="sourceId">Source indicator ID for TRIX input.</param>
        /// <param name="period">TRIX period. Default: 14.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created TRIX indicator instance.</returns>
        IIndicatorTRIX CreateIndicatorTRIX(string sourceId, int? period = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a Williams %R (WPR) indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="period">WPR period. Default: 14.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created WPR indicator instance.</returns>
        IIndicatorWPR CreateIndicatorWPR(string? symbol = null, Timeframe? timeframe = null, int? period = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates an Accelerator Oscillator (AC) indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created AC indicator instance.</returns>
        IIndicatorAC CreateIndicatorAC(string? symbol = null, Timeframe? timeframe = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates an Awesome Oscillator (AO) indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created AO indicator instance.</returns>
        IIndicatorAO CreateIndicatorAO(string? symbol = null, Timeframe? timeframe = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates an Alligator indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created Alligator indicator instance.</returns>
        IIndicatorAlligator CreateIndicatorAlligator(string? symbol = null, Timeframe? timeframe = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a Gator Oscillator indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created Gator indicator instance.</returns>
        IIndicatorGator CreateIndicatorGator(string? symbol = null, Timeframe? timeframe = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates an Envelopes indicator for upper/lower band trading envelopes.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="period">Envelope MA period. Default: 14.</param>
        /// <param name="deviation">Envelope deviation. Default: 0.1.</param>
        /// <param name="method">Moving average method. Default: <see cref="MaMethod.SMA"/>.</param>
        /// <param name="appliedPrice">Source price. Default: <see cref="AppliedPrice.Close"/>.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created Envelopes indicator instance.</returns>
        IIndicatorEnvelopes CreateIndicatorEnvelopes(string? symbol = null, Timeframe? timeframe = null, int? period = null, double? deviation = null, MaMethod? method = null, AppliedPrice? appliedPrice = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a Force Index indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="period">Force Index period. Default: 13.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created Force indicator instance.</returns>
        IIndicatorForce CreateIndicatorForce(string? symbol = null, Timeframe? timeframe = null, int? period = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a Fractals indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created Fractals indicator instance.</returns>
        IIndicatorFractals CreateIndicatorFractals(string? symbol = null, Timeframe? timeframe = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a Bears Power indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="period">Bears Power period. Default: 13.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created Bears Power indicator instance.</returns>
        IIndicatorBears CreateIndicatorBears(string? symbol = null, Timeframe? timeframe = null, int? period = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a Bulls Power indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="period">Bulls Power period. Default: 13.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created Bulls Power indicator instance.</returns>
        IIndicatorBulls CreateIndicatorBulls(string? symbol = null, Timeframe? timeframe = null, int? period = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a Chaikin Oscillator indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="fast">Fast period. Default: 3.</param>
        /// <param name="slow">Slow period. Default: 10.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created Chaikin indicator instance.</returns>
        IIndicatorChaikin CreateIndicatorChaikin(string? symbol = null, Timeframe? timeframe = null, int? fast = null, int? slow = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a Money Flow Index (MFI) indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="period">MFI period. Default: 14.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created MFI indicator instance.</returns>
        IIndicatorMFI CreateIndicatorMFI(string? symbol = null, Timeframe? timeframe = null, int? period = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a Relative Vigor Index (RVI) indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="period">RVI period. Default: 10.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created RVI indicator instance.</returns>
        IIndicatorRVI CreateIndicatorRVI(string? symbol = null, Timeframe? timeframe = null, int? period = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

#pragma warning disable CA1716
        /// <summary>
        /// Creates a Parabolic SAR indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="step">SAR step increment. Default: 0.02.</param>
        /// <param name="maximum">SAR maximum step. Default: 0.2.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created SAR indicator instance.</returns>
        IIndicatorSAR CreateIndicatorSAR(string? symbol = null, Timeframe? timeframe = null, double? step = null, double? maximum = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);
#pragma warning restore CA1716

        /// <summary>
        /// Creates a Commodity Channel Index (CCI) indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="period">CCI period. Default: 14.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created CCI indicator instance.</returns>
        IIndicatorCCI CreateIndicatorCCI(string? symbol = null, Timeframe? timeframe = null, int? period = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a Triple Exponential Moving Average (TEMA) indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="period">TEMA period. Default: 14.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created TEMA indicator instance.</returns>
        IIndicatorTEMA CreateIndicatorTEMA(string? symbol = null, Timeframe? timeframe = null, int? period = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates an Adaptive Moving Average (AMA) indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="period">AMA period. Default: 10.</param>
        /// <param name="fast">Fast period. Default: 2.</param>
        /// <param name="slow">Slow period. Default: 30.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created AMA indicator instance.</returns>
        IIndicatorAMA CreateIndicatorAMA(string? symbol = null, Timeframe? timeframe = null, int? period = null, int? fast = null, int? slow = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a Double Exponential Moving Average (DEMA) indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="period">DEMA period. Default: 14.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created DEMA indicator instance.</returns>
        IIndicatorDEMA CreateIndicatorDEMA(string? symbol = null, Timeframe? timeframe = null, int? period = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a Momentum indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="period">Momentum period. Default: 14.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created Momentum indicator instance.</returns>
        IIndicatorMomentum CreateIndicatorMomentum(string? symbol = null, Timeframe? timeframe = null, int? period = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates an Average Directional Index (ADX) indicator for measuring trend strength.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="period">ADX period. Default: 14.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created ADX indicator instance.</returns>
        IIndicatorADX CreateIndicatorADX(string? symbol = null, Timeframe? timeframe = null, int? period = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);
        /// <summary>
        /// Creates a Bollinger Bands indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="period">Bollinger period. Default: 20.</param>
        /// <param name="multiplier">Band multiplier. Default: 2.0.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created Bollinger Bands indicator.</returns>
        IIndicatorBollingerBands CreateIndicatorBollingerBands(string? symbol = null, Timeframe? timeframe = null, int? period = null, double? multiplier = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a Bollinger Bands %B indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="period">Bollinger period. Default: 20.</param>
        /// <param name="multiplier">Band multiplier. Default: 2.0.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created Bollinger Bands %B indicator.</returns>
        IIndicatorBollingerPercentB CreateIndicatorBollingerPercentB(string? symbol = null, Timeframe? timeframe = null, int? period = null, double? multiplier = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a Bollinger Band Width indicator.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="period">Bollinger period. Default: 20.</param>
        /// <param name="multiplier">Band multiplier. Default: 2.0.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created Bollinger Band Width indicator.</returns>
        IIndicatorBollingerBandWidth CreateIndicatorBollingerBandWidth(string? symbol = null, Timeframe? timeframe = null, int? period = null, double? multiplier = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a Smoothed RSI indicator (RSI smoothed by MA).
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="rsiPeriod">RSI period. Default: 14.</param>
        /// <param name="maPeriod">Smoothing MA period. Default: 10.</param>
        /// <param name="maMethod">Smoothing MA method. Default: <see cref="MaMethod.SMA"/>.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created Smoothed RSI indicator.</returns>
        IIndicator CreateIndicatorSmoothedRSI(string? symbol = null, Timeframe? timeframe = null, int? rsiPeriod = null, int? maPeriod = null, MaMethod? maMethod = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates a Difference indicator between two data sources.
        /// </summary>
        /// <param name="sourceId1">First source indicator ID.</param>
        /// <param name="sourceId2">Second source indicator ID.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created Difference indicator.</returns>
        IIndicatorDiff CreateIndicatorDiff(string sourceId1, string sourceId2, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);

        /// <summary>
        /// Creates an MA Spread indicator (difference between two MAs).
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="fastPeriod">Fast MA period. Default: 10.</param>
        /// <param name="slowPeriod">Slow MA period. Default: 20.</param>
        /// <param name="method">Moving average method. Default: <see cref="MaMethod.SMA"/>.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <returns>The created MA Spread indicator.</returns>
        IIndicator CreateIndicatorMASpread(string? symbol = null, Timeframe? timeframe = null, int fastPeriod = 10, int slowPeriod = 20, MaMethod method = MaMethod.SMA, string? indicatorAlias = null);

        /// <summary>
        /// Creates an indicator for the distance between the current price and a higher timeframe MA.
        /// </summary>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="htf">Higher timeframe. Default: <see cref="Timeframe.OneHour"/>.</param>
        /// <param name="htfPeriod">Higher-timeframe MA period. Default: 200.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <returns>The created price-to-HTF-MA distance indicator.</returns>
        IIndicator CreateIndicatorPriceHTFDiff(string? symbol = null, Timeframe? htf = null, int htfPeriod = 200, string? indicatorAlias = null);

        /// <summary>
        /// Creates a custom indicator loaded from an external plugin DLL.
        /// </summary>
        /// <param name="customName">The registered name of the custom indicator.</param>
        /// <param name="symbol">Trading symbol. Defaults to the current symbol.</param>
        /// <param name="timeframe">Timeframe. Defaults to the current timeframe.</param>
        /// <param name="parameters">Optional custom parameter dictionary.</param>
        /// <param name="indicatorAlias">Optional indicator alias.</param>
        /// <param name="propertyOptions">Optional property configuration.</param>
        /// <returns>The created custom indicator.</returns>
        IIndicator CreateCustomIndicator(string customName, string? symbol = null, Timeframe? timeframe = null, Dictionary<string, object>? parameters = null, string? indicatorAlias = null, Action<IndicatorProperty>? propertyOptions = null);
        #endregion
    }
}


