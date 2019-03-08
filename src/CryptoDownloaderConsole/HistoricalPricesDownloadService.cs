using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CryptoDownloader
{
    /// <summary>
    /// End user class to download historical prices data for many instruments.
    /// </summary>
    public class HistoricalPricesDownloadService : IHistoricalPricesDownloadService
    {
		private static readonly log4net.ILog _log = log4net.LogManager.GetLogger (typeof(HistoricalPricesDownloadService));
        private readonly ISingleInstrumentDownloadService downloadService;

        public HistoricalPricesDownloadService(ISingleInstrumentDownloadService downloadService)
        {
            this.downloadService = downloadService;
        }

        public void DownloadAndSave(IEnumerable<string> instruments, int batchNumber, string outputDirectory,
            CancellationToken cancelToken)
        {
            var instrumentsArr = instruments.ToArray();
            string instrumentsStr = "";            
            for (int i=0; i< instrumentsArr.Length; i++)
            {
                instrumentsStr += instrumentsArr[i];
            }
            _log.InfoFormat("Will download batch: {0}, instruments: {1}", batchNumber, instrumentsStr);

            for (int i=0; i< instrumentsArr.Length; i++)
            {
                string instrument = instrumentsArr[i];
                _log.InfoFormat("Downloading instrument {0}/{1}: {2}", i+1, instrumentsArr.Length, instrument);
                this.downloadService.DownloadAndSaveOneInstrument(
                    instrument, batchNumber, outputDirectory, cancelToken);
            }
            _log.InfoFormat("Finished download with success");
        }

    }
}