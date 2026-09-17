using Pt.Okx.Abstractions.Indicators.Base;
using Pt.Okx.Shared.Indicators.Models;

namespace Pt.Okx.Abstractions.Indicators.BuiltIn;

/// <summary>
/// Defines the contract for an Accumulation/Distribution (AD) indicator.
/// </summary>
public interface IIndicatorAD : IIndicator, IIndicatorMethodCommon
{
}

/// <summary>
/// Defines the contract for a Volumes indicator.
/// </summary>
public interface IIndicatorVolumes : IIndicator, IIndicatorMethodCommon
{
    /// <summary>
    /// Gets the volume value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The volume value.</returns>
    IndicatorValue FindVolume(int index = 0);
}

/// <summary>
/// Defines the contract for an On-Balance Volume (OBV) indicator.
/// </summary>
public interface IIndicatorOBV : IIndicator, IIndicatorMethodCommon
{
}

/// <summary>
/// Defines the contract for a Volume Spike indicator.
/// </summary>
public interface IIndicatorVolumeSpike : IIndicator, IIndicatorMethodCommon
{
    /// <summary>
    /// Gets the volume value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The volume value.</returns>
    IndicatorValue FindVolume(int index = 0);

    /// <summary>
    /// Gets the average volume at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The average volume value.</returns>
    IndicatorValue FindAvgVolume(int index = 0);

    /// <summary>
    /// Gets the volume ratio (current volume / average volume) at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The volume ratio value.</returns>
    IndicatorValue FindVolumeRatio(int index = 0);

    /// <summary>
    /// Determines whether a volume spike occurred at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to check. Default is 0.</param>
    /// <returns><c>true</c> if a volume spike is detected; otherwise, <c>false</c>.</returns>
    bool IsSpike(int index = 0);
}

/// <summary>
/// Defines the contract for a Force Index indicator.
/// </summary>
public interface IIndicatorForce : IIndicator, IIndicatorMethodCommon
{
}

/// <summary>
/// Defines the contract for a Volume Weighted Average Price (VWAP) indicator.
/// </summary>
public interface IIndicatorVWAP : IIndicator, IIndicatorMethodCommon
{
    /// <summary>
    /// Gets the VWAP value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The VWAP value.</returns>
    IndicatorValue FindVWAP(int index = 0);

    /// <summary>
    /// Determines whether the specified price is above the VWAP.
    /// </summary>
    /// <param name="closePrice">The price to evaluate.</param>
    /// <param name="index">The zero-based index of the VWAP value. Default is 0.</param>
    /// <returns><c>true</c> if the price is above the VWAP; otherwise, <c>false</c>.</returns>
    bool IsPriceAboveVWAP(double closePrice, int index = 0);

    /// <summary>
    /// Determines whether the specified price is below the VWAP.
    /// </summary>
    /// <param name="closePrice">The price to evaluate.</param>
    /// <param name="index">The zero-based index of the VWAP value. Default is 0.</param>
    /// <returns><c>true</c> if the price is below the VWAP; otherwise, <c>false</c>.</returns>
    bool IsPriceBelowVWAP(double closePrice, int index = 0);

    /// <summary>
    /// Calculates the percentage distance between the specified price and the VWAP.
    /// </summary>
    /// <param name="closePrice">The price to evaluate.</param>
    /// <param name="index">The zero-based index of the VWAP value. Default is 0.</param>
    /// <returns>The percentage distance.</returns>
    double GetDistanceFromVWAP(double closePrice, int index = 0);

    /// <summary>
    /// Gets the cumulative volume at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The cumulative volume value.</returns>
    IndicatorValue FindCumulativeVolume(int index = 0);
}

/// <summary>
/// Defines the contract for a Chaikin Oscillator indicator.
/// </summary>
public interface IIndicatorChaikin : IIndicator, IIndicatorMethodCommon
{
}
