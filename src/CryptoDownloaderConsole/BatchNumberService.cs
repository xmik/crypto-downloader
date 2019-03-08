using System;

namespace CryptoDownloader
{
    public class BatchNumberService : IBatchNumberService
    {
        public static IRange<NodaTime.Instant> FirstBatchDateRange = new BatchDateRange(
            DateTimeExtensions.CreateNodaTime(2019,2,10,0,0,0),
            DateTimeExtensions.CreateNodaTime(2019,2,16,23,59,59));


        /// <summary>
        /// Returns the latest possible number of batch (integer) that covers the passed BatchDateRange,
        /// just before the dateTime.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public int GetBatchNumberToBeDownloaded(NodaTime.Instant dateTime) 
        {
            var currentBatchDateRange = FirstBatchDateRange;
            if (currentBatchDateRange.End >= dateTime) 
            {
                // the dateTime is so old, it will never be covered in a batch
                return 0;
            }
            int i = 1;
            while(true)
            {
                var nextBatchDateRange = currentBatchDateRange.GetNext();
                if (currentBatchDateRange.End < dateTime && nextBatchDateRange.End >= dateTime)
                {
                    return i;
                }
                i++;
                currentBatchDateRange = nextBatchDateRange;
                nextBatchDateRange = nextBatchDateRange.GetNext();
            }
        }
        public int GetOldBatchNumber(NodaTime.Instant dateTime) 
        {
            var batchNumer = GetBatchNumberToBeDownloaded(dateTime);
            return batchNumer -1;
        }

        public BatchDateRange GetWiderBatchDateRange(IRange<NodaTime.Instant> range)
        {
            var start = range.Start.Minus(NodaTime.Duration.FromMinutes(15));
            var end = range.End.Plus(NodaTime.Duration.FromMinutes(15));
            return new BatchDateRange(start, end);
        }

        public IRange<NodaTime.Instant> GetDataRange(int batchNumber)
        {
            if (batchNumber <= 0)
            {
                throw new ArgumentException(String.Format(
                    "Expected batch number >0, but was: {0}", batchNumber
                ));
            }
            var currentBatchDateRange = FirstBatchDateRange;
            int i = 1;
            while(true)
            {
                if (i == batchNumber)
                {
                    return currentBatchDateRange;
                }
                i++;
                currentBatchDateRange = currentBatchDateRange.GetNext();
            }
        }
    }
}