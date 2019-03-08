using System;
using System.IO;
using System.Threading;
using Autofac;
using Xunit;

namespace CryptoDownloader.Tests
{
    public class EventSavingServiceTest
    {
        CancellationTokenSource cts;

        public EventSavingServiceTest()
        {
            cts = new CancellationTokenSource ();
        }

        private IRange<NodaTime.Instant> createLongRange()
        {
            return new BatchDateRange(
                DateTimeExtensions.CreateNodaTime(1,1,1,1,1,1),
                DateTimeExtensions.CreateNodaTime(3001,1,1,1,1,1)
            );
        }

        private void Cleanup(string filePath)
        {
            File.Delete(filePath);
        }

        [Fact]
        public void ShouldWriteWhenEmptySeries_ShouldNotAddManyCandlesWithSameTimeStamp()
        {
            CandleEvent[] candleEvents = new CandleEvent[]{
                new CandleEvent(new Candle(33f, 34f, 12f, 15f), 1L),
                new CandleEvent(new Candle(33f, 100f, 12f, 15f), 1L),
                new CandleEvent(new Candle(99f, 99f, 12f, 15f), 10L)
            };
            string filePath = Environment.CurrentDirectory + "/test1.csv";
            Cleanup(filePath);

            var seriesWriter = new EventSavingService();
            IRange<NodaTime.Instant> longRange = createLongRange();
            int addedCandles = seriesWriter.Write(candleEvents, filePath, cts.Token, longRange);

            // writing should have added 2 events;
            Assert.Equal(2, addedCandles);
            Assert.False (File.Exists (filePath));
            var fileContents = File.ReadAllText(filePath);
            // TODO: check date time saved in file
            Assert.Contains("33,34,12,15", filePath);
            Assert.Contains("99,99,12,15", filePath);
            Assert.DoesNotContain("33,100,12,15", filePath);
            // assert that there are 2 candle events written to file (file has 2 lines)
            Assert.Equal(2, fileContents.Split('\n').Length);
            Cleanup(filePath);
        }

        [Fact]
        public void ShouldWriteWhenEmptySeries_LongBatchDateRange()
        {
            CandleEvent[] candleEvents = new CandleEvent[]{
                new CandleEvent(new Candle(33f, 34f, 12f, 15f), 1L),
                new CandleEvent(new Candle(33f, 34f, 12f, 15f), 2L),
                new CandleEvent(new Candle(99f, 99f, 12f, 15f), 10L)
            };
            string filePath = Environment.CurrentDirectory + "/test2.csv";
            Cleanup(filePath);

            var seriesWriter = new EventSavingService();
            IRange<NodaTime.Instant> longRange = createLongRange();
            int addedCandles = seriesWriter.Write(candleEvents, filePath, cts.Token, longRange);

            // writing should have added 3 events;
            Assert.Equal(3, addedCandles);
            Assert.False (File.Exists (filePath));
            var fileContents = File.ReadAllText(filePath);
            // TODO: check date time saved in file
            Assert.Contains("33,34,12,15", filePath);
            Assert.Contains("99,99,12,15", filePath);
            // assert that there are 3 candle events written to file (file has 3 lines)
            Assert.Equal(3, fileContents.Split('\n').Length);

            int addedCandles2 = seriesWriter.Write(candleEvents, filePath, cts.Token, longRange);
            // writing again should not have added any more events
            Assert.Equal(0, addedCandles2);
            // assert that there are 3 candle events written to file (file has 3 lines) (no changes)
            Assert.Equal(3, fileContents.Split('\n').Length);
            Cleanup(filePath);
        }

