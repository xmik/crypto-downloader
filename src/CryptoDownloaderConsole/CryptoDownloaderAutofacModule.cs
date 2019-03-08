using Autofac;

namespace CryptoDownloader
{
    public class CryptoDownloaderAutofacModule: Module
    {
		protected override void Load (ContainerBuilder builder)
		{
			builder.RegisterType<FilePathService> ().As<IFilePathService> ();
			builder.RegisterType<BatchNumberService> ().As<IBatchNumberService> ();

			builder.RegisterType<HistoricalPricesDownloadService> ().As<IHistoricalPricesDownloadService>();
			builder.RegisterType<ExchangeSharpPoloniex> ().As<IExchange>();
			builder.RegisterType<EventSavingService>().As<IEventSavingService>();
		}
    }
}