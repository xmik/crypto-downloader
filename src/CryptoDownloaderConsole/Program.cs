using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Threading;
using Autofac;
using CommandLine;
using log4net;
using log4net.Appender;
using log4net.Config;
using NodaTime;

namespace CryptoDownloader
{
    public class Program
    {
   		  private static readonly log4net.ILog _log = log4net.LogManager.GetLogger (typeof(Program));
        private static CancellationTokenSource cts;

        public static void Main(string[] args) {
            cts = new CancellationTokenSource();
            Console.CancelKeyPress += Console_CancelKeyPress;
            try {
                var result = Parser.Default.ParseArguments<
                    CliCommonOptions,
                    GetBatchNumberOptions,GetOldBatchNumberOptions,BatchToDateRangeOptions,ListOptions,GetOptions>(args);
                result.MapResult(
                    (GetBatchNumberOptions options) => {
                        GetBatchNumber(options);
                        return 0;
                    },
                    (GetOldBatchNumberOptions options) => {
                        GetOldBatchNumber(options);
                        return 0;
                    },
                    (BatchToDateRangeOptions options) => {
                        BatchToDateRange(options);
                        return 0;
                    },
                    (ListOptions options) => {
                        List(options);
                        return 0;
                    },
                    (GetOptions options) => {
                        Get(options);
                        return 0;
                    },
                    errors => 1
                );
                result.WithNotParsed(
                    errors => {
                        if (errors.Count() == 1 && 
                            (errors.First() is HelpRequestedError || errors.First() is HelpVerbRequestedError))
                        {
                            // --help was chosen, do not treat it as error
                            System.Environment.Exit(0);
                        }
                        System.Environment.Exit(1); }
                );
            }
			catch(OperationCanceledException ex) {
				_log.Warn ("Operation was canceled", ex);
				Console.WriteLine ("Interrupted");
				Environment.Exit(5);
			}
			catch(Exception ex) {
				Console.WriteLine ("Program crashed {0}", ex);
				_log.Fatal ("Program crashed",ex);
				Environment.Exit(5);
			}
        }

        public static void ConfigureLogging(bool debug)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            ConsoleAppender appender = new ConsoleAppender();
            appender.Layout = new log4net.Layout.PatternLayout(
                "%date %-5level [%thread] %logger - %message%newline");
            if (debug)
            {
                appender.Threshold = log4net.Core.Level.Debug;
            }
            BasicConfigurator.Configure(logRepository, appender);
        }

        private static void GetBatchNumber(GetBatchNumberOptions opts)
        {
            ConfigureLogging(opts.Debug);
            var batchService = new BatchNumberService();
            var now = SystemClock.Instance.GetCurrentInstant();
            var batchNumber = batchService.GetBatchNumberToBeDownloaded(now);
            Console.WriteLine(batchNumber);
        }

        private static void GetOldBatchNumber(GetOldBatchNumberOptions opts)
        {
            ConfigureLogging(opts.Debug);
            var batchService = new BatchNumberService();
            var now = SystemClock.Instance.GetCurrentInstant();
            var batchNumber = batchService.GetBatchNumberToBeDownloaded(now) -1;
            Console.WriteLine(batchNumber);
        }

        private static void BatchToDateRange(BatchToDateRangeOptions opts)
        {
            ConfigureLogging(opts.Debug);
            var batchService = new BatchNumberService();
            int batchNumber = opts.BatchNumber;
            var dateRange = batchService.GetDataRange(batchNumber);
            Console.WriteLine(dateRange);
        }

        private static void List(ListOptions opts)
        {
            ConfigureLogging(opts.Debug);
            IExchange poloniexApi = new ExchangeSharpPoloniex();
            var listerService = new SymbolsListerService(poloniexApi);
            string[] instruments = listerService.ListSymbols();
            Console.WriteLine(string.Join(" ", instruments));
        }

        private static void Get(GetOptions opts)
        {
            ConfigureLogging(opts.Debug);
            var instruments = opts.Instruments;
            ContainerBuilder b = new ContainerBuilder();
            b.RegisterModule<CryptoDownloaderAutofacModule>();
            if(opts.DryRun)
                b.RegisterType<SingleInstrumentDownloadServiceDryRun>().As<ISingleInstrumentDownloadService>();
            else
                b.RegisterType<SingleInstrumentDownloadService>().As<ISingleInstrumentDownloadService>();

            using (var c = b.Build())
            {
                var historicalService = c.Resolve<IHistoricalPricesDownloadService>();
                historicalService.DownloadAndSave(instruments, opts.BatchNumber, opts.OutputDirectory,
                    cts.Token);
            }
        }
        private static void Console_CancelKeyPress (object sender, ConsoleCancelEventArgs e)
		{
			if (!cts.IsCancellationRequested) {
				cts.Cancel ();
				e.Cancel = true;
			}
		}
    }
}
