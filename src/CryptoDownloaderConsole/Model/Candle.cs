using System;

namespace CryptoDownloader
{
    public struct Candle
    {     
        public Candle(ExchangeSharp.MarketCandle candle)
        {
            this.OpenPrice = (float)candle.OpenPrice;
            this.HighPrice = (float)candle.HighPrice;
            this.LowPrice = (float)candle.LowPrice;
            this.ClosePrice = (float)candle.ClosePrice;
        }
        public Candle(float open, float high, float low, float close)
        {
            this.OpenPrice = open;
            this.HighPrice = high;
            this.LowPrice = low;
            this.ClosePrice = close;
        }

        /// <summary>
        /// Opening price
        /// </summary>
        public float OpenPrice { get; set; }

        /// <summary>
        /// High price
        /// </summary>
        public float HighPrice { get; set; }

        /// <summary>
        /// Low price
        /// </summary>
        public float LowPrice { get; set; }

        /// <summary>
        /// Close price
        /// </summary>
        public float ClosePrice { get; set; }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}",
                OpenPrice, HighPrice, LowPrice, ClosePrice);
        }
    }
}