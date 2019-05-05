using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Linq;
using System.Globalization;

namespace CryptoDownloader
{
    public class EventSavingService : IEventSavingService
    {
		private static readonly log4net.ILog _log = log4net.LogManager.GetLogger (typeof(EventSavingService));

        public EventSavingService()
        {        }

        public NodaTime.Instant ParseDateTime(string line, string dateTimePattern)
        {
            DateTime dt = DateTime.ParseExact(line, dateTimePattern, CultureInfo.InvariantCulture);
            // specify that the datetime saved in file uses UTC timezone
            DateTime ut = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            return DateTimeExtensions.ToNodaTime(ut.ToUniversalTime());
        }

        public int Write(CandleEvent[] candleEvents, string filePath, CancellationToken ct, IRange<NodaTime.Instant> allowedDateRange)
        {
			int eventsWrittenCount = 0;
            Boolean newfile = false;
            NodaTime.Instant lastTimestamp;
            string dateTimePattern = "yyyy-MM-ddTHH:mm:ss";

            if (File.Exists(filePath))
            { //if file exists read the time stamp of the last entry
                string lastline = File.ReadLines(filePath, Encoding.UTF8).Last();
                lastline = lastline.Substring(0, lastline.IndexOf(','));
                lastTimestamp = ParseDateTime(lastline, dateTimePattern);
            }
            else
            { //if the file doesn't exist create one and add the header with information about file content
                newfile = true;
                string dirPath = Path.GetDirectoryName(filePath);
                Directory.CreateDirectory(dirPath);
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, false))
                {
                    file.Write("<date time>,<open price>,<high price>,<low price>,<close price>");
                }
                lastTimestamp = NodaTime.Instant.FromUnixTimeTicks(0);
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, true))
            {
                foreach (var ce in candleEvents) //save all candle events to file
                {
                    if(ct.IsCancellationRequested) break; //if cancelled stop the loop

                    var candleTime = NodaTime.Instant.FromUnixTimeTicks(ce.Date);
                    if (candleTime > lastTimestamp && allowedDateRange.Includes(candleTime)) {
                        // the current event from candleEvents has later date then the last event
                        // already saved to a file AND
                        // the date is included in the allowedDateRange
                        string candleValue = ce.Value.ToString();
                        candleValue = candleValue.Replace(" ", "");
                        file.Write("\n{0},{1}",
                            candleTime.ToDateTime().ToString(dateTimePattern),
                            candleValue);
                        file.Flush();
                        eventsWrittenCount++;
                        lastTimestamp = candleTime;
                    }
                }
            }
            if(ct.IsCancellationRequested){
                if(newfile) File.Delete(filePath); //if cancellation and file didn't exist before delete it entirely 
                else { //if cancellation and file existed before delete appended lines
                    string [] filecontent = File.ReadAllLines(filePath);
                    Array.Resize(ref filecontent, filecontent.Length - eventsWrittenCount);
                    File.Delete(filePath); //delete file
                    File.WriteAllLines(filePath, filecontent); //recreate file with lines before program action
                }
            }
            _log.DebugFormat("Finished writing to series, added: {0} events", eventsWrittenCount);
            return eventsWrittenCount;
        }
    }
}