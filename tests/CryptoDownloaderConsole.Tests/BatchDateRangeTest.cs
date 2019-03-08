using System;
using Xunit;

namespace CryptoDownloader.Tests
{
    public class BatchDateRangeTest
    {
        [Fact]
        public void ShouldHaveValidDateRange()
        {
            var batchdr = new BatchDateRange(
                new DateTime(2019,2,10,0,0,0, DateTimeKind.Utc).ToNodaTime(),
                new DateTime(2019,2,16,23,59,59, DateTimeKind.Utc).ToNodaTime());
            Assert.Equal(new DateTime(2019,2,10,0,0,0, DateTimeKind.Utc).ToNodaTime(), batchdr.Start);
            Assert.Equal(new DateTime(2019,2,16,23,59,59, DateTimeKind.Utc).ToNodaTime(), batchdr.End);
        }

        [Fact]
        public void ShouldThrowExceptionWhenInvalidDateTimeKind()
        {
            Action testCode = () => {  new BatchDateRange(
                new DateTime(2019,2,10,0,0,0).ToNodaTime(),
                new DateTime(2019,2,16,23,59,59).ToNodaTime()); };
            var ex = Record.Exception(testCode);
            Assert.NotNull(ex);
            Assert.IsType<ArgumentException>(ex);
            Assert.Equal("Invalid DateTime.Kind for Instant.FromDateTimeUtc\nParameter name: dateTime", ex.Message);
        }

        [Fact]
        public void ShouldThrowExceptionWhenStartIsNotMidnight()
        {
            var range = new BatchDateRange(
                DateTimeExtensions.CreateNodaTime(2019,2,10,0,1,0), DateTimeExtensions.CreateNodaTime(2019,2,16,23,59,59));
            Action testCode = () => { range.Verify(); };
            var ex = Record.Exception(testCode);
            Assert.NotNull(ex);
            Assert.IsType<ArgumentException>(ex);
            Assert.Equal("Expected start dateTime to be HH:MM:SS 00:00:00, was: 00:01:00", ex.Message);
        }

        [Fact]
        public void ShouldThrowExceptionWhenStartIsNotSunday()
        {
            var range = new BatchDateRange(DateTimeExtensions.CreateNodaTime(2019,2,9,0,0,0), DateTimeExtensions.CreateNodaTime(2019,2,16,23,59,59));
            Action testCode = () => { range.Verify(); };
            var ex = Record.Exception(testCode);
            Assert.NotNull(ex);
            Assert.IsType<ArgumentException>(ex);
            Assert.Equal("Expected start dateTime to be Sunday, was: Saturday", ex.Message);
        }

        [Fact]
        public void ShouldThrowExceptionWhenEndIsNotSaturday()
        {
            var range = new BatchDateRange(DateTimeExtensions.CreateNodaTime(2019,2,10,0,0,0), DateTimeExtensions.CreateNodaTime(2019,2,15,23,59,59));
            Action testCode = () => { range.Verify(); };
            var ex = Record.Exception(testCode);
            Assert.NotNull(ex);
            Assert.IsType<ArgumentException>(ex);
            Assert.Equal("Expected end dateTime to be Saturday, was: Friday", ex.Message);
        }

        [Fact]
        public void ShouldThrowExceptionWhenEndIsNotAt235959()
        {
            var range = new BatchDateRange(DateTimeExtensions.CreateNodaTime(2019,2,10,0,0,0), DateTimeExtensions.CreateNodaTime(2019,2,16,23,59,58));
            Action testCode = () => { range.Verify(); };
            var ex = Record.Exception(testCode);
            Assert.NotNull(ex);
            Assert.IsType<ArgumentException>(ex);
            Assert.Equal("Expected end dateTime to be HH:MM:SS 23:59:59, was: 23:59:58", ex.Message);
        }

        [Fact]
        public void ShouldThrowExceptionWhenRangeIsTooLong()
        {
            var range = new BatchDateRange(DateTimeExtensions.CreateNodaTime(2019,2,10,0,0,0), DateTimeExtensions.CreateNodaTime(2019,2,23,23,59,59));
            Action testCode = () => { range.Verify(); };
            var ex = Record.Exception(testCode);
            Assert.NotNull(ex);
            Assert.IsType<ArgumentException>(ex);
            Assert.Equal("Expected end datetime - start datetime to be 1 week - 1 hour, was: 336", ex.Message);
        }

        [Fact]
        public void ShouldThrowExceptionWhenRangeIsTooShort()
        {
            var range = new BatchDateRange(DateTimeExtensions.CreateNodaTime(2019,2,10,0,0,0), DateTimeExtensions.CreateNodaTime(2019,2,2,23,59,59));
            Action testCode = () => { range.Verify(); };
            var ex = Record.Exception(testCode);
            Assert.NotNull(ex);
            Assert.IsType<ArgumentException>(ex);
            Assert.Equal("Expected end datetime - start datetime to be 1 week - 1 hour, was: -168", ex.Message);
        }

