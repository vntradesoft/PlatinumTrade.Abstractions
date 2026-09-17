using Pt.Okx.Abstractions.Indicators.Base;
using Pt.Okx.Shared.Indicators.Enums;
using Pt.Okx.Shared.Indicators.Models;

namespace Pt.Okx.Abstractions.Indicators.BuiltIn;

/// <summary>
/// Defines the contract for a moving average indicator that provides trend analysis and crossover detection
/// functionality.
/// </summary>
public interface IIndicatorMA : IIndicator, IIndicatorMethodCommon
{
    /// <summary>
    /// Determines the moving average trend direction over a specified lookback period.
    /// </summary>
    /// <param name="lookback">The number of most recent data points to consider when calculating the trend. Default is 5.</param>
    /// <param name="tolerance">The minimum absolute difference required to consider the trend as upward or downward. Default is 1e-6.</param>
    /// <returns>A value indicating whether the trend is upward, downward, or flat.</returns>
    MATrendDirection GetTrend(int lookback = 5, double tolerance = 1e-6);

    /// <summary>
    /// Determines the type of crossover event between the specified fast and slow moving average indicators.
    /// </summary>
    /// <param name="fastMA">The fast moving average indicator to evaluate.</param>
    /// <param name="slowMA">The slow moving average indicator to evaluate.</param>
    /// <returns>A value indicating whether a bullish, bearish, or no crossover has occurred.</returns>
    MACrossoverType DetectCrossover(IIndicatorMA fastMA, IIndicatorMA slowMA);
}

/// <summary>
/// Defines the contract for a Double Exponential Moving Average (DEMA) indicator.
/// </summary>
public interface IIndicatorDEMA : IIndicator, IIndicatorMethodCommon
{
}

/// <summary>
/// Represents a Triple Exponential Moving Average (TEMA) indicator.
/// </summary>
public interface IIndicatorTEMA : IIndicator, IIndicatorMethodCommon
{
}

/// <summary>
/// Defines the interface for an Adaptive Moving Average (AMA) indicator.
/// </summary>
public interface IIndicatorAMA : IIndicator, IIndicatorMethodCommon
{
}

/// <summary>
/// Defines the contract for a Parabolic SAR (Stop and Reverse) indicator.
/// </summary>
public interface IIndicatorSAR : IIndicator, IIndicatorMethodCommon
{
    /// <summary>
    /// Determines whether the current market condition is bullish.
    /// </summary>
    /// <returns><c>true</c> if the market is considered bullish; otherwise, <c>false</c>.</returns>
    bool IsBullish();

    /// <summary>
    /// Determines whether the current condition is bearish.
    /// </summary>
    /// <returns><c>true</c> if the condition is bearish; otherwise, <c>false</c>.</returns>
    bool IsBearish();
}

/// <summary>
/// Defines the contract for envelope-based technical indicators (e.g., Moving Average Envelopes).
/// </summary>
public interface IIndicatorEnvelopes : IIndicator, IIndicatorMethodCommon
{
    /// <summary>
    /// Gets the lower band value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The lower band value.</returns>
    IndicatorValue GetLower(int index = 0);

    /// <summary>
    /// Gets a range of lower band values.
    /// </summary>
    /// <param name="count">The number of values to retrieve. Default is 1.</param>
    /// <returns>A collection of lower band values.</returns>
    IEnumerable<IndicatorValue> GetLowerRange(int count = 1);

    /// <summary>
    /// Gets the upper band value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The upper band value.</returns>
    IndicatorValue GetUpper(int index = 0);

    /// <summary>
    /// Gets a range of upper band values.
    /// </summary>
    /// <param name="count">The number of values to retrieve. Default is 1.</param>
    /// <returns>A collection of upper band values.</returns>
    IEnumerable<IndicatorValue> GetUpperRange(int count = 1);

    /// <summary>
    /// Determines whether the specified price exceeds the upper limit.
    /// </summary>
    /// <param name="price">The price to evaluate.</param>
    /// <returns><c>true</c> if the price is above the upper limit; otherwise, <c>false</c>.</returns>
    bool IsPriceAboveUpper(double price);

