using System.Collections.Generic;
using System.Threading;

namespace CryptoDownloader
{
    public interface IEventSavingService
    {
        /// <summary>
        /// Writes (serializes) candleEvents to a file. Returns number of added Events.
        /// Before writing, it reads the date time of the last saved event in the file and
        /// if the next event from candleEvents has the same date time as the saved one,
        /// it will not be saved again.
        /// 
        /// candleEvents may contain more events than should be saved to a file. Thus,
        /// we only save those candleEvents, which date times is included in allowedDateRange.
        /// </summary>
        /// <param name="candleEvents">collection of CandleEvent objects to be serialized to a file</param>
        /// <param name="allowedDateRange">only candles within this date range will be added</param>
        /// <returns></returns>
        int Write(CandleEvent[] candleEvents,
            string filePath, CancellationToken ct, IRange<NodaTime.Instant> allowedDateRange);
    }
}