using Pt.Okx.Shared.Drawing.Enums;
using Pt.Okx.Shared.Drawing.Events;
using Pt.Okx.Shared.Drawing.Models;
using Pt.Okx.Shared.Drawing.Objects;
using Pt.Okx.Shared.Enums;

namespace Pt.Okx.Abstractions.Drawing;

/// <summary>
/// A null implementation of <see cref="IDrawingManager"/> for CLI use where drawing functionality is not required (no-op).
/// </summary>
public sealed class NullDrawingManager : IDrawingManager
{
    /// <summary>Gets the singleton instance of <see cref="NullDrawingManager"/>.</summary>
    public static readonly NullDrawingManager Instance = new();

    /// <inheritdoc/>
#pragma warning disable CS0067
    public event EventHandler<DrawingChangeEventArgs>? DrawingChanged;
#pragma warning restore CS0067

    /// <inheritdoc/>
    public string Add(DrawingObject obj)
    {
        ArgumentException.ThrowIfNullOrEmpty(obj?.Id, nameof(obj.Id));
        return obj.Id;
    }
    /// <inheritdoc/>
    public void Remove(string id) { }
    /// <inheritdoc/>
    public void Update(string id, Action<DrawingObject> mutate) { }
    /// <inheritdoc/>
    public void Clear(string? symbol = null) { }
    /// <inheritdoc/>
    public IReadOnlyList<DrawingObject> GetAll(string symbol, Timeframe tf) => Array.Empty<DrawingObject>();
    /// <inheritdoc/>
    public DrawingObject? GetById(string id) => null;
    /// <inheritdoc/>
    public string AddHorizontalLine(string symbol, Timeframe tf, decimal price, DrawingStyle? style = null, DrawingSource source = DrawingSource.Strategy, string? indicatorId = null) => string.Empty;
    /// <inheritdoc/>
    public string AddTrendLine(string symbol, Timeframe tf, DrawingAnchor startAnchor, DrawingAnchor endAnchor, DrawingStyle? style = null, string? indicatorId = null) => string.Empty;
    /// <inheritdoc/>
    public string AddRectangle(string symbol, Timeframe tf, DrawingAnchor topLeft, DrawingAnchor bottomRight, DrawingStyle? style = null, string? indicatorId = null) => string.Empty;
    /// <inheritdoc/>
    public string AddText(string symbol, Timeframe tf, DrawingAnchor anchor, string text, DrawingStyle? style = null, string? indicatorId = null) => string.Empty;
    /// <inheritdoc/>
    public string AddEmoji(string symbol, Timeframe tf, DrawingAnchor anchor, string emoji, DrawingStyle? style = null, string? indicatorId = null) => string.Empty;
    /// <inheritdoc/>
    public string AddMeasurement(string symbol, Timeframe tf, DrawingAnchor startAnchor, DrawingAnchor endAnchor, string? indicatorId = null) => string.Empty;
}
