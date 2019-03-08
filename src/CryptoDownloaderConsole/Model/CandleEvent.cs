namespace CryptoDownloader
{
    public class CandleEvent
    {
        public Candle Value { get; }
        public long Date { get; }
        public CandleEvent(Candle candle, long timestamp)
        {
            this.Date = timestamp;
            this.Value = candle;

        }
    }
}