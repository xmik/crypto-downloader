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
        public void ShouldParseDateTime()
        {
            string line = "1970-01-01T00:00:00";
            string dateTimePattern = "yyyy-MM-ddTHH:mm:ss";
            var service = new EventSavingService();
            var parsedDT = service.ParseDateTime(line, dateTimePattern);
            Assert.Equal(NodaTime.Instant.FromUnixTimeTicks(0), parsedDT);
        }

        [Fact]
        public void ShouldParseDateTime2()
        {
            string line = "1970-01-01T00:02:00";
            string dateTimePattern = "yyyy-MM-ddTHH:mm:ss";
            var service = new EventSavingService();
            var parsedDT = service.ParseDateTime(line, dateTimePattern);
            Assert.Equal(DateTimeExtensions.CreateNodaTime(1970,1,1,0,2,0), parsedDT);
        }

        [Fact]
        public void ShouldWriteWhenEmptySeries_ShouldNotAddManyCandlesWithSameTimeStamp()
        {
            CandleEvent[] candleEvents = new CandleEvent[]{
                new CandleEvent(new Candle(33f, 34f, 12f, 15f), 1L),
                new CandleEvent(new Candle(33f, 100f, 12f, 15f), 1L),
                // we may want to check the date time written to a file,
                // so let's use the DateTime().ToNodaTime()... instead of just 
                // using integer number of ticks
                new CandleEvent(new Candle(99f, 99f, 12f, 15f), new DateTime(2019,3,30,1,32,0, DateTimeKind.Utc).ToNodaTime().ToUnixTimeTicks())
            };
            string filePath = Environment.CurrentDirectory + "/test1.csv";
            Cleanup(filePath);

            var seriesWriter = new EventSavingService();
            IRange<NodaTime.Instant> longRange = createLongRange();
            int addedCandles = seriesWriter.Write(candleEvents, filePath, cts.Token, longRange);

            // writing should have added 2 events;
            Assert.Equal(2, addedCandles);
            Assert.True (File.Exists (filePath));
            var fileContents = File.ReadAllText(filePath);
            Assert.Contains("1970-01-01T00:00:00,33,34,12,15", fileContents);
            Assert.Contains("2019-03-30T01:32:00,99,99,12,15", fileContents);
            Assert.DoesNotContain("33,100,12,15", fileContents);
            // assert that there are 2 candle events written to file (file has 3 lines, 1 is header)
            Assert.Equal(3, fileContents.Split('\n').Length);
            Cleanup(filePath);
        }

        /// <summary>
        /// Test that if we save to a file in some arbitrary directory,
        /// and that directory does not exist, it will be created 
        /// </summary>
        [Fact]
        public void ShouldEnsureDirectoryExists()
        {
            CandleEvent[] candleEvents = new CandleEvent[]{
                new CandleEvent(new Candle(33f, 34f, 12f, 15f), 1L),
            };
            string filePath = Environment.CurrentDirectory + "/newdir/test1.csv";
            Cleanup(filePath);

            var seriesWriter = new EventSavingService();
            IRange<NodaTime.Instant> longRange = createLongRange();
            int addedCandles = seriesWriter.Write(candleEvents, filePath, cts.Token, longRange);
            Assert.True (File.Exists (filePath));
            Cleanup(filePath);
            Directory.Delete("newdir");
        }

        [Fact]
        public void ShouldWriteWhenEmptySeries_LongBatchDateRange()
        {
            CandleEvent[] candleEvents = new CandleEvent[]{
                new CandleEvent(new Candle(33f, 34f, 12f, 15f), 1L),
                new CandleEvent(new Candle(33f, 34f, 12f, 15f), new DateTime(2019,3,30,1,30,0, DateTimeKind.Utc).ToNodaTime().ToUnixTimeTicks()),
                new CandleEvent(new Candle(99f, 99f, 12f, 15f), new DateTime(2019,3,30,1,32,0, DateTimeKind.Utc).ToNodaTime().ToUnixTimeTicks())
            };
            string filePath = Environment.CurrentDirectory + "/test2.csv";
            Cleanup(filePath);

            var seriesWriter = new EventSavingService();
            IRange<NodaTime.Instant> longRange = createLongRange();
            int addedCandles = seriesWriter.Write(candleEvents, filePath, cts.Token, longRange);

            // writing should have added 3 events;
            Assert.Equal(3, addedCandles);
            Assert.True (File.Exists (filePath));
            var fileContents = File.ReadAllText(filePath);
            Assert.Contains("1970-01-01T00:00:00,33,34,12,15", fileContents);
            Assert.Contains("2019-03-30T01:30:00,33,34,12,15", fileContents);
            Assert.Contains("2019-03-30T01:32:00,99,99,12,15", fileContents);
            // assert that there are 3 candle events written to file (file has 4 lines)
            Assert.Equal(4, fileContents.Split('\n').Length);

            int addedCandles2 = seriesWriter.Write(candleEvents, filePath, cts.Token, longRange);
            // writing again should not have added any more events
            Assert.Equal(0, addedCandles2);
            // assert that there are 3 candle events written to file (file has 4 lines) (no changes)
            Assert.Equal(4, fileContents.Split('\n').Length);
            Cleanup(filePath);
        }

        [Fact]
        public void ShouldWriteWhenSeriesHasFirstItem_LongDateRange()
        {
            CandleEvent[] candleEvents = new CandleEvent[]{
                new CandleEvent(new Candle(33f, 34f, 12f, 15f), 0L),
                new CandleEvent(new Candle(33f, 34f, 12f, 15f), new DateTime(2019,3,30,1,30,0, DateTimeKind.Utc).ToNodaTime().ToUnixTimeTicks()),
                new CandleEvent(new Candle(99f, 99f, 12f, 15f), new DateTime(2019,3,30,1,32,0, DateTimeKind.Utc).ToNodaTime().ToUnixTimeTicks())
            };
            string filePath = Environment.CurrentDirectory + "/test3.csv";
            Cleanup(filePath);

            IRange<NodaTime.Instant> longRange = createLongRange();

            // manually add 1st event
            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                // write to the file
                sw.WriteLine("1970-01-01T00:00:00,33,34,12,15");
            }

            var seriesWriter = new EventSavingService();
            int addedCandles = seriesWriter.Write(candleEvents, filePath, cts.Token, longRange);

            // writing should have added 3 - 1 = 2 events;
            Assert.Equal(2, addedCandles);
            Assert.True (File.Exists (filePath));
            var fileContents = File.ReadAllText(filePath);
            Assert.Contains("1970-01-01T00:00:00,33,34,12,15", fileContents);
            Assert.Contains("2019-03-30T01:30:00,33,34,12,15", fileContents);
            Assert.Contains("2019-03-30T01:32:00,99,99,12,15", fileContents);
            // assert that there are 3 candle events written to file (file has 4 lines)
            Assert.Equal(4, fileContents.Split('\n').Length);

            int addedCandles2 = seriesWriter.Write(candleEvents, filePath, cts.Token, longRange);
            // writing again should not have added any more events
            Assert.Equal(0, addedCandles2);
            // assert that there are 3 candle events written to file (file has 4 lines) (no changes)
            Assert.Equal(4, fileContents.Split('\n').Length);
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
            Assert.True (File.Exists (filePath));
            var fileContents = File.ReadAllText(filePath);
            Assert.Contains("2000-01-01T00:00:00,33,34,12,15", fileContents);
            Assert.Contains("2000-01-01T01:00:00,15,34,12,15", fileContents);
            Assert.Contains("2000-01-01T01:15:00,15,66,12,15", fileContents);
            Assert.Contains("2000-01-01T01:54:00,15,88,12,15", fileContents);
            // assert that there are 4 candle events written to file (file has 5 lines)
            Assert.Equal(5, fileContents.Split('\n').Length);

            int addedCandles2 = seriesWriter.Write(candleEvents, filePath, cts.Token, oneDayRange);
            // writing again should not have added any more events
            Assert.Equal(0, addedCandles2);
            // assert that there are 4 candle events written to file (file has 5 lines) (no changes)
            Assert.Equal(5, fileContents.Split('\n').Length);
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
                // date included in date range, event will be added (mock that it was already written to a file)
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
                sw.WriteLine("2000-01-01T00:00:00,33,34,12,15");
            }

            var seriesWriter = new EventSavingService();
            IRange<NodaTime.Instant> oneDayRange = new BatchDateRange(
                DateTimeExtensions.CreateNodaTime(2000,1,1,0,0,0),
                DateTimeExtensions.CreateNodaTime(2000,1,1,23,59,59)
            );

            int addedCandles = seriesWriter.Write(candleEvents, filePath, cts.Token, oneDayRange);
            // writing should have added 4-1=3 events;
            Assert.Equal(3, addedCandles);
            Assert.True (File.Exists (filePath));
            var fileContents = File.ReadAllText(filePath);
            Assert.Contains("2000-01-01T00:00:00,33,34,12,15", fileContents);
            Assert.Contains("2000-01-01T01:15:00,15,66,12,15", fileContents);
            Assert.Contains("2000-01-01T01:54:00,15,88,12,15", fileContents);
            Assert.DoesNotContain("99,99,12,15", fileContents);
            // assert that there are 4 candle events written to file (file has 5 lines)
            Assert.Equal(5, fileContents.Split('\n').Length);

            int addedCandles2 = seriesWriter.Write(candleEvents, filePath, cts.Token, oneDayRange);
            // writing again should not have added any more events
            Assert.Equal(0, addedCandles2);
            // assert that there are 4 candle events written to file (file has 5 lines) (no changes)
            Assert.Equal(5, fileContents.Split('\n').Length);
            Cleanup(filePath);
        }
    }
}