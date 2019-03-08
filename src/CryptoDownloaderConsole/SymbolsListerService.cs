namespace CryptoDownloader
{
    /// <summary>
    /// End user class to list symbols (instruments) on an exchange.
    /// </summary>
    public class SymbolsListerService
    {
        private readonly IExchange exchangeApi;

        public SymbolsListerService(IExchange exchangeApi)
        {
            this.exchangeApi = exchangeApi;
        }

        public string[] ListSymbols()
        {
            return this.exchangeApi.ListSymbols();
        }
    }
}