        [Fact]
        public void ShouldWriteWhenSeriesHasFirstItem_LongDateRange()
        {
            CandleEvent[] candleEvents = new CandleEvent[]{
                new CandleEvent(new Candle(33f, 34f, 12f, 15f), 1L),
                new CandleEvent(new Candle(33f, 34f, 12f, 15f), 2L),
                new CandleEvent(new Candle(99f, 99f, 12f, 15f), 10L)
            };
            string filePath = Environment.CurrentDirectory + "/test3.csv";
            Cleanup(filePath);

            IRange<NodaTime.Instant> longRange = createLongRange();

            // manually add 1st event
            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                // write to the file
                // TODO: add date time
                sw.WriteLine("33,34,12,15");
            }

            var seriesWriter = new EventSavingService();
            int addedCandles = seriesWriter.Write(candleEvents, filePath, cts.Token, longRange);

            // writing should have added 3 - 1 = 2 events;
            Assert.Equal(2, addedCandles);
            Assert.False (File.Exists (filePath));
            var fileContents = File.ReadAllText(filePath);
            // TODO: check date time saved in file
            Assert.Contains("33,34,12,15", filePath);
            Assert.Contains("99,99,12,15", filePath);
            // assert that there are 3 candle events written to file (file has 3 lines)
            Assert.Equal(3, fileContents.Split('\n').Length);

            int addedCandles2 = seriesWriter.Write(candleEvents, filePath, cts.Token, longRange);
            // writing again should not have added any more events
            Assert.Equal(0, addedCandles2);
            // assert that there are 3 candle events written to file (file has 3 lines) (no changes)
            Assert.Equal(3, fileContents.Split('\n').Length);
            Cleanup(filePath);
        }

        [Fact]
        public void ShouldWriteWhenEmptySeries_ShortBatchDateRange()
        {
            CandleEvent[] candleEvents = new CandleEvent[]{
                // date NOT included in date range, event will NOT be added
                new CandleEvent(new Candle(11f, 34f, 12f, 15f), DateTimeExtensions.CreateNodaTime(1999,12,31,0,0,0).ToUnixTimeTicks()),
                // date NOT included in date range, event will NOT be added
                new CandleEvent(new Candle(11f, 34f, 12f, 15f), DateTimeExtensions.CreateNodaTime(1999,12,31,15,0,0).ToUnixTimeTicks()),
                // date included in date range, event will be added
                new CandleEvent(new Candle(33f, 34f, 12f, 15f), DateTimeExtensions.CreateNodaTime(2000,1,1,0,0,0).ToUnixTimeTicks()),
                // date included in date range, event will be added
                new CandleEvent(new Candle(15f, 34f, 12f, 15f), DateTimeExtensions.CreateNodaTime(2000,1,1,1,0,0).ToUnixTimeTicks()),
                // date included in date range, event will be added
                new CandleEvent(new Candle(15f, 66f, 12f, 15f), DateTimeExtensions.CreateNodaTime(2000,1,1,1,15,0).ToUnixTimeTicks()),
                // date included in date range, event will be added
                new CandleEvent(new Candle(15f, 88f, 12f, 15f), DateTimeExtensions.CreateNodaTime(2000,1,1,1,54,0).ToUnixTimeTicks()),
                // date NOT included in date range, event will NOT be added
                new CandleEvent(new Candle(99f, 99f, 12f, 15f), DateTimeExtensions.CreateNodaTime(2000,1,2,1,0,0).ToUnixTimeTicks()),
            };
            string filePath = Environment.CurrentDirectory + "/test4.csv";
            Cleanup(filePath);

            var seriesWriter = new EventSavingService();
            IRange<NodaTime.Instant> oneDayRange = new BatchDateRange(
                DateTimeExtensions.CreateNodaTime(2000,1,1,0,0,0),
                DateTimeExtensions.CreateNodaTime(2000,1,1,23,59,59)
            );
            int addedCandles = seriesWriter.Write(candleEvents, filePath, cts.Token, oneDayRange);

            // writing should have added 4 events;
            Assert.Equal(4, addedCandles);
            Assert.False (File.Exists (filePath));
            var fileContents = File.ReadAllText(filePath);
            // TODO: check date time saved in file
            Assert.Contains("33,34,12,15", filePath);
            Assert.Contains("15,88,12,15", filePath);
            // assert that there are 4 candle events written to file (file has 4 lines)
            Assert.Equal(4, fileContents.Split('\n').Length);

            int addedCandles2 = seriesWriter.Write(candleEvents, filePath, cts.Token, oneDayRange);
            // writing again should not have added any more events
            Assert.Equal(0, addedCandles2);
            // assert that there are 4 candle events written to file (file has 4 lines) (no changes)
            Assert.Equal(4, fileContents.Split('\n').Length);
            Cleanup(filePath);
        }

