using Pt.Okx.Shared.Indicators.Models;

namespace Pt.Okx.Abstractions.Indicators.Base
{

    /// <summary>
    /// Represents a buffer for storing indicator values over time.
    /// </summary>
    public interface IIndicatorBuffer
    {
        /// <summary>
        /// Actual number of elements with data in the buffer.
        /// (does not include empty parts)
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Total number of elements from the beginning to the end of the buffer that have been filled with data.
        /// Different from Count: Count only calculates the current number of elements,
        /// while FullCount includes the offset (_startIndex + Count).
        /// </summary>
        int FullCount { get; }

        /// <summary>
        /// The buffer's maximum capacity.
        /// When the number of elements exceeds, the buffer may need to resize.
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// Indicator's current time based on the period (only applicable for backtest).
        /// This value will be updated every time new data is available.
        /// </summary>
        DateTime CurrentTime { get; set; }

        /// <summary>
        /// Returns all indicator values (original array)
        /// </summary>
        IReadOnlyList<IndicatorValue> InternalValues { get; }

        /// <summary>
        /// Returns the version of the buffer, which increments whenever data is added or modified.
        /// </summary>
        long Version { get; }

        /// <summary>
        /// Accesses elements by index.
        /// Equivalent to calling <see cref="At(int)"/>.
        /// </summary>
        IndicatorValue this[int index] { get; }

        /// <summary>
        /// Returns the value at the specified index.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Index out of range</exception>
        IndicatorValue At(int index);

        /// <summary>
        /// Finds and returns the value corresponding to <see cref="CurrentTime"/>.
        /// (Usually used to get data at the current time in trading)
        /// </summary>
        IndicatorValue FindCurrent();

        /// <summary>
        /// Returns the indicator value at or before the time <see cref="CurrentTime"/>.
        /// </summary>
        /// <param name="offset">
        /// Number of steps back from the found position.
        /// </param>
        IndicatorValue FindAtOrBeforeCurrent(int offset = 0);

        /// <summary>
        /// Adds a new value to the buffer at a specified position.
        /// Requires data to be added in ascending chronological order.
        /// </summary>
        void Add(int index, IndicatorValue value);

        /// <summary>
        /// Finds and returns a collection of n elements immediately after a given time.
        /// </summary>
        IEnumerable<IndicatorValue> FindAfter(DateTime dateTime, int count);

        /// <summary>
        /// Finds and returns all elements within a specified time range.
        /// Complexity: O(log n + k), where n is the number of elements and k is the number of satisfied elements.
        /// </summary>
        IEnumerable<IndicatorValue> FindRange(DateTime startTime, DateTime endTime);

        /// <summary>
        /// Returns a <see cref="Span{IndicatorValue}"/> representing a slice in the buffer.
        /// This function is very fast because it only creates a view on the original array, not copying data.
        /// </summary>
        Span<IndicatorValue> GetSlice(int startIndex, int length);

        /// <summary>
        /// Returns the value at or immediately before dateTime. 
        /// If not found, returns the default value.
        /// </summary>
        IndicatorValue FindAtOrBefore(DateTime dateTime);

        /// <summary>
        /// Returns the index of the element with a time greater than or equal to the specified time dateTime.
        /// Uses BinarySearch for fast O(log n) searching.
        /// </summary>
        int FindIndexAtOrAfter(DateTime dateTime);

        /// <summary>
        /// Returns the index of the element with a time less than or equal to dateTime.
        /// </summary>
        int FindIndexAtOrBefore(DateTime dateTime);

        /// <summary>
        /// Adds a new value to the buffer at a specified position, with timestamp and numeric value.
        /// </summary>
        void Add(int index, DateTime timestamp, double value);

        /// <summary>
        /// Overwrites the value at the index regardless of whether the slot already has data or not.
        /// Used for multi-buffer indicators (SAR, etc.) when 2 buffers track the same index.
        /// </summary>
        void ForceAdd(int index, DateTime timestamp, double value);

        /// <summary>
        /// Records an index position as processed but without a value (used for multi-buffer indicators like SAR).
        /// Does not increase Count, only updates FullCount so At()/TryAt() knows the index has been visited.
        /// </summary>
        void MarkEmpty(int index, DateTime timestamp);

        /// <summary>
        /// Returns all indicator values.
        /// </summary>
        IndicatorValue[] GetValues();

        /// <summary>
        /// Returns the value at the specified index, or null if the index is invalid.
        /// </summary>
        IndicatorValue TryAt(int index);

