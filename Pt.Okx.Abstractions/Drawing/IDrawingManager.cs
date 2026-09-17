using Pt.Okx.Shared.Drawing.Enums;
using Pt.Okx.Shared.Drawing.Events;
using Pt.Okx.Shared.Drawing.Models;
using Pt.Okx.Shared.Drawing.Objects;
using Pt.Okx.Shared.Enums;

namespace Pt.Okx.Abstractions.Drawing;

/// <summary>
/// Defines the contract for managing drawing objects (lines, shapes, text, etc.) within the trading workspace.
/// </summary>
public interface IDrawingManager
{
    // CRUD
    /// <summary>
    /// Adds a new drawing object to the manager.
    /// </summary>
    /// <param name="obj">The drawing object to add.</param>
    /// <returns>The unique identifier of the added object.</returns>
    string Add(DrawingObject obj);

    /// <summary>
    /// Removes a drawing object by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the object to remove.</param>
    void Remove(string id);

    /// <summary>
    /// Updates an existing drawing object by applying a mutation action.
    /// </summary>
    /// <param name="id">The unique identifier of the object to update.</param>
    /// <param name="mutate">An action to modify the drawing object.</param>
    void Update(string id, Action<DrawingObject> mutate);

    /// <summary>
    /// Clears all drawing objects, optionally filtered by symbol.
    /// </summary>
    /// <param name="symbol">Optional symbol to filter objects for clearing. If null, all objects are cleared.</param>
    void Clear(string? symbol = null);

    // Query
    /// <summary>
    /// Retrieves all drawing objects for a specific symbol and timeframe.
    /// </summary>
    /// <param name="symbol">The trading symbol.</param>
    /// <param name="tf">The timeframe.</param>
    /// <returns>A read-only list of drawing objects.</returns>
    IReadOnlyList<DrawingObject> GetAll(string symbol, Timeframe tf);

    /// <summary>
    /// Retrieves a specific drawing object by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the object.</param>
    /// <returns>The drawing object if found; otherwise, <c>null</c>.</returns>
    DrawingObject? GetById(string id);

    // Events - UI subscribes, CLI ignores
    /// <summary>
    /// Event triggered whenever a drawing object is added, updated, or removed.
    /// </summary>
    event EventHandler<DrawingChangeEventArgs>? DrawingChanged;

    // Convenience factory methods for the strategy API
    /// <summary>
    /// Creates and adds a horizontal line drawing object.
    /// </summary>
    string AddHorizontalLine(string symbol, Timeframe tf, decimal price, DrawingStyle? style = null, DrawingSource source = DrawingSource.Strategy, string? indicatorId = null);

    /// <summary>
    /// Creates and adds a trend line drawing object.
    /// </summary>
    string AddTrendLine(string symbol, Timeframe tf, DrawingAnchor startAnchor, DrawingAnchor endAnchor, DrawingStyle? style = null, string? indicatorId = null);

    /// <summary>
    /// Creates and adds a rectangle drawing object.
    /// </summary>
    string AddRectangle(string symbol, Timeframe tf, DrawingAnchor topLeft, DrawingAnchor bottomRight, DrawingStyle? style = null, string? indicatorId = null);

    /// <summary>
    /// Creates and adds a text drawing object.
    /// </summary>
    string AddText(string symbol, Timeframe tf, DrawingAnchor anchor, string text, DrawingStyle? style = null, string? indicatorId = null);

    /// <summary>
    /// Creates and adds an emoji drawing object.
    /// </summary>
    string AddEmoji(string symbol, Timeframe tf, DrawingAnchor anchor, string emoji, DrawingStyle? style = null, string? indicatorId = null);

    /// <summary>
    /// Creates and adds a measurement tool drawing object.
    /// </summary>
    string AddMeasurement(string symbol, Timeframe tf, DrawingAnchor startAnchor, DrawingAnchor endAnchor, string? indicatorId = null);
}