    /// <summary>
    /// Determines whether the specified price is below the lower threshold.
    /// </summary>
    /// <param name="price">The price to evaluate.</param>
    /// <returns><c>true</c> if the price is below the lower threshold; otherwise, <c>false</c>.</returns>
    bool IsPriceBelowLower(double price);
}

/// <summary>
/// Defines the contract for a SuperTrend indicator.
/// </summary>
public interface IIndicatorSuperTrend : IIndicator, IIndicatorMethodCommon
{
    /// <summary>
    /// Gets the Average True Range (ATR) value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The ATR value.</returns>
    IndicatorValue GetAtr(int index = 0);

    /// <summary>
    /// Calculates the distance between the specified close price and the SuperTrend value.
    /// </summary>
    /// <param name="closePrice">The closing price to compare.</param>
    /// <param name="index">The zero-based index of the SuperTrend value. Default is 0.</param>
    /// <returns>The difference between the close price and the SuperTrend value.</returns>
    double GetDistanceFromSuperTrend(double closePrice, int index = 0);

    /// <summary>
    /// Calculates the smoothed Average True Range (ATR) using an exponential moving average.
    /// </summary>
    /// <param name="alpha">The smoothing factor (between 0 and 1). Default is 0.3.</param>
    /// <returns>The smoothed ATR value.</returns>
    double GetSmoothedATR(double alpha = 0.3);

    /// <summary>
    /// Retrieves the trend direction at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>A value representing the trend direction (e.g., 1 for bullish, -1 for bearish).</returns>
    IndicatorValue GetTrendDirection(int index = 0);

    /// <summary>
    /// Determines if there is a bearish reversal at the current index.
    /// </summary>
    /// <returns><c>true</c> if a bearish reversal is detected; otherwise, <c>false</c>.</returns>
    bool HasBearishReversal();

    /// <summary>
    /// Determines whether a bearish reversal pattern has been confirmed within the specified number of bars.
    /// </summary>
    /// <param name="confirmBars">The number of most recent bars to check for confirmation. Default is 1.</param>
    /// <returns><c>true</c> if confirmed; otherwise, <c>false</c>.</returns>
    bool HasBearishReversalConfirmed(int confirmBars = 1);

    /// <summary>
    /// Determines whether a bullish reversal pattern is present.
    /// </summary>
    /// <returns><c>true</c> if a bullish reversal is detected; otherwise, <c>false</c>.</returns>
    bool HasBullishReversal();

    /// <summary>
    /// Determines whether a bullish reversal pattern has been confirmed within the specified number of bars.
    /// </summary>
    /// <param name="confirmBars">The number of most recent bars to check for confirmation. Default is 1.</param>
    /// <returns><c>true</c> if confirmed; otherwise, <c>false</c>.</returns>
    bool HasBullishReversalConfirmed(int confirmBars = 1);

    /// <summary>
    /// Determines whether the ATR has been increasing over the specified lookback period.
    /// </summary>
    /// <param name="lookback">The number of periods to evaluate. Default is 3.</param>
    /// <returns><c>true</c> if the ATR is increasing; otherwise, <c>false</c>.</returns>
    bool IsATRIncreasing(int lookback = 3);

    /// <summary>
    /// Determines whether the current market condition is bearish.
    /// </summary>
    /// <returns><c>true</c> if the market is bearish; otherwise, <c>false</c>.</returns>
    bool IsBearish();

    /// <summary>
    /// Determines whether the current market condition is bullish.
    /// </summary>
    /// <returns><c>true</c> if the market is bullish; otherwise, <c>false</c>.</returns>
    bool IsBullish();

    /// <summary>
    /// Determines whether the close price is sufficiently distant from the SuperTrend line.
    /// </summary>
    /// <param name="closePrice">The closing price to evaluate.</param>
    /// <param name="atrMultiplier">The ATR multiple threshold. Default is 0.3.</param>
    /// <param name="index">The zero-based index of the SuperTrend value. Default is 0.</param>
    /// <returns><c>true</c> if the distance exceeds the threshold; otherwise, <c>false</c>.</returns>
    bool IsFarEnoughFromSuperTrend(double closePrice, double atrMultiplier = 0.3, int index = 0);

