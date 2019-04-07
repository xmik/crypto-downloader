using System;
using System.IO;
using System.Threading;
using Autofac;
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
            
            string seriesDir = Environment.CurrentDirectory + "/tests/ShouldCreateSpecifiedTimeSeries";
            Cleanup(seriesDir);
            CancellationTokenSource cts = new CancellationTokenSource();
            b.RegisterType<SingleInstrumentDownloadService>().As<ISingleInstrumentDownloadService>();

            using (var container = b.Build())
            {
                var historicalService = container.Resolve<IHistoricalPricesDownloadService>();
                var instruments = new string[]{"USDC_BTC"};
                historicalService.DownloadAndSave(instruments, 1, seriesDir, cts.Token);

                string filePath = seriesDir + "/1/USDC_BTC.csv";
                Assert.False (File.Exists (filePath));
                var fileContents = File.ReadAllText(filePath);
                // assert that there are 2016 candle events written to file (file has 2016 lines)
                Assert.Equal(2016, fileContents.Split('\n').Length);
                // do not check the file contents, because after some time
                // the data will be not available on Poloniex and this test would fail
            }
            // directory was created, because Batch Metadata Service created it
            Assert.True(Directory.Exists (seriesDir));
            Cleanup(seriesDir);
        }
    }
}