using Newtonsoft.Json;
using Occtoo.Formatter.Brink.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Occtoo.Formatter.Brink.Services
{
    public interface IOcctooApiService<T>
    {
        Task<IEnumerable<T>> FetchBatchFromApi(string destinationEndPoint, string productNumber);
        Task<IEnumerable<string>> FetchAllIdsFromApi(string destionationEndPoint);
    }

    public class OcctooApiService<T> : IOcctooApiService<T>
    {
        private readonly AppSettings _appSettings;
        private readonly HttpClient _httpClient;
        private DateTime? accessTokenExpiration;
        private readonly Dictionary<string, string> body;

        public OcctooApiService(AppSettings appSettings, HttpClient httpClient)
        {
            _appSettings = appSettings;
            _httpClient = httpClient;
            body = new Dictionary<string, string>
             {
                {"clientId", _appSettings.OcctooClientId},
                {"clientSecret", _appSettings.OcctooClientSecret }
            };
        }

        public async Task SetToken()
        {
            if (accessTokenExpiration == null || DateTime.UtcNow >= accessTokenExpiration)
            {

                var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(
                    _appSettings.OcctooTokenAuthUrl,
                    content);
                response.EnsureSuccessStatusCode();
                var token = await response.Content.ReadAsAsync<OcctooTokenModel>();
                accessTokenExpiration = DateTime.UtcNow.AddSeconds(token.ExpiresIn - 500);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            }
        }

        public async Task<IEnumerable<string>> FetchAllIdsFromApi(string destinationEndPoint)
        {
            //empty entry important here
            var allIds = new List<string> { "" };
            var currentIds = await GetIds(destinationEndPoint, "", "en");

            while (currentIds != null && currentIds.Any())
            {
                var lastProduct = currentIds.Last();
                allIds.Add(lastProduct);
                currentIds = await GetIds(destinationEndPoint, lastProduct, "en");
            }

            return allIds;
        }

        public async Task<IEnumerable<T>> FetchBatchFromApi(string destinationEndPoint, string productNumber)
        {
            await SetToken();
            var content = await GetContent(destinationEndPoint, productNumber, "en");
            return content;
        }

        private async Task<IEnumerable<string>> GetIds(string fullDestinationApi, string lastIdPrevBatch, string language)
        {
            var queryString = $"?top={_appSettings.BatchSize}&language={language}&sortAsc=id&listIds=true";
            if (!string.IsNullOrEmpty(lastIdPrevBatch))
            {
                queryString = $"?top={_appSettings.BatchSize}&language={language}&sortAsc=id&listIds=true&after={lastIdPrevBatch}";
            }

            await SetToken();
            var response = await _httpClient.GetFromJsonAsync<DestinationResponseModel<string>>($"{fullDestinationApi}{queryString}");

            return response?.Results;
        }

        private async Task<IEnumerable<T>> GetContent(string fullDestinationApi, string lastIdPrevBatch, string language)
        {
            var queryString = $"?top={_appSettings.BatchSize}&language={language}&sortAsc=id";
            if (!string.IsNullOrEmpty(lastIdPrevBatch))
            {
                queryString = $"?top={_appSettings.BatchSize}&language={language}&sortAsc=id&after={lastIdPrevBatch}";
            }

            await SetToken();
            var response = await _httpClient.GetFromJsonAsync<DestinationResponseModel<T>>($"{fullDestinationApi}{queryString}");

            return response?.Results;
        }
    }
}