    /// <summary>
    /// Determines whether the specified closing price represents a strong breakout.
    /// </summary>
    /// <param name="closePrice">The closing price to evaluate.</param>
    /// <param name="thresholdAtr">The ATR multiple threshold for breakout. Default is 0.5.</param>
    /// <returns><c>true</c> if a strong breakout is detected; otherwise, <c>false</c>.</returns>
    bool IsStrongBreakout(double closePrice, double thresholdAtr = 0.5);

    /// <summary>
    /// Determines whether the trend is considered stable over a specified lookback period.
    /// </summary>
    /// <param name="lookback">The number of data points to evaluate. Default is 5.</param>
    /// <returns><c>true</c> if the trend is stable; otherwise, <c>false</c>.</returns>
    bool IsTrendStable(int lookback = 5);

    /// <summary>
    /// Determines whether the specified close price is within the allowed distance from the SuperTrend line.
    /// </summary>
    /// <param name="closePrice">The closing price to evaluate.</param>
    /// <param name="maxAtrMultiplier">The maximum allowed ATR multiple. Default is 2.0.</param>
    /// <param name="index">The zero-based index of the SuperTrend value. Default is 0.</param>
    /// <returns><c>true</c> if the price is within the specified distance; otherwise, <c>false</c>.</returns>
    bool IsWithinSuperTrendDistance(double closePrice, double maxAtrMultiplier = 2, int index = 0);
}

/// <summary>
/// Defines the contract for an Ichimoku Kinko Hyo indicator.
/// </summary>
public interface IIndicatorIchimoku : IIndicator
{
    /// <summary>
    /// Retrieves all Ichimoku component values at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>A tuple containing Tenkan, Kijun, SenkouA, SenkouB, and Chikou values.</returns>
    (IndicatorValue Tenkan, IndicatorValue Kijun, IndicatorValue SenkouA, IndicatorValue SenkouB, IndicatorValue Chikou) GetIchimoku(int index = 0);
}

/// <summary>
/// Defines the contract for a Bollinger Bands indicator.
/// </summary>
public interface IIndicatorBollingerBands : IIndicator, IIndicatorMethodCommon
{
    /// <summary>
    /// Gets the lower band value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The lower band value.</returns>
    IndicatorValue GetLower(int index = 0);

    /// <summary>
    /// Gets the middle band (basis) value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The middle band value.</returns>
    IndicatorValue GetMiddle(int index = 0);

    /// <summary>
    /// Gets the upper band value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The upper band value.</returns>
    IndicatorValue GetUpper(int index = 0);

    /// <summary>
    /// Gets the Bollinger Band Width at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The band width value.</returns>
    IndicatorValue GetWidth(int index = 0);

    /// <summary>
    /// Determines whether the bands are expanding (volatility increasing).
    /// </summary>
    /// <param name="threshold">The expansion threshold. Default is 2.5.</param>
    /// <returns><c>true</c> if expanding; otherwise, <c>false</c>.</returns>
    bool IsExpansion(double threshold = 2.5);

    /// <summary>
    /// Determines whether the bands are in a squeeze (volatility decreasing).
    /// </summary>
    /// <param name="threshold">The squeeze threshold. Default is 1.5.</param>
    /// <returns><c>true</c> if in a squeeze; otherwise, <c>false</c>.</returns>
    bool IsSqueeze(double threshold = 1.5);

    /// <summary>
    /// Determines whether volatility has been decreasing over the specified lookback period.
    /// </summary>
    /// <param name="lookback">The number of periods to evaluate. Default is 3.</param>
    /// <returns><c>true</c> if volatility is decreasing; otherwise, <c>false</c>.</returns>
    bool IsVolatilityDecreasing(int lookback = 3);

    /// <summary>
    /// Determines whether volatility has been increasing over the specified lookback period.
    /// </summary>
    /// <param name="lookback">The number of periods to evaluate. Default is 3.</param>
    /// <returns><c>true</c> if volatility is increasing; otherwise, <c>false</c>.</returns>
    bool IsVolatilityIncreasing(int lookback = 3);
}

/// <summary>
/// Defines the contract for an Average Directional Index (ADX) indicator.
/// </summary>
public interface IIndicatorADX : IIndicator, IIndicatorMethodCommon
{
    /// <summary>
    /// Gets the ADX value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The ADX value.</returns>
    IndicatorValue GetAdx(int index = 0);

