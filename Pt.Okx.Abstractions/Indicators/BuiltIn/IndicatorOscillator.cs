using Pt.Okx.Abstractions.Indicators.Base;
using Pt.Okx.Shared.Indicators.Models;

namespace Pt.Okx.Abstractions.Indicators.BuiltIn;

/// <summary>
/// Defines the contract for a Relative Strength Index (RSI) indicator.
/// </summary>
public interface IIndicatorRSI : IIndicator, IIndicatorMethodCommon
{
    /// <summary>
    /// Determines whether the RSI value at the specified index is in the overbought zone.
    /// </summary>
    bool IsOverbought(double threshold = 70, int index = 0);

    /// <summary>
    /// Determines whether the RSI value at the specified index is in the oversold zone.
    /// </summary>
    bool IsOversold(double threshold = 30, int index = 0);
}

/// <summary>
/// Defines the contract for a Stochastic Oscillator indicator.
/// </summary>
public interface IIndicatorStochastic : IIndicator, IIndicatorMethodCommon
{
    /// <summary>
    /// Gets the %K line value at the specified index.
    /// </summary>
    IndicatorValue GetK(int index = 0);

    /// <summary>
    /// Gets the %D line value at the specified index.
    /// </summary>
    IndicatorValue GetD(int index = 0);

    /// <summary>
    /// Determines whether the Stochastic %K value at the specified index is in the overbought zone.
    /// </summary>
    bool IsOverbought(double threshold = 80, int index = 0);

    /// <summary>
    /// Determines whether the Stochastic %K value at the specified index is in the oversold zone.
    /// </summary>
    bool IsOversold(double threshold = 20, int index = 0);
}

/// <summary>
/// Defines the contract for a Moving Average Convergence Divergence (MACD) indicator.
/// </summary>
public interface IIndicatorMACD : IIndicator, IIndicatorMethodCommon
{
    /// <summary>
    /// Gets the MACD line value at the specified index.
    /// </summary>
    IndicatorValue GetMacd(int index = 0);

    /// <summary>
    /// Gets the Signal line value at the specified index.
    /// </summary>
    IndicatorValue GetSignal(int index = 0);

    /// <summary>
    /// Gets the Histogram value at the specified index.
    /// </summary>
    IndicatorValue GetHistogram(int index = 0);
}

/// <summary>
/// Defines the contract for an Moving Average of Oscillator (OsMA) indicator.
/// </summary>
public interface IIndicatorOsMA : IIndicator, IIndicatorMethodCommon
{
}

/// <summary>
/// Defines the contract for a Commodity Channel Index (CCI) indicator.
/// </summary>
public interface IIndicatorCCI : IIndicator, IIndicatorMethodCommon
{
    /// <summary>
    /// Determines whether the CCI value at the specified index is in the overbought zone.
    /// </summary>
    bool IsOverbought(double threshold = 100, int index = 0);

    /// <summary>
    /// Determines whether the CCI value at the specified index is in the oversold zone.
    /// </summary>
    bool IsOversold(double threshold = -100, int index = 0);
}

/// <summary>
/// Defines the contract for a Momentum indicator.
/// </summary>
public interface IIndicatorMomentum : IIndicator, IIndicatorMethodCommon
{
    /// <summary>
    /// Gets the momentum value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The momentum value.</returns>
    IndicatorValue FindMomentum(int index = 0);
}

/// <summary>
/// Defines the contract for a Money Flow Index (MFI) indicator.
/// </summary>
public interface IIndicatorMFI : IIndicator, IIndicatorMethodCommon
{
    /// <summary>
    /// Determines whether the MFI value at the specified index is in the overbought zone.
    /// </summary>
    bool IsOverbought(double threshold = 80, int index = 0);

    /// <summary>
    /// Determines whether the MFI value at the specified index is in the oversold zone.
    /// </summary>
    bool IsOversold(double threshold = 20, int index = 0);
}

/// <summary>
/// Defines the contract for a Relative Vigor Index (RVI) indicator.
/// </summary>
public interface IIndicatorRVI : IIndicator, IIndicatorMethodCommon
{
}

/// <summary>
/// Defines the contract for a Bears Power indicator.
/// </summary>
public interface IIndicatorBears : IIndicator, IIndicatorMethodCommon
{
}

/// <summary>
/// Defines the contract for a Bulls Power indicator.
/// </summary>
public interface IIndicatorBulls : IIndicator, IIndicatorMethodCommon
{
}

/// <summary>
/// Defines the contract for a Williams' Percent Range (WPR) indicator.
/// </summary>
public interface IIndicatorWPR : IIndicator, IIndicatorMethodCommon
{
    /// <summary>
    /// Determines whether the WPR value at the specified index is in the overbought zone.
    /// </summary>
    bool IsOverbought(double threshold = -20, int index = 0);

    /// <summary>
    /// Determines whether the WPR value at the specified index is in the oversold zone.
    /// </summary>
    bool IsOversold(double threshold = -80, int index = 0);
}

/// <summary>
/// Defines the contract for a DeMarker indicator.
/// </summary>
public interface IIndicatorDeMarker : IIndicator, IIndicatorMethodCommon
{
    /// <summary>
    /// Determines whether the DeMarker value at the specified index is in the overbought zone.
    /// </summary>
    bool IsOverbought(double threshold = 0.7, int index = 0);

    /// <summary>
    /// Determines whether the DeMarker value at the specified index is in the oversold zone.
    /// </summary>
    bool IsOversold(double threshold = 0.3, int index = 0);
}

/// <summary>
/// Defines the contract for a Triple Exponential Average (TRIX) indicator.
/// </summary>
public interface IIndicatorTRIX : IIndicator, IIndicatorMethodCommon
{
}

/// <summary>
/// Defines the contract for a Standard Deviation indicator.
/// </summary>
public interface IIndicatorStdDev : IIndicator, IIndicatorMethodCommon
{
}

/// <summary>
/// Defines the contract for an Average True Range (ATR) indicator.
/// </summary>
public interface IIndicatorATR : IIndicator, IIndicatorMethodCommon
{
}

/// <summary>
/// Defines the contract for a Bollinger Bands %B indicator.
/// </summary>
public interface IIndicatorBollingerPercentB : IIndicator, IIndicatorMethodCommon
{
    /// <summary>
    /// Gets the %B value at the specified index.
    /// </summary>
    IndicatorValue GetPercentB(int index = 0);

    /// <summary>
    /// Determines whether %B is above the overbought threshold.
    /// </summary>
    bool IsOverbought(double threshold = 1.0, int index = 0);

    /// <summary>
    /// Determines whether %B is below the oversold threshold.
    /// </summary>
    bool IsOversold(double threshold = 0.0, int index = 0);
}

/// <summary>
/// Defines the contract for a Bollinger Band Width indicator.
/// </summary>
public interface IIndicatorBollingerBandWidth : IIndicator, IIndicatorMethodCommon
{
    /// <summary>
    /// Gets the band width value at the specified index.
    /// </summary>
    IndicatorValue GetWidth(int index = 0);

    /// <summary>
    /// Determines whether width is below a squeeze threshold.
    /// </summary>
    bool IsSqueeze(double threshold = 1.5, int index = 0);

    /// <summary>
    /// Determines whether width is above an expansion threshold.
    /// </summary>
    bool IsExpansion(double threshold = 2.5, int index = 0);
}