        [Fact]
        public void ShouldReturnNextBatchDateRange()
        {
            var batchdr = new BatchDateRange(DateTimeExtensions.CreateNodaTime(2019,2,10,0,0,0), DateTimeExtensions.CreateNodaTime(2019,2,16,23,59,59));
            var next = batchdr.GetNext();
            Assert.Equal(DateTimeExtensions.CreateNodaTime(2019,2,17,0,0,0), next.Start);
            Assert.Equal(DateTimeExtensions.CreateNodaTime(2019,2,23,23,59,59), next.End);
            var next2 = next.GetNext();
            Assert.Equal(DateTimeExtensions.CreateNodaTime(2019,2,24,0,0,0), next2.Start);
            Assert.Equal(DateTimeExtensions.CreateNodaTime(2019,3,2,23,59,59), next2.End);
        }

        [Fact]
        public void ShouldReturnPrettyString()
        {
            var batchdr = new BatchDateRange(DateTimeExtensions.CreateNodaTime(2019,2,10,0,0,0), DateTimeExtensions.CreateNodaTime(2019,2,16,23,59,59));
            string str = batchdr.ToString();
            Assert.Equal("Start: 2019-02-10 00:00:00 End: 2019-02-16 23:59:59", str);
        }


        [Fact]
        public void ShouldIncludesNodaTime()
        {
            var batchdr = new BatchDateRange(DateTimeExtensions.CreateNodaTime(2019,2,10,0,0,0),
                DateTimeExtensions.CreateNodaTime(2019,2,16,23,59,59));
            // start
            bool included = batchdr.Includes(DateTimeExtensions.CreateNodaTime(2019,2,10,0,0,0));
            Assert.True(included);
            // > start && < end
            bool included1 = batchdr.Includes(DateTimeExtensions.CreateNodaTime(2019,2,13,23,22,22));
            Assert.True(included1);
            // end
            bool included2 = batchdr.Includes(DateTimeExtensions.CreateNodaTime(2019,2,16,23,59,59));
            Assert.True(included2);
            // < start
            bool included3 = batchdr.Includes(DateTimeExtensions.CreateNodaTime(1,2,13,23,22,22));
            Assert.False(included3);
            // > end
            bool included4 = batchdr.Includes(DateTimeExtensions.CreateNodaTime(2019,2,17,0,0,0));
            Assert.False(included4);
        }

        [Fact]
        public void ShouldIncludesNodaTimeRange()
        {
            var batchdr = new BatchDateRange(DateTimeExtensions.CreateNodaTime(2019,2,10,0,0,0),
                DateTimeExtensions.CreateNodaTime(2019,2,16,23,59,59));
            // start == batchdr.Start && end == batchdr.Start
            var dr1 = new BatchDateRange(DateTimeExtensions.CreateNodaTime(2019,2,10,0,0,0), DateTimeExtensions.CreateNodaTime(2019,2,10,0,0,0));
            Assert.True(batchdr.Includes(dr1));
            // start == batchdr.Start && end == batchdr.End
            var dr2 = new BatchDateRange(DateTimeExtensions.CreateNodaTime(2019,2,10,0,0,0), DateTimeExtensions.CreateNodaTime(2019,2,16,23,59,59));
            Assert.True(batchdr.Includes(dr2));
            // start == batchdr.End && end == batchdr.End
            var dr3 = new BatchDateRange(DateTimeExtensions.CreateNodaTime(2019,2,16,23,59,59), DateTimeExtensions.CreateNodaTime(2019,2,16,23,59,59));
            Assert.True(batchdr.Includes(dr3));
            // start > batchdr.Start && end < batchdr.End
            var dr4 = new BatchDateRange(DateTimeExtensions.CreateNodaTime(2019,2,10,1,59,59), DateTimeExtensions.CreateNodaTime(2019,2,15,23,59,59));
            Assert.True(batchdr.Includes(dr4));
            // start > batchdr.Start && end > batchdr.End
            var dr5 = new BatchDateRange(DateTimeExtensions.CreateNodaTime(2019,2,10,1,59,59), DateTimeExtensions.CreateNodaTime(2019,2,17,23,59,59));
            Assert.False(batchdr.Includes(dr5));
            // start < batchdr.Start && end < batchdr.End
            var dr6 = new BatchDateRange(DateTimeExtensions.CreateNodaTime(2019,2,9,1,59,59), DateTimeExtensions.CreateNodaTime(2019,2,15,23,59,59));
            Assert.False(batchdr.Includes(dr6));
            // start < batchdr.Start && end > batchdr.End
            var dr7 = new BatchDateRange(DateTimeExtensions.CreateNodaTime(2019,2,9,1,59,59), DateTimeExtensions.CreateNodaTime(2019,2,17,23,59,59));
            Assert.False(batchdr.Includes(dr7));
        }
    }
}