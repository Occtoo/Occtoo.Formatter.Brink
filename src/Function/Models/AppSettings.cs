namespace Occtoo.Formatter.Brink.Models
{
    public class AppSettings
    {
        public string AzureWebJobsStorage { get; set; }
        public string OcctooTokenAuthUrl { get; set; }
        public string ApiUrl { get; set; }
        public string StockApiUrl { get; set; }
        public string PriceApiUrl { get; set; }
        public string OcctooClientId { get; set; }
        public string OcctooClientSecret { get; set; }
        public int BatchSize { get; } = 50;
        public string BrinkApiUrl { get; set; }
        public string BrinkAccessTokenSecret { get; set; }
        public string BrinkAccessTokenClientId { get; set; }
        public string BrinkTokenAuthUrl { get; set; }
    }
}
