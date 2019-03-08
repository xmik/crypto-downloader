using System;
using ExchangeSharp;

namespace CryptoDownloaderConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            ExchangeKrakenAPI api = new ExchangeKrakenAPI();
            ExchangeTicker ticker = api.GetTickerAsync("XXBTZUSD").Sync();
            Console.WriteLine("On the Kraken exchange, 1 bitcoin is worth {0} USD.", ticker.Bid);
            var symbols = api.GetMarketSymbolsAsync().Sync();
            Console.WriteLine("symbols:");
            foreach (string symbol in symbols)
            {
                Console.WriteLine(symbol);
            }
        }
    }
}
