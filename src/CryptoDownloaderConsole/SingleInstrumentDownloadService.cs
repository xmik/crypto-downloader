using System.Threading;

namespace CryptoDownloader
{
    /// <summary>
    /// End user class to download historical prices data for one instrument.
    /// </summary>
    public class SingleInstrumentDownloadService : ISingleInstrumentDownloadService
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger (typeof(SingleInstrumentDownloadService));
        private readonly IExchange exchange;
        private readonly IBatchNumberService batchService;
        private readonly IFilePathService filePathService;
        private readonly IEventSavingService seriesAppendingService;
        private readonly int timeframeSeconds = 300;
        public SingleInstrumentDownloadService(IExchange exchange, IBatchNumberService batchService,
            IFilePathService filePathService, IEventSavingService seriesAppendingService)
        {
            this.exchange = exchange;
            this.batchService = batchService;
            this.filePathService = filePathService;
            this.seriesAppendingService = seriesAppendingService;
        }

        public void DownloadAndSaveOneInstrument(string instrument, int batchNumber, string outputDirectory, CancellationToken cancelToken)
        {
            var batchDateRange = this.batchService.GetDataRange(batchNumber);
            _log.InfoFormat("{0} batch date time range: {1}", instrument, batchDateRange);
            var safeBatchDateRange = this.batchService.GetWiderBatchDateRange(batchDateRange);
            string instrumentFilePath = this.filePathService.GetTimeSeriesFilePath(
                outputDirectory, batchNumber, instrument);

            _log.InfoFormat("{0} downloading data covering safe date time range: {1}", instrument, safeBatchDateRange);
            var candleEvents = exchange.GetHistoricalPrices(
                instrument, timeframeSeconds, safeBatchDateRange.Start, safeBatchDateRange.End);
            _log.InfoFormat("{0} downloaded {1} events", instrument, candleEvents.Length);

            seriesAppendingService.Write(candleEvents, instrument, cancelToken, batchDateRange);
        }
    }
}