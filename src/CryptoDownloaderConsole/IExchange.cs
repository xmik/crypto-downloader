using System;

namespace CryptoDownloader
{
    /// <summary>
    /// Interface for many implementations of Poloniex Exchange API.
    /// </summary>
    public interface IExchange
    {
        string[] ListSymbols();
        CandleEvent[] GetHistoricalPrices(string instrument, int timeframeSeconds, NodaTime.Instant start, NodaTime.Instant end);
    }
}