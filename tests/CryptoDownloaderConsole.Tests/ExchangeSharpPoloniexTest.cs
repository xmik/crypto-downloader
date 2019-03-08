using Xunit;
using System;
using System.Diagnostics;

namespace CryptoDownloader.Tests
{
    public class ExchangeSharpPoloniexTest
    {
        [Fact]
        public void ShouldConvertCandle_WhenTimestampIsUnixEpoch()
        {
            ExchangeSharp.MarketCandle candle = new ExchangeSharp.MarketCandle();
            candle.OpenPrice = 32m;
            candle.HighPrice = 50m;
            candle.LowPrice = 1m;
            candle.ClosePrice = 1m;
            candle.Timestamp = new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc);

            ExchangeSharpPoloniex poloniexApi = new ExchangeSharpPoloniex();
            CandleEvent myEvent = poloniexApi.Convert(candle);
            Candle ourCandle = new Candle(32f, 50f, 1f, 1f);
            Assert.Equal(ourCandle, myEvent.Value);
            Assert.Equal(0, myEvent.Date);
        }

        [Fact]
        public void ShouldConvertCandle_WhenTimestampIsLaterThanUnixEpoch()
        {
            ExchangeSharp.MarketCandle candle = new ExchangeSharp.MarketCandle();
            candle.OpenPrice = 32m;
            candle.HighPrice = 50m;
            candle.LowPrice = 1m;
            candle.ClosePrice = 1m;
            candle.Timestamp = new DateTime(2019,2,19,15,22,32, DateTimeKind.Utc);

            ExchangeSharpPoloniex poloniexApi = new ExchangeSharpPoloniex();
            CandleEvent myEvent = poloniexApi.Convert(candle);
            Candle ourCandle = new Candle(32f, 50f, 1f, 1f);
            Assert.Equal(ourCandle, myEvent.Value);
            Assert.Equal(candle.Timestamp.ToNodaTime().ToUnixTimeTicks(), myEvent.Date);
        }        
    }
}