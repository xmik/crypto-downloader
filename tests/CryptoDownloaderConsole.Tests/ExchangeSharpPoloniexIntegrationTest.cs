using System;
using Xunit;

namespace CryptoDownloader.Tests
{
    [Trait("Category","integration")]
    public class ExchangeSharpPoloniexIntegrationTest
    {
        [Fact]
        public void ShouldListSymbols()
        {
            ExchangeSharpPoloniex poloniexApi = new ExchangeSharpPoloniex();
            var symbols = poloniexApi.ListSymbols();
            Assert.Contains("BTC_ARDR", symbols);
            Assert.Contains("BTC_BCN", symbols);
            Assert.Contains("BTC_ETC", symbols);
            Assert.Contains("BTC_ETH", symbols);
            Assert.Contains("BTC_XMR", symbols);
            Assert.Contains("USDC_BTC", symbols);
            Assert.Contains("USDC_XMR", symbols);
            Assert.Contains("USDT_BCH", symbols);
            Assert.Contains("USDT_XMR", symbols);
            // Uncomment in order to print all the symbols. You have to run the test with "Debug Test"
            // and go to the View: "Debug Console".
            // foreach (string symbol in symbols)
            // {
            //     Debug.WriteLine(symbol);
            // }
        }

        [Fact]
        public void ShouldGetHistoricalPrices()
        {
            ExchangeSharpPoloniex poloniexApi = new ExchangeSharpPoloniex();
            CandleEvent[] candleEvents = poloniexApi.GetHistoricalPrices("USDC_BTC", 300, 
                DateTimeExtensions.CreateNodaTime(2019,2,10,0,0,0),
                DateTimeExtensions.CreateNodaTime(2019,2,10,12,04,0));
            // Uncomment in order to print all the symbols. You have to run the test with "Debug Test"
            // and go to the View: "Debug Console".
            // foreach (Event<Candle> candleEvent in candleEvents)
            // {
            //     Debug.WriteLine(String.Format("{0} - {1}", candleEvent.Date, candleEvent.Value));
            // }

            // [GlobalDate: 15497568000000000] - 3628.599, 3628.599, 3628.599, 3628.599
            var firstEvent = candleEvents[0];
            Assert.Equal(15497568000000000, firstEvent.Date);
            Assert.Equal(DateTimeExtensions.CreateNodaTime(2019,2,10,0,0,0).ToUnixTimeTicks(), firstEvent.Date);
            var tuple = CheckCandlesEqual(new Candle(3628.599f, 3628.599f, 3628.599f, 3628.599f), firstEvent.Value);
            Assert.True(tuple.Item1, tuple.Item2);

            // [GlobalDate: 15497571000000000] - 3628.599, 3628.6, 3624.248, 3625.45
            var secondEvent = candleEvents[1];
            Assert.Equal(15497571000000000, secondEvent.Date);
            Assert.Equal(DateTimeExtensions.CreateNodaTime(2019,2,10,0,05,0).ToUnixTimeTicks(), secondEvent.Date);
            var tuple2 = CheckCandlesEqual(new Candle(3628.599f, 3628.6f, 3624.248f, 3625.45f), secondEvent.Value);
            Assert.True(tuple2.Item1, tuple2.Item2);
        }

        public static Tuple<bool,string> CheckCandlesEqual(Candle candle1, Candle candle2)
        {
            float epsilon = 0.0005f;
            if (!nearlyEqual(candle1.OpenPrice,candle2.OpenPrice, epsilon))
                return new Tuple<bool,string>(false,String.Format(
                    "Expected candle1: {0} to equal candle2: {1}. Compare OpenPrice", candle1, candle2));
            if (!nearlyEqual(candle1.HighPrice,candle2.HighPrice, epsilon))
                return new Tuple<bool,string>(false,String.Format(
                    "Expected candle1: {0} to equal candle2: {1}. Compare HighPrice", candle1, candle2));
            if (!nearlyEqual(candle1.LowPrice,candle2.LowPrice, epsilon))
                return new Tuple<bool,string>(false,String.Format(
                    "Expected candle1: {0} to equal candle2: {1}. Compare LowPrice", candle1, candle2));
            if (!nearlyEqual(candle1.ClosePrice,candle2.ClosePrice, epsilon))
                return new Tuple<bool,string>(false,String.Format(
                    "Expected candle1: {0} to equal candle2: {1}. Compare ClosePrice", candle1, candle2));
            return new Tuple<bool,string>(true,"");
        }
        public static bool nearlyEqual(float a, float b, float epsilon) {
            float absA = Math.Abs(a);
            float absB = Math.Abs(b);
            float diff = Math.Abs(a - b);

            if (a == b) 
            { // shortcut, handles infinities
                return true;
            }
            else if (diff < epsilon)
            {
                return true;
            } 
            else
            {
                return false;
            }
        }   
    }
}