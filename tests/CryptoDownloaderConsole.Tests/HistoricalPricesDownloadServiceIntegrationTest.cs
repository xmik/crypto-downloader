using System;
using System.IO;
using System.Threading;
using Autofac;
using CryptoDownloaderConsole;
using Moq;
using Xunit;

namespace CryptoDownloader.Tests
{
    [Trait("Category","integration")]
    public class HistoricalPricesDownloadServiceIntegrationTest
    {
        private void Cleanup(string directoryPath)
        {
			if (Directory.Exists (directoryPath))
				Directory.Delete (directoryPath, true);
        }
             

        [Fact]
        public void ShouldAppendEventsToTimeSeries()
        {
            ContainerBuilder b = new ContainerBuilder();
            b.RegisterModule<CryptoDownloaderAutofacModule>();
            
            string seriesDir = Path.Combine(Environment.CurrentDirectory, "tests", "ShouldCreateSpecifiedTimeSeries");
            Cleanup(seriesDir);
            CancellationTokenSource cts = new CancellationTokenSource();
            b.RegisterType<SingleInstrumentDownloadService>().As<ISingleInstrumentDownloadService>();

            using (var container = b.Build())
            {
                var batchNumberService = container.Resolve<IBatchNumberService>();
                int batchNumber = batchNumberService.GetBatchNumberToBeDownloaded(
                    DateTime.Now.ToUniversalTime().ToNodaTime());

                var historicalService = container.Resolve<IHistoricalPricesDownloadService>();
                var instruments = new string[]{"USDC_BTC"};
                historicalService.DownloadAndSave(instruments, batchNumber, seriesDir, cts.Token);

                string filePath = Path.Combine(seriesDir, batchNumber.ToString(), "USDC_BTC.csv");
                Assert.True (File.Exists (filePath));
                var fileContents = File.ReadAllText(filePath);
                // assert that there are 2016 candle events written to file (file has 2017 lines, 1 is a header)
                Assert.Equal(2017, fileContents.SplitByNewLine().Length);
                // do not check the file contents, because after some time
                // the data will be not available on Poloniex and this test would fail
            }
            // directory was created, because Batch Metadata Service created it
            Assert.True(Directory.Exists (seriesDir));
            Cleanup(seriesDir);
        }
    }
}