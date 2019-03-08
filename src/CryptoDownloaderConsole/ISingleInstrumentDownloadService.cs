using System.Threading;

namespace CryptoDownloader
{
    public interface ISingleInstrumentDownloadService
    {
        /// <summary>
        /// Download and save data of 1 instrument.
        /// </summary>
        /// <param name="instrument"></param>
        /// <param name="batchNumber"></param>
        /// <param name="outputDirectory"></param>
        /// <param name="cancelToken"></param>
        void DownloadAndSaveOneInstrument(string instrument, int batchNumber, string outputDirectory,
            CancellationToken cancelToken);
    }
}