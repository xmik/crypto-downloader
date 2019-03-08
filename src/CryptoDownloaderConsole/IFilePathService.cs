namespace CryptoDownloader
{
    public interface IFilePathService
    {
        /// <summary>
        /// Returns path to the directory that will contain files with candle data of 1 instrument.
        /// </summary>
        /// <param name="outputDirectory">The main volume directory to keep all the time series for all the instruments</param>
        /// <param name="batchNumber">Batch number to be saved</param>
        /// <param name="instrument">Name of a currency pair on the Poloniex Exchange</param>
        /// <returns></returns>
        string GetTimeSeriesFilePath(string outputDirectory, int batchNumber, string instrument);
    }
}