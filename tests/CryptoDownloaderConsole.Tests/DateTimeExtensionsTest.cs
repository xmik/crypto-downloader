using System;
using Xunit;

namespace CryptoDownloader.Tests
{
    public class DateTimeExtensionsTest
    {
        [Fact]
        public void ShouldConvertNodaTimeToDateTime()
        {
            var nodaTime = NodaTime.Instant.FromUnixTimeTicks(0);
            Assert.Equal(nodaTime.ToDateTime(), new DateTime(1970,1,1,0,0,0));

            var nodaTime2 = NodaTime.Instant.FromUnixTimeTicks(10000000);
            Assert.Equal(nodaTime2.ToDateTime(), new DateTime(1970,1,1,0,0,1));

            var nodaTime3 = NodaTime.Instant.FromUnixTimeTicks(15497568000000000);
            Assert.Equal(nodaTime3.ToDateTime(), new DateTime(2019,2,10,0,0,0));
        }


        [Fact]
        public void ShouldConvertDateTimeToNodaTime()
        {
            var dateTime = new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc);
            Assert.Equal(dateTime.ToNodaTime(), NodaTime.Instant.FromUnixTimeTicks(0));

            var dateTime1 = new DateTime(1970,1,1,0,0,1, DateTimeKind.Utc);
            Assert.Equal(dateTime1.ToNodaTime(), NodaTime.Instant.FromUnixTimeTicks(10000000));

            var dateTime2 = new DateTime(2019,2,10,0,0,0, DateTimeKind.Utc);
            Assert.Equal(dateTime2.ToNodaTime(), NodaTime.Instant.FromUnixTimeTicks(15497568000000000));
        }

        [Fact]
        public void ShouldCreateNodaTime()
        {
            var nodaTime = DateTimeExtensions.CreateNodaTime(1970,1,1,0,0,0);
            Assert.Equal(new DateTime(1970,1,1,0,0,0), nodaTime.ToDateTime());

            var nodaTime1 = DateTimeExtensions.CreateNodaTime(2011,1,3,0,44,0);
            Assert.Equal(new DateTime(2011,1,3,0,44,0), nodaTime1.ToDateTime());
        }

        [Fact]
        public void DateTimeShouldNotBeInfluencesByTimeZones()
        {
            // before date time change
            var timestamp = new DateTime(2019,3,30,1,30,0, DateTimeKind.Utc);
            var ticks1 = timestamp.ToNodaTime().ToUnixTimeTicks();
            // the number of timestamp.Ticks is from the beginning of the DateTime era
            var ticks2 = timestamp.Ticks - new DateTime(1970, 1, 1).Ticks;
            Assert.Equal(ticks1, ticks2);
            Assert.Equal(new DateTime(2019,3,30,1,30,0,DateTimeKind.Local), timestamp);

            // after date time change
            var timestamp1 = new DateTime(2019,3,30,4,30,0, DateTimeKind.Utc);
            var ticks1a = timestamp.ToNodaTime().ToUnixTimeTicks();
            // the number of timestamp.Ticks is from the beginning of the DateTime era
            var ticks2a = timestamp.Ticks - new DateTime(1970, 1, 1).Ticks;
            Assert.Equal(ticks1a, ticks2a);
            Assert.Equal(new DateTime(2019,3,30,4,30,0,DateTimeKind.Local), timestamp1);
        }
    }
}