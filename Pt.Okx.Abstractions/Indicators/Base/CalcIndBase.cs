using Pt.Okx.Abstractions.Indicators.BuiltIn;
using Pt.Okx.Shared.Indicators.Enums;
using Pt.Okx.Shared.Indicators.Models;
using Pt.Okx.Abstractions.Indicators.Services;

namespace Pt.Okx.Abstractions.Indicators.Base
{
    /// <summary>
    /// Base class for indicator calculators. It manages indicator identity, configuration, and buffers.
    /// </summary>
    public abstract class CalcIndBase : IIndicator, IIndicatorMethodCommon
    {
        private string _uniqueId = string.Empty;

        /// <inheritdoc/>
        public bool IsInitialized { get; private set; }

        /// <inheritdoc/>
        public IndicatorConfig Config { get; private set; }


        /// <summary>
        /// Factory used to create child indicators during calculation.
        /// This keeps calculators decoupled from concrete manager implementations and helps unit tests.
        /// </summary>
        protected readonly IIndicatorFactory _factory;

        /// <summary>
        /// Manager that provides buffers and registers indicator identities.
        /// </summary>
        protected readonly IIndicatorManager _manager;

        /// <inheritdoc/>
        public IndicatorIdentity Identity { get; private set; }

        /// <inheritdoc/>
        public IndicatorProperty Property { get; set; } = null!;

        /// <summary>
        /// Initializes the base calculator with manager, configuration, and optional property customization.
        /// </summary>
        /// <param name="factory">Factory used to create child indicators.</param>
        /// <param name="manager">Manager responsible for indicator buffers and registration.</param>
        /// <param name="config">Input configuration for the indicator.</param>
        /// <param name="propertyOptions">Optional customization applied after default properties are created.</param>
        protected CalcIndBase(IIndicatorFactory factory, IIndicatorManager manager, IndicatorConfig config, Action<IndicatorProperty>? propertyOptions = null)
        {

            ArgumentNullException.ThrowIfNull(factory);
            ArgumentNullException.ThrowIfNull(manager);
            ArgumentNullException.ThrowIfNull(config);

            _factory = factory;
            _manager = manager;
            Config = config;
            Identity = new IndicatorIdentity
            {
                Symbol = config.Symbol,
                TimeFrame = config.TimeFrame,
                IndicatorType = config.IndicatorType,
                Parameters = config.Parameters,
            };

#pragma warning disable CA2214 // Do not call overridable methods in constructors
            Property = CreateDefaultProperty();
#pragma warning restore CA2214

            propertyOptions?.Invoke(Property);
        }

        /// <summary>
        /// Creates default display metadata for the indicator.
        /// </summary>
        protected abstract IndicatorProperty CreateDefaultProperty();

        /// <summary>
        /// Sets an indicator parameter with a fallback default value.
        /// </summary>
        /// <typeparam name="TValue">Parameter value type.</typeparam>
        /// <param name="key">Parameter key.</param>
        /// <param name="value">Requested value.</param>
        /// <param name="defaultValue">Default value used when <paramref name="value"/> is null.</param>
        protected void SetParameter<TValue>(string key, TValue? value, TValue defaultValue)
        {
            Identity.Parameters.Set(key, value, defaultValue);
        }

        /// <summary>
        /// Gets a typed indicator parameter by key.
        /// </summary>
        /// <typeparam name="TValue">Parameter value type.</typeparam>
        /// <param name="key">Parameter key.</param>
        /// <returns>The parameter value, or the type default when it is not present.</returns>
        protected TValue? GetParameter<TValue>(string key)
        {
            return Identity.Parameters.Get<TValue>(key);
        }

        /// <inheritdoc/>
        public virtual bool OnInit()
        {
            if (IsInitialized)
                return true;

            _uniqueId = _manager.RegisterIndicator(this);
            IsInitialized = !string.IsNullOrEmpty(_uniqueId);
            return IsInitialized;
        }

        /// <inheritdoc/>
        public virtual int OnCalculate(in int ratesTotal, in int prevCalculated, in int begin, in double[] prices)
        {
            return 0;
        }

        /// <summary>
        /// Resolves an indicator source as an <see cref="IndicatorValue"/> while preserving empty/missing state.
        /// </summary>
        protected IndicatorValue GetSourceValue(
            int sourceIndex,
            int dataIndex,
            DateTime timestamp,
            double[] opens,
            double[] highs,
            double[] lows,
            double[] closes)
        {
            return TryGetSourceValue(sourceIndex, dataIndex, timestamp, opens, highs, lows, closes, out var value)
                ? value
                : IndicatorValue.Empty.WithTimestamp(timestamp);
        }

