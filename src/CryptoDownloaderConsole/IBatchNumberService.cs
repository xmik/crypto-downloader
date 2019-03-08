using System;
using NodaTime.Text;

namespace CryptoDownloader
{
    public interface IBatchNumberService
    {
        int GetBatchNumberToBeDownloaded(NodaTime.Instant dateTime) ;
        int GetOldBatchNumber(NodaTime.Instant dateTime) ;
        IRange<NodaTime.Instant> GetDataRange(int batchNumber);
        /// <summary>
        /// Returns a longer (wider) batch date range. Start will be earlier,
        /// End will be later.
        /// This is done for safety reasons.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        BatchDateRange GetWiderBatchDateRange(IRange<NodaTime.Instant> range);
    }
}