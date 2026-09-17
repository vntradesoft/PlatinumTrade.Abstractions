namespace Pt.Okx.Abstractions.Strategy.Plugin
{
    /// <summary>
    /// Declares the input schema type for a strategy plugin.
    /// The schema type is parsed by the platform to build InputParameter defaults.
    /// </summary>
    public interface IStrategyPluginInputSchema
    {
        /// <summary>
        /// Gets the schema type that contains input parameter declarations.
        /// </summary>
        Type GetInputSchemaType();
    }
}