        /// <summary>
        /// Attempts to resolve an indicator source from candle data or a parent indicator buffer.
        /// </summary>
        protected bool TryGetSourceValue(
            int sourceIndex,
            int dataIndex,
            DateTime timestamp,
            double[] opens,
            double[] highs,
            double[] lows,
            double[] closes,
            out IndicatorValue value)
        {

            ArgumentNullException.ThrowIfNull(opens);
            ArgumentNullException.ThrowIfNull(highs);
            ArgumentNullException.ThrowIfNull(lows);
            ArgumentNullException.ThrowIfNull(closes);

            value = IndicatorValue.Empty.WithTimestamp(timestamp);

            if (dataIndex < 0 || dataIndex >= closes.Length)
                return false;

            var hasExplicitSource = sourceIndex >= 0 && sourceIndex < Config.Sources.Count;
            if (!hasExplicitSource && !(sourceIndex == 0 && Config.Sources.Count == 0))
                return false;

            var src = hasExplicitSource ? Config.Sources[sourceIndex] : null;
            if (src == null || src.IndicatorId == null)
            {
                var appliedPrice = src?.AppliedPrice ?? GetFallbackAppliedPrice();
                value = new IndicatorValue(timestamp, GetAppliedPriceValue(appliedPrice, dataIndex, opens, highs, lows, closes));
                return true;
            }

            try
            {
                if (src.TimeFrame == null || src.TimeFrame == Config.TimeFrame)
                {
                    var buffer = _manager.GetIndicatorBuffer(src.IndicatorId, src.BufferIndex);
                    if (buffer == null || dataIndex >= buffer.FullCount)
                        return false;

                    value = buffer.TryAt(dataIndex);
                    if (value.IsEmpty)
                    {
                        value = IndicatorValue.Empty.WithTimestamp(timestamp);
                        return false;
                    }
                    return true;
                }

                value = _manager.GetIndicatorBufferValueAtTime(src.IndicatorId, src.BufferIndex, timestamp);
                if (value.IsEmpty)
                {
                    value = IndicatorValue.Empty.WithTimestamp(timestamp);
                    return false;
                }
                return true;
            }
            catch (InvalidOperationException)
            {
                value = IndicatorValue.Empty.WithTimestamp(timestamp);
                return false;
            }
        }

        private AppliedPrice GetFallbackAppliedPrice()
        {
            if (Config.Parameters.Contains("AppliedPrice"))
                return GetParameter<AppliedPrice>("AppliedPrice");

            if (Config.Parameters.Contains("ApplyPrice"))
                return GetParameter<AppliedPrice>("ApplyPrice");

            return AppliedPrice.Close;
        }

        private static double GetAppliedPriceValue(AppliedPrice appliedPrice, int dataIndex, double[] opens, double[] highs, double[] lows, double[] closes)
        {
            return appliedPrice switch
            {
                AppliedPrice.Open => opens[dataIndex],
                AppliedPrice.High => highs[dataIndex],
                AppliedPrice.Low => lows[dataIndex],
                AppliedPrice.Close => closes[dataIndex],
                AppliedPrice.Median => (highs[dataIndex] + lows[dataIndex]) / 2.0,
                AppliedPrice.Typical => (highs[dataIndex] + lows[dataIndex] + closes[dataIndex]) / 3.0,
                AppliedPrice.Weighted => (highs[dataIndex] + lows[dataIndex] + 2 * closes[dataIndex]) / 4.0,
                _ => closes[dataIndex]
            };
        }

        /// <summary>
        /// Gets the parent indicator buffer configured at the requested source index.
        /// </summary>
        protected IIndicatorBuffer? GetParentBuffer(int sourceIndex = 0)
        {
            if (sourceIndex < 0 || sourceIndex >= Config.Sources.Count)
                return null;

            var src = Config.Sources[sourceIndex];
            if (src.IndicatorId == null)
                return null;
            return _manager.GetIndicatorBuffer(src.IndicatorId, src.BufferIndex);
        }

