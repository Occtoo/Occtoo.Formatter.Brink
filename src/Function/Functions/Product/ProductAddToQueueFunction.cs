using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Occtoo.Formatter.Brink.Models;
using Occtoo.Formatter.Brink.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Occtoo.Formatter.Brink.Functions.Product
{
    public class ProductAddToQueueFunction
    {
        private readonly IOcctooApiService<OcctooProductResponse> _occtooApiService;
        private readonly AppSettings _appSettings;

        public ProductAddToQueueFunction(IOcctooApiService<OcctooProductResponse> occtooApiService, AppSettings appSettings)
        {
            _occtooApiService = occtooApiService;
            _appSettings = appSettings;
        }

        /// <summary>
        /// This function adds strings to the que. The que can't hold more than 64 bytes of data so be careful what you pass in
        /// Ussually you should just pass an ID and do the rest in the trigger function.
        /// </summary>
        /// <returns></returns>
        [FunctionName(nameof(ProductAddToQueueFunction))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [Queue("%productQueue%"), StorageAccount("AzureWebJobsStorage")] ICollector<string> que,
            ILogger log)
        {
            var results = await _occtooApiService.FetchAllIdsFromApi(_appSettings.ApiUrl);
            foreach (var item in results)
            {
                que.Add(item);
            }

            return new OkObjectResult($"Added to the que. {results.Count()} products");
        }
    }
}
