using System.Collections.Generic;
using CommandLine;

namespace CryptoDownloader
{
    [Verb("get-batch-number", HelpText = "Return batch number to download now (current date will not be included)")]
    public class GetBatchNumberOptions : CliCommonOptions {
    }  

    [Verb("get-old-batch-number", HelpText = "Same as get-batch-number -1")]
    public class GetOldBatchNumberOptions : CliCommonOptions {
    }

    [Verb("batch-to-date-range", HelpText = "Check which dateTime span is covered by a specified batch number (this is not an inverse of get-batch-number)")]
    public class BatchToDateRangeOptions : CliCommonOptions {
            [Value(0, Required=true)]
            public int BatchNumber { get; set; }
    }

    [Verb("list", HelpText = "List currency pairs available on the Poloniex Exchange")]
    public class ListOptions : CliCommonOptions {
    }   

    [Verb("get", HelpText = "Download historical prices and save to local disk")]
    public class GetOptions : CliCommonOptions {
        [Option("instruments", Required=true, HelpText="Instruments split by space, e.g. --instruments=XMR_ZEC USDT_ETH,")]
        public IEnumerable<string> Instruments { get; set; }
        
        [Option("batch", Required=true)]
        public int BatchNumber { get; set; }

        [Option("directory", Required=true)]
        public string OutputDirectory { get; set; }

        [Option("dryrun", HelpText="Skip actual task execution", Default=false)]
		public bool DryRun { get; set; }
    }
}