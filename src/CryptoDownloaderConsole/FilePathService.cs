using System;
using System.Text;

namespace CryptoDownloader
{
    public class FilePathService : IFilePathService
    {
        public string GetTimeSeriesFilePath(string outputDirectory, int batchNumber, string instrument)
        {
            instrument = instrument.Replace(' ', '_');
            return String.Format("{0}/{1}/{2}.csv", outputDirectory, batchNumber, instrument);
        }
    }
}