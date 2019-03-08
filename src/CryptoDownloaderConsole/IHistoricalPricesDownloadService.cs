using System.Collections.Generic;
using System.Threading;

namespace CryptoDownloader
{
    public interface IHistoricalPricesDownloadService
    {
        /// <summary>
        /// Download and save data of many instruments.
        /// </summary>
        /// <param name="instruments"></param>
        /// <param name="batchNumber"></param>
        /// <param name="outputDirectory"></param>
        /// <param name="cancelToken"></param>
        void DownloadAndSave(IEnumerable<string> instruments, int batchNumber, 
            string outputDirectory, CancellationToken cancelToken);
    }
}