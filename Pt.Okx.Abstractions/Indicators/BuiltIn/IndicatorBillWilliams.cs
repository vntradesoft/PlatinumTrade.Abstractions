using Pt.Okx.Abstractions.Indicators.Base;
using Pt.Okx.Shared.Indicators.Models;

namespace Pt.Okx.Abstractions.Indicators.BuiltIn;

/// <summary>
/// Defines the contract for an Accelerator Oscillator (AC) indicator.
/// </summary>
public interface IIndicatorAC : IIndicator, IIndicatorMethodCommon
{
    /// <summary>
    /// Gets the bullish (up) histogram value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The bullish histogram value.</returns>
    IndicatorValue GetUp(int index = 0);

    /// <summary>
    /// Gets the bearish (down) histogram value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The bearish histogram value.</returns>
    IndicatorValue GetDown(int index = 0);
}

/// <summary>
/// Defines the contract for an Awesome Oscillator (AO) indicator.
/// </summary>
public interface IIndicatorAO : IIndicator, IIndicatorMethodCommon
{
    /// <summary>
    /// Gets the bullish (up) histogram value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The bullish histogram value.</returns>
    IndicatorValue GetUp(int index = 0);

    /// <summary>
    /// Gets the bearish (down) histogram value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The bearish histogram value.</returns>
    IndicatorValue GetDown(int index = 0);
}

/// <summary>
/// Defines the contract for a Williams Alligator indicator.
/// </summary>
public interface IIndicatorAlligator : IIndicator, IIndicatorMethodCommon
{
    /// <summary>
    /// Gets the Alligator's Jaw (Blue line) value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The Jaw line value.</returns>
    IndicatorValue GetJaw(int index = 0);

    /// <summary>
    /// Gets the Alligator's Teeth (Red line) value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The Teeth line value.</returns>
    IndicatorValue GetTeeth(int index = 0);

    /// <summary>
    /// Gets the Alligator's Lips (Green line) value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The Lips line value.</returns>
    IndicatorValue GetLips(int index = 0);
}

/// <summary>
/// Defines the contract for a Gator Oscillator indicator.
/// </summary>
public interface IIndicatorGator : IIndicator, IIndicatorMethodCommon
{
    /// <summary>
    /// Gets the upper histogram value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The upper histogram value.</returns>
    IndicatorValue GetUp(int index = 0);

    /// <summary>
    /// Gets the lower histogram value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The lower histogram value.</returns>
    IndicatorValue GetDown(int index = 0);
}

/// <summary>
/// Defines the contract for a Williams Fractals indicator.
/// </summary>
public interface IIndicatorFractals : IIndicator, IIndicatorMethodCommon
{
    /// <summary>
    /// Gets the up fractal value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The up fractal value.</returns>
    IndicatorValue GetUp(int index = 0);

    /// <summary>
    /// Gets the down fractal value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The down fractal value.</returns>
    IndicatorValue GetDown(int index = 0);
}

/// <summary>
/// Defines the contract for a Bill Williams Market Facilitation Index (BWMFI) indicator.
/// </summary>
public interface IIndicatorBWMFI : IIndicator, IIndicatorMethodCommon
{
    /// <summary>
    /// Gets the Green bar value (MFI+, Vol+) at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The Green bar value.</returns>
    IndicatorValue GetGreen(int index = 0);

    /// <summary>
    /// Gets the Brown bar value (MFI-, Vol-) at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The Brown bar value.</returns>
    IndicatorValue GetBrown(int index = 0);

    /// <summary>
    /// Gets the Blue bar value (MFI+, Vol-) at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The Blue bar value.</returns>
    IndicatorValue GetBlue(int index = 0);

    /// <summary>
    /// Gets the Pink bar value (MFI-, Vol+) at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the value to retrieve. Default is 0.</param>
    /// <returns>The Pink bar value.</returns>
    IndicatorValue GetPink(int index = 0);
}