        /// <summary>
        /// Gets source data from the configured candle price source.
        /// </summary>
        protected double[]? GetSourceData(in int ratesTotal, in double[] opens, in double[] highs, in double[] lows, in double[] closes, int sourceIndex = 0)
        {
            // If no source is configured for slot 0, use candle close prices by default.
            if (sourceIndex == 0 && Config.Sources.Count == 0)
            {
                var fallback = GetFallbackAppliedPrice();
                return fallback switch
                {
                    AppliedPrice.Open => opens,
                    AppliedPrice.High => highs,
                    AppliedPrice.Low => lows,
                    AppliedPrice.Close => closes,
                    AppliedPrice.Median => highs.Zip(lows, (h, l) => (h + l) / 2.0).ToArray(),
                    AppliedPrice.Typical => highs.Zip(lows, (h, l) => h + l).Zip(closes, (hl, c) => (hl + c) / 3.0).ToArray(),
                    AppliedPrice.Weighted => highs.Zip(lows, (h, l) => h + l).Zip(closes, (hl, c) => (hl + 2 * c) / 4.0).ToArray(),
                    _ => closes
                };
            }

            if (sourceIndex < 0 || sourceIndex >= Config.Sources.Count)
                return null;

            var src = Config.Sources[sourceIndex];
            if (src.IndicatorId == null)
            {
                return src.AppliedPrice switch
                {
                    AppliedPrice.Open => opens,
                    AppliedPrice.High => highs,
                    AppliedPrice.Low => lows,
                    AppliedPrice.Close => closes,
                    AppliedPrice.Median => highs.Zip(lows, (h, l) => (h + l) / 2.0).ToArray(),
                    AppliedPrice.Typical => highs.Zip(lows, (h, l) => h + l).Zip(closes, (hl, c) => (hl + c) / 3.0).ToArray(),
                    AppliedPrice.Weighted => highs.Zip(lows, (h, l) => h + l).Zip(closes, (hl, c) => (hl + 2 * c) / 4.0).ToArray(),
                    _ => closes
                };
            }

            if (src.TimeFrame != null && src.TimeFrame != Config.TimeFrame)
                return null;

            try
            {
                var buffer = _manager.GetIndicatorBuffer(src.IndicatorId, src.BufferIndex);
                if (buffer == null)
                    return null;

                var result = new double[ratesTotal];
                for (var i = 0; i < ratesTotal; i++)
                {
                    var item = buffer.TryAt(i);
                    result[i] = item.IsEmpty ? double.NaN : item.Value;
                }
                return result;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public virtual int OnCalculate(in int ratesTotal, in int prevCalculated,
            in DateTime[] datetime, in double[] opens, in double[] highs, in double[] lows,
            in double[] closes, in double[] volumes, in double spreads)
        {
            // Default implementation resolves the configured source and delegates to the single-series overload.
            var prices = GetSourceData(ratesTotal, opens, highs, lows, closes);
            if (prices != null)
            {
                return OnCalculate(ratesTotal, prevCalculated, 0, prices);
            }
            return 0;
        }

        /// <summary>
        /// Declares an indicator buffer at the given index.
        /// </summary>
        /// <param name="index">Buffer index.</param>
        /// <param name="type">Buffer type.</param>
        /// <param name="allowEmptySlots">Whether the buffer may contain empty slots.</param>
        protected void SetBuffer(int index, IndicatorBufferType type = IndicatorBufferType.Data, bool allowEmptySlots = false)
        {
            _manager.SetIndexBuffer(_uniqueId, index, type, allowEmptySlots);
        }

        /// <summary>
        /// Writes a value into an indicator buffer at a data index and timestamp.
        /// </summary>
        protected void SetBufferValue(int bufferIndex, int dataIndex, DateTime timestamp, double value)
        {
            var buffer = GetBuffer(bufferIndex);
            if (buffer != null)
            {
                buffer.Add(dataIndex, timestamp, value);
            }
        }

        /// <inheritdoc/>
        public string GetIndicatorId() => _uniqueId ?? Identity.GenerateUniqueId();

        /// <inheritdoc/>
        public string GetDisplayName() => Identity.GetDisplayName();

        /// <inheritdoc/>
        public IIndicatorBuffer GetBuffer(int index)
        {
            return _manager.GetIndicatorBuffer(_uniqueId, index) ?? throw new InvalidOperationException($"Buffer {index} not found");
        }

        /// <summary>
        /// Sets the unique ID manually. This is intended for special integration scenarios.
        /// </summary>
        /// <param name="id">Unique indicator ID.</param>
        public void SetUniqueId(string id) => _uniqueId = id;

        /// <inheritdoc/>
        public virtual IndicatorValue GetAt(int index = 0)
        {
            return GetBuffer(0).FindAtOrBeforeCurrent(index);
        }

        /// <inheritdoc/>
        public virtual IndicatorValue GetAt(int index, int bufferIndex)
        {
            return GetBuffer(bufferIndex).FindAtOrBeforeCurrent(index);
        }

        /// <inheritdoc/>
        public virtual IEnumerable<IndicatorValue> GetRange(int count = 1)
        {
            return GetBuffer(0).FindBefore(count) ?? Enumerable.Empty<IndicatorValue>();
        }

        /// <inheritdoc/>
        public virtual IEnumerable<IndicatorValue> GetRange(int count, int bufferIndex)
        {
            return GetBuffer(bufferIndex).FindBefore(count) ?? Enumerable.Empty<IndicatorValue>();
        }
    }
}
