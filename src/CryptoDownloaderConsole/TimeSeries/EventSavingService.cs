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
            Boolean newfile = false;
            NodaTime.Instant lasttimestamp = NodaTime.Instant.FromDateTimeUtc(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            if (File.Exists(filePath))
            { //if file exists read the time stamp of the last entry
                string lastline = File.ReadLines(filePath, Encoding.UTF8).Last();
                lastline = lastline.Substring(0, lastline.IndexOf(','));
                DateTime lasttime = DateTime.ParseExact(lastline, "yyyy-M-ddTHH:mm:ss", null);
                lasttimestamp = DateTimeExtensions.ToNodaTime(lasttime.ToUniversalTime());
            }
            else
            { //if the file doesn't exist create one and add the header with information about file content
                newfile = true;
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, false))
                {
                    file.WriteLine("<date time>,<open price>,<high price>,<low price>,<close price>");
                }
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, true))
            {
                foreach (var ce in candleEvents) //save all candle events to file
                {
                    if(ct.IsCancellationRequested) break; //if cancelled stop the loop
                    var candletime = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddTicks(ce.Date);
                    if (i == 0 & candletime.Equals(lasttimestamp)) continue; //if the next event from candleEvents has the same date time as the last saved one it will be omitted
                    if (allowedDateRange.Includes(DateTimeExtensions.ToNodaTime(candletime)))
                    {
                        file.WriteLine("{0},{1}",
                            candletime.ToString("yyyy-M-ddTHH:mm:ss"),
                            ce.Value.ToString());
                    }
                    i++;
                }
            }
            if(ct.IsCancellationRequested){
                if(newfile) File.Delete(filePath); //if cancellation and file didn't exist before delete it entirely 
                else { //if cancellation and file existed before delete appended lines
                    string [] filecontent = File.ReadAllLines(filePath);
                    Array.Resize(ref filecontent, filecontent.Length - i);
                    File.Delete(filePath); //delete file
                    File.WriteAllLines(filePath, filecontent); //recreate file with lines before program action
                }
            }
            _log.DebugFormat("Finished writing to series, added: {0} events", candleEvents.Length);
            return i;
        }
    }
}