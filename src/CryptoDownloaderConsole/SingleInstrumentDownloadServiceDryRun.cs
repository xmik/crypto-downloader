using System.Threading;

namespace CryptoDownloader
{
    public class SingleInstrumentDownloadServiceDryRun : ISingleInstrumentDownloadService
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger (typeof(SingleInstrumentDownloadServiceDryRun));
        private readonly IBatchNumberService batchService;

        public SingleInstrumentDownloadServiceDryRun(IBatchNumberService batchService)
        {
            this.batchService = batchService;
        }
        public void DownloadAndSaveOneInstrument(string instrument, int batchNumber, string outputDirectory, CancellationToken cancelToken)
        {
            var batchDateRange = this.batchService.GetDataRange(batchNumber);
            _log.InfoFormat("{0} batch date time range: {1}", instrument, batchDateRange);
            var safeBatchDateRange = this.batchService.GetWiderBatchDateRange(batchDateRange);

            _log.InfoFormat("{0} downloading data covering safe date time range: {1}", instrument, safeBatchDateRange);
        }
    }
}