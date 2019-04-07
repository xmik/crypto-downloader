using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Linq;

namespace CryptoDownloader
{
    public class EventSavingService : IEventSavingService
    {
		private static readonly log4net.ILog _log = log4net.LogManager.GetLogger (typeof(EventSavingService));

        public EventSavingService()
        {        }

        public int Write(CandleEvent[] candleEvents, string filePath, CancellationToken ct, IRange<NodaTime.Instant> allowedDateRange)
        {
			int i = 0;
            NodaTime.Instant lasttimestamp = NodaTime.Instant.FromDateTimeUtc(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            if (File.Exists(filePath))
            {
                string lastline = File.ReadLines(filePath, Encoding.UTF8).Last();
                lastline = lastline.Substring(0, lastline.IndexOf(','));
                DateTime lasttime = DateTime.ParseExact(lastline, "yyyy-M-ddTHH:mm:ss", null);
                lasttimestamp = DateTimeExtensions.ToNodaTime(lasttime.ToUniversalTime());
            }
            else
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, false))
                {
                    file.WriteLine("<date time>,<open price>,<high price>,<low price>,<close price>");
                }
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, true))
            {
                foreach (var ce in candleEvents)
                {
                    var candletime = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddTicks(ce.Date);
                    if (i == 0 & candletime.Equals(lasttimestamp)) continue;
                    if (allowedDateRange.Includes(DateTimeExtensions.ToNodaTime(candletime)))
                    {
                        file.WriteLine("{0},{1}",
                            candletime.ToString("yyyy-M-ddTHH:mm:ss"),
                            ce.Value.ToString());
                    }
                    i++;
                }
            }
            _log.DebugFormat("Finished writing to series, added: {0} events", candleEvents.Length);
            return i;
        }
    }
}