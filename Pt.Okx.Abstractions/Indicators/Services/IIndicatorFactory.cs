using Pt.Okx.Abstractions.Indicators.Base;
using Pt.Okx.Shared.Indicators.Enums;
using Pt.Okx.Shared.Indicators.Models;
using Pt.Okx.Abstractions.Indicators.Plugin;

namespace Pt.Okx.Abstractions.Indicators.Services
{
    /// <summary>
    /// Provides registration, discovery, and creation of indicator instances.
    /// </summary>
    public interface IIndicatorFactory
    {
        /// <summary>
        /// Determines whether a custom indicator is registered under the specified name.
        /// </summary>
        bool IsCustomRegistered(string name);

        /// <summary>
        /// Gets the names of all registered custom indicators.
        /// </summary>
        string[] GetRegisteredCustomIndicators();

        /// <summary>
        /// Gets the parameter definitions for a registered custom indicator.
        /// </summary>
        IReadOnlyList<IndicatorParameterInfo> GetCustomParameterDefs(string name);

        /// <summary>
        /// Determines whether the specified built-in indicator type is supported.
        /// </summary>
        bool IsSupported(IndicatorType indicatorType);

        /// <summary>
        /// Gets all supported built-in indicator types.
        /// </summary>
        IndicatorType[] GetSupportedIndicators();

        /// <summary>
        /// Registers a custom indicator by name.
        /// </summary>
        void RegisterCustom(string name, IndicatorCreator creator);

        /// <summary>
        /// Registers a custom indicator and its parameter definitions.
        /// </summary>
        void RegisterCustom(string name, IndicatorCreator creator,
                            IReadOnlyList<IndicatorParameterInfo> parameterDefs);

        /// <summary>
        /// Sets or replaces the parameter definitions for a custom indicator.
        /// </summary>
        void SetCustomParameterDefs(string name, IReadOnlyList<IndicatorParameterInfo> defs);

        /// <summary>
        /// Removes a custom indicator registration by name.
        /// </summary>
        bool UnregisterCustom(string name);

        /// <summary>
        /// Creates an indicator instance for the specified configuration.
        /// </summary>
        IIndicator CreateIndicator(IIndicatorManager manager, IndicatorConfig config,
                                   Action<IndicatorProperty>? propertyOptions = null);
    }
}
