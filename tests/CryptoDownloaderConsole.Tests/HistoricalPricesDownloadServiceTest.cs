using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Autofac;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace CryptoDownloader.Tests
{
    public class HistoricalPricesDownloadServiceTest
    {
        private void Cleanup(string directoryPath)
        {
			if (Directory.Exists (directoryPath))
				Directory.Delete (directoryPath, true);
        }

        /// <summary>
        /// No data are saved to files here, because the service responsible for data saving is mocked.
        /// </summary>
        [Fact]
        public void ShouldInvokeExchangeWithCorrectDateRanges()
        {
            string seriesDir = Environment.CurrentDirectory + "/tests/ShouldInvokeExchangeWithCorrectDateRanges";
            Mock<IExchange> exchangeService = new Mock<IExchange>(MockBehavior.Loose);
            var batchNumberService = new BatchNumberService();
            var filePathService = new FilePathService();

            CancellationTokenSource cts = new CancellationTokenSource();
            Mock<IEventSavingService> savingService = new Mock<IEventSavingService>(MockBehavior.Loose);
            var downloader = new SingleInstrumentDownloadService(exchangeService.Object,
                batchNumberService, filePathService, savingService.Object);
            var service = new HistoricalPricesDownloadService(downloader);

            var instruments = new string[]{"BTC_USD","ETC_USD"};
            service.DownloadAndSave(instruments, 12, seriesDir, cts.Token);

            exchangeService.Verify(
                g => g.GetHistoricalPrices("BTC_USD", 300, 
                // batch12 start - 15 minutes
                DateTimeExtensions.CreateNodaTime(2019,4,27,23,45,0),
                // batch12 end + 15 minutes
                DateTimeExtensions.CreateNodaTime(2019,5,5,0,14,59)
                ),
                Times.Exactly(1)
            );
            exchangeService.Verify(
                g => g.GetHistoricalPrices("ETC_USD", 300, 
                DateTimeExtensions.CreateNodaTime(2019,4,27,23,45,0),
                DateTimeExtensions.CreateNodaTime(2019,5,5,0,14,59)
                ),
                Times.Exactly(1)
            );
            exchangeService.VerifyNoOtherCalls();
            Assert.False (File.Exists (String.Format("{0}/12/BTC_USD.csv",seriesDir)));
        }

        [Fact]
        public void ShouldResolveHistoricalServiceFromContainer_WhenDryrunDownloader()
        {
            ContainerBuilder b = new ContainerBuilder();
            b.RegisterModule<CryptoDownloaderAutofacModule>();
            b.RegisterType<SingleInstrumentDownloadServiceDryRun>().As<ISingleInstrumentDownloadService>();
            using (var c = b.Build())
            {
                var historicalService = c.Resolve<IHistoricalPricesDownloadService>();
            }
        }

        [Fact]
        public void ShouldResolveHistoricalServiceFromContainer_WhenProductionDownloader()
        {
            ContainerBuilder b = new ContainerBuilder();
            b.RegisterModule<CryptoDownloaderAutofacModule>();
            b.RegisterType<SingleInstrumentDownloadService>().As<ISingleInstrumentDownloadService>();
            using (var c = b.Build())
            {
                var historicalService = c.Resolve<IHistoricalPricesDownloadService>();
            }
        }

        /// <summary>
        /// No data are saved to files here, because we use the dryrun implementation of 
        /// the service responsible for data saving.
        /// </summary>
        [Fact]
        public void ShouldNotNeedManyServices_WhenDummyDownloaderImplementation()
        {
            string seriesDir = Environment.CurrentDirectory +
                 "/tests/ShouldNotNeedManyServices_WhenDummyDownloaderImplementation";
            var batchNumberService = new BatchNumberService();
            CancellationTokenSource cts = new CancellationTokenSource();
            var downloader = new SingleInstrumentDownloadServiceDryRun(batchNumberService);
            var service = new HistoricalPricesDownloadService(downloader);

            var instruments = new string[]{"BTC_USD","ETC_USD"};
            service.DownloadAndSave(instruments, 12, seriesDir, cts.Token);
            Assert.False (File.Exists (String.Format("{0}/12/BTC_USD.csv",seriesDir)));
            Assert.False (File.Exists (String.Format("{0}/12/ETC_USD.csv",seriesDir)));
        }
    }
}