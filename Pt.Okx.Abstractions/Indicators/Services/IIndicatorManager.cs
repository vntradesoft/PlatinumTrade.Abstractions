using Pt.Okx.Abstractions.Indicators.Base;
using Pt.Okx.Shared.Indicators.Enums;
using Pt.Okx.Shared.Indicators.Models;

namespace Pt.Okx.Abstractions.Indicators.Services
{
    /// <summary>
    /// Manages the lifecycle, registration, and data access for technical indicators.
    /// </summary>
    public interface IIndicatorManager
    {
        /// <summary>
        /// Registers an indicator instance and assigns it a unique identifier.
        /// </summary>
        /// <param name="indicator">The indicator instance to register.</param>
        /// <returns>The unique identifier assigned to the indicator.</returns>
        string RegisterIndicator(IIndicator indicator);

        /// <summary>
        /// Retrieves a specific buffer for a registered indicator.
        /// </summary>
        /// <param name="indicatorId">The unique identifier of the indicator.</param>
        /// <param name="index">The zero-based index of the buffer to retrieve.</param>
        /// <returns>The indicator buffer if found; otherwise, <c>null</c>.</returns>
        IIndicatorBuffer? GetIndicatorBuffer(string indicatorId, int index);

        /// <summary>
        /// Retrieves an indicator's buffer value at a specific timestamp.
        /// </summary>
        /// <param name="indicatorId">The unique identifier of the indicator.</param>
        /// <param name="index">The zero-based index of the buffer.</param>
        /// <param name="timestamp">The timestamp for which to retrieve the value.</param>
        /// <returns>The indicator value at the specified time.</returns>
        IndicatorValue GetIndicatorBufferValueAtTime(string indicatorId, int index, DateTime timestamp);

        /// <summary>
        /// Configures a buffer for a registered indicator.
        /// </summary>
        /// <param name="indicatorId">The unique identifier of the indicator.</param>
        /// <param name="index">The zero-based index of the buffer to configure.</param>
        /// <param name="type">The type of the buffer (e.g., Data, Calculations).</param>
        /// <param name="allowEmptySlots">Whether the buffer allows empty slots. Default is <c>false</c>.</param>
        void SetIndexBuffer(string indicatorId, int index, IndicatorBufferType type, bool allowEmptySlots = false);

#pragma warning disable CA1716 // Identifiers should not match keywords
        /// <summary>
        /// Retrieves a typed indicator instance by its alias.
        /// </summary>
        /// <typeparam name="TIndicator">The type of the indicator.</typeparam>
        /// <param name="alias">The alias assigned to the indicator.</param>
        /// <returns>The typed indicator instance if found; otherwise, <c>null</c>.</returns>
        TIndicator? GetIndicatorByAlias<TIndicator>(string alias) where TIndicator : class, IIndicator;
#pragma warning restore CA1716 // Identifiers should not match keywords

        /// <summary>
        /// Gets a list of all registered indicators.
        /// </summary>
        /// <returns>A list of registered indicators.</returns>
        IReadOnlyList<IIndicator> GetIndicators();

        /// <summary>
        /// Retrieves a typed indicator instance by its unique identifier.
        /// </summary>
        /// <typeparam name="TIndicator">The type of the indicator.</typeparam>
        /// <param name="indicatorId">The unique identifier of the indicator.</param>
        /// <returns>The typed indicator instance if found; otherwise, <c>null</c>.</returns>
        TIndicator? GetIndicator<TIndicator>(string indicatorId) where TIndicator : class, IIndicator;

        /// <summary>
        /// Retrieves an indicator instance by its unique identifier.
        /// </summary>
        /// <param name="indicatorId">The unique identifier of the indicator.</param>
        /// <returns>The indicator instance if found; otherwise, <c>null</c>.</returns>
        IIndicator? GetIndicator(string indicatorId);

        /// <summary>
        /// Unregisters an indicator and cleans up its associated resources.
        /// </summary>
        /// <param name="uniqueId">The unique identifier of the indicator to unregister.</param>
        void UnregisterIndicator(string uniqueId);

        /// <summary>
        /// Clears all registered indicators.
        /// </summary>
        void ClearAllIndicators();
    }
}
