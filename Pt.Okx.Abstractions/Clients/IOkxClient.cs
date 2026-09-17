using Pt.Okx.Abstractions.Clients.Account;
using Pt.Okx.Abstractions.Clients.Instruments;
using Pt.Okx.Abstractions.Clients.Market;
using Pt.Okx.Abstractions.Clients.Trading;

namespace Pt.Okx.Abstractions.Clients
{
    /// <summary>
    /// Interface aggregating the main OKX platform services for clients.
    /// Provides restricted access compared to IOkx, exposing only safe functions for client use.
    /// </summary>
    public interface IOkxClient
    {
        /// <summary>
        /// Access to timeseries and technical indicator functions.
        /// </summary>
        ITimeSeriesClient Timeseries { get; init; }

        /// <summary>
        /// Access to trading instrument information and management functions.
        /// </summary>
        IInstrumentClient Instrument { get; init; }

        /// <summary>
        /// Access to account information and management functions.
        /// </summary>
        IAccountClient Account { get; init; }

        /// <summary>
        /// Access to trading and order placement functions.
        /// </summary>
        ITradeClient Trade { get; init; }
    }
}