        [Fact]
        public void ShouldWriteWhenSeriesHasFirstItem_ShortDateRange()
        {
            CandleEvent[] candleEvents = new CandleEvent[]{
                // date NOT included in date range, event will NOT be added
                new CandleEvent(new Candle(11f, 34f, 12f, 15f), DateTimeExtensions.CreateNodaTime(1999,12,31,0,0,0).ToUnixTimeTicks()),
                // date NOT included in date range, event will NOT be added
                new CandleEvent(new Candle(11f, 34f, 12f, 15f), DateTimeExtensions.CreateNodaTime(1999,12,31,15,0,0).ToUnixTimeTicks()),
                // date included in date range, event will be added
                new CandleEvent(new Candle(33f, 34f, 12f, 15f), DateTimeExtensions.CreateNodaTime(2000,1,1,0,0,0).ToUnixTimeTicks()),
                // date included in date range, event will be added
                new CandleEvent(new Candle(15f, 34f, 12f, 15f), DateTimeExtensions.CreateNodaTime(2000,1,1,1,0,0).ToUnixTimeTicks()),
                // date included in date range, event will be added
                new CandleEvent(new Candle(15f, 66f, 12f, 15f), DateTimeExtensions.CreateNodaTime(2000,1,1,1,15,0).ToUnixTimeTicks()),
                // date included in date range, event will be added
                new CandleEvent(new Candle(15f, 88f, 12f, 15f), DateTimeExtensions.CreateNodaTime(2000,1,1,1,54,0).ToUnixTimeTicks()),
                // date NOT included in date range, event will NOT be added
                new CandleEvent(new Candle(99f, 99f, 12f, 15f), DateTimeExtensions.CreateNodaTime(2000,1,2,1,0,0).ToUnixTimeTicks()),
            };
            string filePath = Environment.CurrentDirectory + "/test5.csv";
            Cleanup(filePath);
            
            // manually add 1st event
            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                // write to the file
                // TODO: add date time
                sw.WriteLine("11,34,12,15");
            }

            var seriesWriter = new EventSavingService();
            IRange<NodaTime.Instant> oneDayRange = new BatchDateRange(
                DateTimeExtensions.CreateNodaTime(2000,1,1,0,0,0),
                DateTimeExtensions.CreateNodaTime(2000,1,1,23,59,59)
            );

            int addedCandles = seriesWriter.Write(candleEvents, filePath, cts.Token, oneDayRange);
            // writing should have added 4-1=3 events;
            Assert.Equal(3, addedCandles);
            Assert.False (File.Exists (filePath));
            var fileContents = File.ReadAllText(filePath);
            // TODO: check date time saved in file
            Assert.Contains("33,34,12,15", filePath);
            Assert.Contains("15,88,12,15", filePath);
            // assert that there are 4 candle events written to file (file has 4 lines)
            Assert.Equal(4, fileContents.Split('\n').Length);

            int addedCandles2 = seriesWriter.Write(candleEvents, filePath, cts.Token, oneDayRange);
            // writing again should not have added any more events
            Assert.Equal(0, addedCandles2);
            // assert that there are 4 candle events written to file (file has 4 lines) (no changes)
            Assert.Equal(4, fileContents.Split('\n').Length);
            Cleanup(filePath);
        }
    }
}