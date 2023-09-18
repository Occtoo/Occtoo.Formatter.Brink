using AutoMapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Occtoo.Formatter.Brink.Models;
using Occtoo.Formatter.Brink.Services;
using System.Threading.Tasks;

namespace Occtoo.Formatter.Brink.Functions.Price
{
    public class PriceQueueFunction
    {
        private readonly IOcctooApiService<OcctooPriceModel> _occtooApiService;
        private readonly IBrinkApiService _brinkApiService;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;

        public PriceQueueFunction(IOcctooApiService<OcctooPriceModel> occtooApiService, IBrinkApiService brinkApiService, IMapper mapper, AppSettings appSettings)
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
        [FunctionName(nameof(PriceQueueFunction))]
        public async Task Run([QueueTrigger("%priceQueue%", Connection = "AzureWebJobsStorage")] string productId, ILogger log)
        {
            var prices = await _occtooApiService.FetchBatchFromApi(_appSettings.PriceApiUrl, productId);

            foreach (var price in prices)
            {
                await _brinkApiService.PutPrice(price);
            }
        }
    }
}
