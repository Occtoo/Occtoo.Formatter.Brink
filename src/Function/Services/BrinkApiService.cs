using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Occtoo.Formatter.Brink.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Occtoo.Formatter.Brink.Services
{
    public interface IBrinkApiService
    {
        Task<bool> PutProduct(ProductModel product);
        Task<bool> PutPrice(OcctooPriceModel price);
        Task<bool> PutStock(OcctooStockModel price);
    }

    public class BrinkApiService : IBrinkApiService
    {
        private readonly AppSettings _appSettings;
        private DateTime? accessTokenExpiration;
        private readonly HttpClient _httpClient;

        public BrinkApiService(AppSettings appSettings, HttpClient httpClient)
        {
            _appSettings = appSettings;
            _httpClient = httpClient;
        }
        public async Task SetToken()
        {
            if (_httpClient.DefaultRequestHeaders.Authorization == null || accessTokenExpiration == null || DateTime.UtcNow >= accessTokenExpiration)
            {
                // Prepare the request parameters
                var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" }
                });
                requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                string credentials = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{_appSettings.BrinkAccessTokenClientId}:{_appSettings.BrinkAccessTokenSecret}"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                HttpResponseMessage response = await _httpClient.PostAsync(_appSettings.BrinkTokenAuthUrl, requestContent);

                response.EnsureSuccessStatusCode();
                var token = await response.Content.ReadAsAsync<TokenModel>();
                accessTokenExpiration = DateTime.UtcNow.AddSeconds(token.ExpiresIn - 500);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            }
        }

        public async Task<bool> PutProduct(ProductModel product)
        {
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");

            ITraceWriter traceWriter = new MemoryTraceWriter();
            try
            {
                var url = $"{_appSettings.BrinkApiUrl}/product/product-parents/{product.ProductParentId}";
                var urlVariant = $"{_appSettings.BrinkApiUrl}/product/product-variants/{product.ProductParentId + "_test"}";

                await SetToken();

                product.Tags = new Dictionary<string, string>();
                product.Dimensions = new Dimensions();


                var content = new StringContent(
                      JsonConvert.SerializeObject(product),
                      System.Text.Encoding.UTF8,
                      "application/json"
                      );

                var request = new HttpRequestMessage(HttpMethod.Put, url);
                request.Headers.TryAddWithoutValidation("x-api-key", "BrinkCommerceDefaultApiKey");
                request.Content = content;
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                var responseAsString = await (await _httpClient.SendAsync(request)).Content.ReadAsStringAsync();
                if (!responseAsString.Contains("error"))
                {
                    var request2 = new HttpRequestMessage(HttpMethod.Put, urlVariant);
                    request2.Headers.TryAddWithoutValidation("x-api-key", "BrinkCommerceDefaultApiKey");
                    request2.Content = content;
                    request2.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                    var responseAsString2 = await (await _httpClient.SendAsync(request2)).Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<ProductResponse>(responseAsString);
                }
                else
                {
                    var response = JsonConvert.DeserializeObject<ErrorResponse>(responseAsString);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("api call: " + " - " + traceWriter.ToString() + ex.Message);
            }
        }

        public async Task<bool> PutPrice(OcctooPriceModel price)
        {
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");

            ITraceWriter traceWriter = new MemoryTraceWriter();
            try
            {
                var url = $"{_appSettings.BrinkApiUrl}/store-groups/001/product-variants/{price.Id}/prices";
                await SetToken();
                var priceModel = new BrinkPriceModel();
                priceModel.productVariantPrices = new List<ProductVariantPrice>();
                ProductVariantPrice priceVariant = new ProductVariantPrice
                {
                    basePriceAmount = int.Parse(price.Price),
                    countryCode = "SE",
                    referencePriceAmount = null,
                    salePriceAmount = int.Parse(price.Price)
                };

                priceModel.productVariantPrices.Add(priceVariant);

                var content = new StringContent(
                     JsonConvert.SerializeObject(priceModel),
                     System.Text.Encoding.UTF8,
                     "application/json"
                     );

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.TryAddWithoutValidation("x-api-key", "BrinkCommerceDefaultApiKey");
                request.Content = content;
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                var responseAsString = await (await _httpClient.SendAsync(request)).Content.ReadAsStringAsync();
                if (!responseAsString.Contains("error"))
                {
                    var response = JsonConvert.DeserializeObject<ProductResponse>(responseAsString);
                }
                else
                {
                    var response = JsonConvert.DeserializeObject<ErrorResponse>(responseAsString);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("api call: " + " - " + traceWriter.ToString() + ex.Message);
            }
        }

        public async Task<bool> PutStock(OcctooStockModel stock)
        {
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");

            ITraceWriter traceWriter = new MemoryTraceWriter();
            try
            {
                var url = $"{_appSettings.BrinkApiUrl}/stock/product-variants/{stock.Id}/inventories/{stock.Id}";
                await SetToken();

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", "BrinkCommerceDefaultApiKey");

                BrinkStockModel model = new BrinkStockModel();
                model.quantity = int.Parse(stock.Stock);

                var content = new StringContent(
                     JsonConvert.SerializeObject(model),
                     System.Text.Encoding.UTF8,
                     "application/json"
                     );

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.TryAddWithoutValidation("x-api-key", "BrinkCommerceDefaultApiKey");
                request.Content = content;
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                var responseAsString = await (await _httpClient.SendAsync(request)).Content.ReadAsStringAsync();
                if (!responseAsString.Contains("error"))
                {
                    var response = JsonConvert.DeserializeObject<ProductResponse>(responseAsString);
                }
                else
                {
                    var response = JsonConvert.DeserializeObject<ErrorResponse>(responseAsString);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("api call: " + " - " + traceWriter.ToString() + ex.Message);
            }
        }

    }
}
