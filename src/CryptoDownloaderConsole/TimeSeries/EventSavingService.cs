using System;
using System.Threading;

namespace CryptoDownloader
{
    public class EventSavingService : IEventSavingService
    {
		private static readonly log4net.ILog _log = log4net.LogManager.GetLogger (typeof(EventSavingService));

        public EventSavingService()
        {        }

        public int Write(CandleEvent[] candleEvents, string filePath, CancellationToken ct, IRange<NodaTime.Instant> allowedDateRange)
        {
            throw new NotImplementedException();
            _log.DebugFormat("Finished writing to series, added: {0} events", candleEvents.Length);
        }
    }
}