    /// <summary>
    /// Gets a range of ADX values.
    /// </summary>
    /// <param name="count">The number of values to retrieve. Default is 1.</param>
    /// <returns>A collection of ADX values.</returns>
    IEnumerable<IndicatorValue> GetAdxRange(int count = 1);

    /// <summary>
    /// Gets the -DI value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The -DI value.</returns>
    IndicatorValue GetMinusDI(int index = 0);

    /// <summary>
    /// Gets a range of -DI values.
    /// </summary>
    /// <param name="count">The number of values to retrieve. Default is 1.</param>
    /// <returns>A collection of -DI values.</returns>
    IEnumerable<IndicatorValue> GetMinusDIRange(int count = 1);

    /// <summary>
    /// Gets the +DI value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The +DI value.</returns>
    IndicatorValue GetPlusDI(int index = 0);

    /// <summary>
    /// Gets a range of +DI values.
    /// </summary>
    /// <param name="count">The number of values to retrieve. Default is 1.</param>
    /// <returns>A collection of +DI values.</returns>
    IEnumerable<IndicatorValue> GetPlusDIRange(int count = 1);

    /// <summary>
    /// Determines whether the ADX value is increasing over the specified lookback period.
    /// </summary>
    /// <param name="lookback">The number of periods to evaluate. Default is 3.</param>
    /// <returns><c>true</c> if ADX is increasing; otherwise, <c>false</c>.</returns>
    bool IsIncreasing(int lookback = 3);

    /// <summary>
    /// Determines whether the market is trending based on the ADX threshold.
    /// </summary>
    /// <param name="threshold">The ADX threshold (e.g., 25). Default is 25.</param>
    /// <returns><c>true</c> if trending; otherwise, <c>false</c>.</returns>
    bool IsTrending(double threshold = 25);
}

/// <summary>
/// Defines the contract for an ADX indicator with Wilder's smoothing (ADXW).
/// </summary>
public interface IIndicatorADXW : IIndicator, IIndicatorMethodCommon
{
    /// <summary>
    /// Gets the -DI value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The -DI value.</returns>
    IndicatorValue GetMinusDI(int index = 0);

    /// <summary>
    /// Gets a range of -DI values.
    /// </summary>
    /// <param name="count">The number of values to retrieve. Default is 1.</param>
    /// <returns>A collection of -DI values.</returns>
    IEnumerable<IndicatorValue> GetMinusDIRange(int count = 1);

    /// <summary>
    /// Gets the +DI value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The +DI value.</returns>
    IndicatorValue GetPlusDI(int index = 0);

    /// <summary>
    /// Gets a range of +DI values.
    /// </summary>
    /// <param name="count">The number of values to retrieve. Default is 1.</param>
    /// <returns>A collection of +DI values.</returns>
    IEnumerable<IndicatorValue> GetPlusDIRange(int count = 1);

    /// <summary>
    /// Determines whether the current trend is bearish.
    /// </summary>
    /// <param name="threshold">The ADX threshold for strong trend. Default is 25.</param>
    /// <returns><c>true</c> if bearish; otherwise, <c>false</c>.</returns>
    bool IsBearish(double threshold = 25);

    /// <summary>
    /// Determines if a bearish crossover (-DI crosses above +DI) has occurred.
    /// </summary>
    /// <returns><c>true</c> if a bearish crossover is detected; otherwise, <c>false</c>.</returns>
    bool IsBearishCrossover();

    /// <summary>
    /// Determines whether the current trend is bullish.
    /// </summary>
    /// <param name="threshold">The ADX threshold for strong trend. Default is 25.</param>
    /// <returns><c>true</c> if bullish; otherwise, <c>false</c>.</returns>
    bool IsBullish(double threshold = 25);

    /// <summary>
    /// Determines if a bullish crossover (+DI crosses above -DI) has occurred.
    /// </summary>
    /// <returns><c>true</c> if a bullish crossover is detected; otherwise, <c>false</c>.</returns>
    bool IsBullishCrossover();

    /// <summary>
    /// Determines whether a strong trend exists based on the ADX threshold.
    /// </summary>
    /// <param name="threshold">The ADX threshold. Default is 25.</param>
    /// <returns><c>true</c> if a strong trend is detected; otherwise, <c>false</c>.</returns>
    bool IsStrongTrend(double threshold = 25);
}
