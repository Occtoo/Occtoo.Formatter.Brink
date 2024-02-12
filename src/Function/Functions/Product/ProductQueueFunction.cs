using AutoMapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Occtoo.Formatter.Brink.Models;
using Occtoo.Formatter.Brink.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Occtoo.Formatter.Brink.Functions.Product
{
    public class ProductQueueFunction
    {
        private readonly IOcctooApiService<OcctooProductResponse> _occtooApiService;
        private readonly IBrinkApiService _brinkApiService;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;

        public ProductQueueFunction(IOcctooApiService<OcctooProductResponse> occtooApiService, IBrinkApiService brinkApiService, IMapper mapper, AppSettings appSettings)
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
        [FunctionName(nameof(ProductQueueFunction))]
        public async Task Run([QueueTrigger("%productQueue%", Connection = "AzureWebJobsStorage")] string productId, ILogger log)
        {
            var products = await _occtooApiService.FetchBatchFromApi(_appSettings.ApiUrl, productId);
            var convertedProducts = _mapper.Map<IEnumerable<ProductModel>>(products).ToList();
            foreach (var product in convertedProducts)
            {
                await _brinkApiService.PutProduct(product);
            }
        }
    }
}
