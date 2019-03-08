using System;
using System.Linq;
using ExchangeSharp;

namespace CryptoDownloader
{
    /// <summary>
    /// Wrapper for ExchangePoloniexAPI from nuget package: ExchangeSharp.
    /// All the references to ExchangeSharp project are put in this file only.
    /// </summary>
    public class ExchangeSharpPoloniex : IExchange
    {
        private ExchangeSharp.ExchangePoloniexAPI _api;
        public ExchangeSharpPoloniex()
        {
            _api = new ExchangeSharp.ExchangePoloniexAPI();
        }
        public string[] ListSymbols()
        {
            string[] currencyPairs = _api.GetMarketSymbolsAsync().Sync().ToArray();
            Array.Sort<string>(currencyPairs);
            return currencyPairs;
        }
        public CandleEvent[] GetHistoricalPrices(string instrument, int timeframeSeconds, NodaTime.Instant start, NodaTime.Instant end)
        {
            ExchangeSharp.MarketCandle[] foreignCandles = _api.GetCandlesAsync(
                instrument, timeframeSeconds, start.ToDateTime(), end.ToDateTime()).
                Sync().ToArray();
            CandleEvent[] ourEvents = new CandleEvent[foreignCandles.Length];
            for (int i=0; i<foreignCandles.Length; i++)
            {
                ourEvents[i] = Convert(foreignCandles[i]);
            }
            return ourEvents;
        }

        public CandleEvent Convert(ExchangeSharp.MarketCandle foreignCandle)
        {
            var candle = new Candle(foreignCandle);
            return new CandleEvent(candle, foreignCandle.Timestamp.ToNodaTime().ToUnixTimeTicks());
        }
    }
}