using Microsoft.Extensions.DependencyInjection;

namespace Pt.Okx.Abstractions.Strategy.Plugin
{
    /// <summary>
    /// Defines a plugin that can register services into the dependency injection container
    /// for extending strategy functionality.
    /// </summary>
    /// <remarks>
    /// A plugin can provide additional indicators, services, or integrations.
    /// Separate registration paths allow customizing behavior for live trading
    /// and backtesting environments.
    /// </remarks>
    public interface IStrategyPlugin
    {
        /// <summary>
        /// Registers services for the default runtime environment (e.g., live trading).
        /// </summary>
        /// <param name="services">The service collection.</param>
        void Register(IServiceCollection services);

        /// <summary>
        /// Registers services specifically for the backtest environment.
        /// </summary>
        /// <param name="services">The service collection.</param>
        void RegisterForBacktest(IServiceCollection services);
    }
}
