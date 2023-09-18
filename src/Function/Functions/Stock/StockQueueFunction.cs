using AutoMapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Occtoo.Formatter.Brink.Models;
using Occtoo.Formatter.Brink.Services;
using System.Threading.Tasks;

namespace Occtoo.Formatter.Brink.Functions.Stock
{
    public class StockQueueFunction
    {
        private readonly IOcctooApiService<OcctooStockModel> _occtooApiService;
        private readonly IBrinkApiService _brinkApiService;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;

        public StockQueueFunction(IOcctooApiService<OcctooStockModel> occtooApiService, IBrinkApiService brinkApiService, IMapper mapper, AppSettings appSettings)
        {
            _occtooApiService = occtooApiService;
            _appSettings = appSettings;
            _mapper = mapper;
            _brinkApiService = brinkApiService;
        }

        /// <summary>
        /// This is where all your business logic should go.
        /// This will trigger whenver you add something to the que.
        /// </summary>
        /// <param name="myQueueItem">This is the item that was passed in the que</param>
        /// <param name="log"></param>
        [FunctionName(nameof(StockQueueFunction))]
        public async Task Run([QueueTrigger("%stockQueue%", Connection = "AzureWebJobsStorage")] string productId, ILogger log)
        {
            var stocks = await _occtooApiService.FetchBatchFromApi(_appSettings.StockApiUrl, productId);
            foreach (var stock in stocks)
            {
                await _brinkApiService.PutStock(stock);
            }
        }
    }
}
