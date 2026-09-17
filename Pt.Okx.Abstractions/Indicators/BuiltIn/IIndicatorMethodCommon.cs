using Pt.Okx.Shared.Indicators.Models;

namespace Pt.Okx.Abstractions.Indicators.BuiltIn;

/// <summary>
/// Provides common access methods for retrieving indicator values
/// from one or more internal buffers.
/// </summary>
public interface IIndicatorMethodCommon
{
    /// <summary>
    /// Gets the indicator value at the specified index from the default buffer (buffer index 0).
    /// </summary>
    /// <param name="index">
    /// The zero-based index of the value to retrieve. Default is <c>0</c>.
    /// </param>
    /// <returns>
    /// The indicator value at the specified index from the default buffer.
    /// </returns>
    IndicatorValue GetAt(int index = 0);

    /// <summary>
    /// Gets the indicator value at the specified index from a specific buffer.
    /// </summary>
    /// <param name="index">
    /// The zero-based index of the value to retrieve.
    /// </param>
    /// <param name="bufferIndex">
    /// The zero-based index of the buffer to retrieve the value from.
    /// </param>
    /// <returns>
    /// The indicator value at the specified index and buffer.
    /// </returns>
    IndicatorValue GetAt(int index, int bufferIndex);

    /// <summary>
    /// Gets a sequence of indicator values from the default buffer.
    /// </summary>
    /// <param name="count">
    /// The number of values to retrieve. Default is <c>1</c>.
    /// </param>
    /// <returns>
    /// A sequence of indicator values starting from the latest position.
    /// </returns>
    IEnumerable<IndicatorValue> GetRange(int count = 1);

    /// <summary>
    /// Gets a sequence of indicator values from a specific buffer.
    /// </summary>
    /// <param name="count">
    /// The number of values to retrieve.
    /// </param>
    /// <param name="bufferIndex">
    /// The zero-based index of the buffer to retrieve values from.
    /// </param>
    /// <returns>
    /// A sequence of indicator values from the specified buffer.
    /// </returns>
    IEnumerable<IndicatorValue> GetRange(int count, int bufferIndex);
}
