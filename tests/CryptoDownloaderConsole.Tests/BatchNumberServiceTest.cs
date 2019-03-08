using System;
using Xunit;

namespace CryptoDownloader.Tests
{
    public class BatchNumberServiceTest
    {
        [Fact]
        public void ShouldReturnValidBatchNumberToBeDownloaded() 
        {
            var service = new BatchNumberService();
            int batchNumber = service.GetBatchNumberToBeDownloaded(DateTimeExtensions.CreateNodaTime(2000,2,19,1,1,1));
            Assert.Equal(0, batchNumber);
            // the first batch data range has not yet passed
            int batchNumber0 = service.GetBatchNumberToBeDownloaded(DateTimeExtensions.CreateNodaTime(2019,2,16,23,59,59));
            Assert.Equal(0, batchNumber0);

            // batch1 == DateTimeExtensions.CreateNodaTime(2019,2,10,0,0,0), DateTimeExtensions.CreateNodaTime(2019,2,16,23,59,59), so dates that will
            // result in getting this batch are: DateTimeExtensions.CreateNodaTime(2019,2,17,0,0,0) to DateTimeExtensions.CreateNodaTime(2019,2,23,23,59,59)
            int batchNumber1a = service.GetBatchNumberToBeDownloaded(DateTimeExtensions.CreateNodaTime(2019,2,17,0,0,0));
            Assert.Equal(1, batchNumber1a);
            int batchNumber1 = service.GetBatchNumberToBeDownloaded(DateTimeExtensions.CreateNodaTime(2019,2,19,1,1,1));
            Assert.Equal(1, batchNumber1);
            int batchNumber2 = service.GetBatchNumberToBeDownloaded(DateTimeExtensions.CreateNodaTime(2019,2,23,1,1,1));
            Assert.Equal(1, batchNumber2);
            int batchNumber2a = service.GetBatchNumberToBeDownloaded(DateTimeExtensions.CreateNodaTime(2019,2,23,23,59,59));
            Assert.Equal(1, batchNumber2a);

            // batch2 == DateTimeExtensions.CreateNodaTime(2019,2,17,0,0,0), DateTimeExtensions.CreateNodaTime(2019,2,23,23,59,59)
            // result in getting this batch are: DateTimeExtensions.CreateNodaTime(2019,2,24,0,0,0) to DateTimeExtensions.CreateNodaTime(2019,3,2,23,59,59)
            int batchNumber3 = service.GetBatchNumberToBeDownloaded(DateTimeExtensions.CreateNodaTime(2019,2,24,1,1,1));
            Assert.Equal(2, batchNumber3);
            int batchNumber4 = service.GetBatchNumberToBeDownloaded(DateTimeExtensions.CreateNodaTime(2019,2,25,1,1,1));
            Assert.Equal(2, batchNumber4);

            int batchNumber5 = service.GetBatchNumberToBeDownloaded(DateTimeExtensions.CreateNodaTime(2019,3,4,1,1,1));
            Assert.Equal(3, batchNumber5);
        }
        
        [Fact]
        public void ShouldReturnValidBatchNumberToBeDownloaded_WhenBatchNumberIsZero() 
        {
            Action testCode = () => { new BatchNumberService().GetDataRange(0); };
            var ex = Record.Exception(testCode);
            Assert.NotNull(ex);
            Assert.IsType<ArgumentException>(ex);
            Assert.Equal("Expected batch number >0, but was: 0", ex.Message);
        }

        [Fact]
        public void ShouldReturnValidDataRange() 
        {
            var service = new BatchNumberService();
            var range = service.GetDataRange(1);
            Assert.Equal(DateTimeExtensions.CreateNodaTime(2019,2,10,0,0,0), range.Start);
            Assert.Equal(DateTimeExtensions.CreateNodaTime(2019,2,16,23,59,59), range.End);

            var range1 = service.GetDataRange(2);
            Assert.Equal(DateTimeExtensions.CreateNodaTime(2019,2,17,0,0,0), range1.Start);
            Assert.Equal(DateTimeExtensions.CreateNodaTime(2019,2,23,23,59,59), range1.End);
            
            var range2 = service.GetDataRange(3);
            Assert.Equal(DateTimeExtensions.CreateNodaTime(2019,2,24,0,0,0), range2.Start);
            Assert.Equal(DateTimeExtensions.CreateNodaTime(2019,3,2,23,59,59), range2.End);
            
            var range12 = service.GetDataRange(12);
            Assert.Equal(DateTimeExtensions.CreateNodaTime(2019,4,28,0,0,0), range12.Start);
            Assert.Equal(DateTimeExtensions.CreateNodaTime(2019,5,4,23,59,59), range12.End);
        }

        [Fact]
        public void ShouldGetWiderBatchDateRange() 
        {
            var range = new BatchDateRange(
                DateTimeExtensions.CreateNodaTime(2019,2,10,0,0,0),
                DateTimeExtensions.CreateNodaTime(2019,2,16,23,59,59));

            var service = new BatchNumberService();
            var widerRange = service.GetWiderBatchDateRange(range);
            Assert.Equal(DateTimeExtensions.CreateNodaTime(2019,2,9,23,45,0), widerRange.Start);
            Assert.Equal(DateTimeExtensions.CreateNodaTime(2019,2,17,0,14,59), widerRange.End);
        }
    }
}