        /// <summary>
        /// Searches for the index of the element with a time exactly equal to dateTime.
        /// Uses BinarySearch with O(log n) complexity.
        /// </summary>
        int FindIndex(DateTime dateTime);

        /// <summary>
        /// Finds and returns the value at the exact specified time.
        /// </summary>
        IndicatorValue Find(DateTime dateTime);

        /// <summary>
        /// Finds and returns the value closest to the time passed in dateTime.
        /// </summary>
        IndicatorValue FindClosest(DateTime dateTime);

        /// <summary>
        /// Finds and returns n elements before (or at) a given time.
        /// (Usually used to retrieve historical data before a time point in trading)
        /// </summary>
        IEnumerable<IndicatorValue> FindBefore(DateTime dateTime, int count);

        /// <summary>
        /// Finds and returns n elements before (or at) the current time.
        /// (Usually used to retrieve the most recent historical data compared to the current time in trading)
        /// </summary>
        IEnumerable<IndicatorValue> FindBefore(int count);

        /// <summary>
        /// Returns the value at CurrentTime, including IsEmpty (no walk-back).
        /// Used to check the exact state at the current bar.
        /// </summary>
        IndicatorValue FindExactAtCurrent();

        /// <summary>
        /// Finds the value closest to a given time, but only returns it if the time difference does not exceed tolerance.
        /// </summary>
        IndicatorValue FindWithTolerance(DateTime dateTime, TimeSpan tolerance);

        /// <summary>
        /// Retrieves the last n elements (latest values) in the buffer.
        /// Returns as Span{T} for fast access without copying data.
        /// </summary>
        Span<IndicatorValue> GetLatest(int count);

        /// <summary>
        /// Returns the number of elements with data up to CurrentTime.
        /// Used instead of Count when counting in backtest context.
        /// </summary>
        int CountUpToCurrent();

        /// <summary>
        /// Returns the number of bars that have been calculated up to CurrentTime.
        /// This counts processed bar slots (including slots marked empty) for indicator progress tracking.
        /// </summary>
        int GetCalculatedBarCount();

        /// <summary>
        /// Checks if the buffer has data at or before CurrentTime.
        /// Used to guard before querying.
        /// </summary>
        bool HasDataAtCurrent();

        /// <summary>
        /// Returns the range [startTime, CurrentTime] — never exceeds CurrentTime.
        /// </summary>
        IEnumerable<IndicatorValue> FindRangeUpToCurrent(DateTime startTime);

        /// <summary>
        /// Changes the buffer size. Only performed when newSize is greater than current Capacity.
        /// </summary>
        bool Resize(int newSize);

        /// <summary>
        /// Clears all data.
        /// </summary>
        void Clear();

        /// <summary>
        /// Returns basic buffer statistics.
        /// </summary>
        BufferStats GetStats();

        /// <summary>
        /// Returns the current end index.
        /// </summary>
        int GetCurrentEndIndexDebug();
    }

    /// <summary>
    /// Represents statistics of a buffer, including capacity, usage,
    /// memory footprint, and the time range of stored data.
    /// </summary>
    /// <param name="TotalSize">The total capacity of the buffer (number of slots).</param>
    /// <param name="UsedSlots">The number of currently used slots.</param>
    /// <param name="MemoryUsageKB">The estimated memory usage in kilobytes.</param>
    /// <param name="FirstDateTime">The earliest timestamp in the buffer.</param>
    /// <param name="LastDateTime">The latest timestamp in the buffer.</param>
    public readonly record struct BufferStats(
        int TotalSize,
        int UsedSlots,
        int MemoryUsageKB,
        DateTime FirstDateTime,
        DateTime LastDateTime)
    {
        /// <summary>
        /// Gets the percentage of used slots in the buffer.
        /// </summary>
        public double UsagePercent =>
            TotalSize > 0 ? (UsedSlots * 100.0 / TotalSize) : 0;

        /// <summary>
        /// Returns a human-readable summary of the buffer statistics.
        /// </summary>
        /// <returns>
        /// A formatted string describing usage, memory consumption, and time range.
        /// </returns>
        public override string ToString()
        {
            return $"Used {UsedSlots}/{TotalSize} slots " +
                   $"({UsagePercent:F1}%), " +
                   $"Memory ≈ {MemoryUsageKB} KB, " +
                   $"Range: {FirstDateTime.ToLocalTime():yyyy-MM-dd HH:mm:ss} → {LastDateTime.ToLocalTime():yyyy-MM-dd HH:mm:ss}";
        }
    }
}
