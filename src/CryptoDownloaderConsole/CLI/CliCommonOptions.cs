using System;
using CommandLine;

namespace CryptoDownloader
{
    public class CliCommonOptions
    {
        [Option('d',"debug", HelpText="Set to truePath to enable debug output")]
        public bool Debug { get; set; }
    }
}