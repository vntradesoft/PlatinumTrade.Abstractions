using Pt.Okx.Shared.Indicators.Models;
using Pt.Okx.Abstractions.Indicators.Services;

namespace Pt.Okx.Abstractions.Indicators.Base
{
    /// <summary>
    /// Delegate for creating an indicator instance.
    /// </summary>
    /// <param name="factory">The indicator factory.</param>
    /// <param name="manager">The indicator manager.</param>
    /// <param name="config">The indicator configuration.</param>
    /// <param name="propertyOptions">Optional property configuration.</param>
    /// <returns>A new indicator instance.</returns>
    public delegate IIndicator IndicatorCreator(IIndicatorFactory factory, IIndicatorManager manager, IndicatorConfig config, Action<IndicatorProperty>? propertyOptions);

    /// <summary>
    /// Represents a technical indicator that can be calculated over a series of price data.
    /// </summary>
    public interface IIndicator
    {
        /// <summary>
        /// Gets the configuration for the indicator.
        /// </summary>
        IndicatorConfig Config { get; }

        /// <summary>
        /// Gets the unique identity of the indicator instance.
        /// </summary>
        IndicatorIdentity Identity { get; }

#pragma warning disable CA1716
        /// <summary>
        /// Gets or sets the indicator properties, such as display name and buffer metadata.
        /// </summary>
        IndicatorProperty Property { get; set; }
#pragma warning restore CA1716

        /// <summary>
        /// Gets a value indicating whether the indicator has been initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Initializes the indicator. Called once before the first calculation.
        /// </summary>
        /// <returns><c>true</c> if initialization was successful; otherwise, <c>false</c>.</returns>
        bool OnInit();

        /// <summary>
        /// Calculates the indicator values for a single price series.
        /// </summary>
        /// <param name="ratesTotal">The total number of price data points.</param>
        /// <param name="prevCalculated">The number of points calculated in the previous call.</param>
        /// <param name="begin">The starting index in the price series.</param>
        /// <param name="prices">The price data series.</param>
        /// <returns>The number of calculated elements.</returns>
        int OnCalculate(in int ratesTotal, in int prevCalculated, in int begin, in double[] prices);

        /// <summary>
        /// Calculates the indicator values for a complete OHLCV data set.
        /// </summary>
        /// <param name="ratesTotal">The total number of price data points.</param>
        /// <param name="prevCalculated">The number of points calculated in the previous call.</param>
        /// <param name="datetime">The timestamps for each data point.</param>
        /// <param name="opens">The opening prices.</param>
        /// <param name="highs">The high prices.</param>
        /// <param name="lows">The low prices.</param>
        /// <param name="closes">The closing prices.</param>
        /// <param name="volumes">The trading volumes.</param>
        /// <param name="spreads">The spreads (if available).</param>
        /// <returns>The number of calculated elements.</returns>
        int OnCalculate(in int ratesTotal, in int prevCalculated,
            in DateTime[] datetime, in double[] opens, in double[] highs, in double[] lows,
            in double[] closes, in double[] volumes, in double spreads);

        /// <summary>
        /// Gets the unique identifier for the indicator instance.
        /// </summary>
        /// <returns>The indicator ID.</returns>
        string GetIndicatorId();

        /// <summary>
        /// Gets the display name for the indicator.
        /// </summary>
        /// <returns>The display name.</returns>
        string GetDisplayName();

        /// <summary>
        /// Gets an indicator buffer by its index.
        /// </summary>
        /// <param name="index">The index of the buffer to retrieve.</param>
        /// <returns>The indicator buffer at the specified index.</returns>
        IIndicatorBuffer GetBuffer(int index);
    }
}
