using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UsersMatcher.Models;

namespace UsersMatcher.Logic
{
    public class LastFmClient
    {
        private static readonly HttpClient httpClient = new HttpClient()
        {
            BaseAddress = new Uri("https://ws.audioscrobbler.com/2.0")
        };

        private readonly string apiKey;

        public LastFmClient(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public async Task<IEnumerable<Album>> GetTopNAlbumsAsync(string username, int n) =>
            await GetAllAsync<TopAlbums, Album>(username, "user.gettopalbums", n);

        public async Task<IEnumerable<User>> GetAllFriendsAsync(string username) =>
            await GetAllAsync<Friends, User>(username, "user.getfriends");

        private async Task<IEnumerable<U>> GetAllAsync<T, U>(string username, string apiMethod, int maxCount = int.MaxValue)
            where T : class, ILastFmJsonResponse<U>
        {
            List<U> resultList = new List<U>();

            var query = $"?method={apiMethod}&user={username}&api_key={apiKey}&format=json";
            int page = 0;
            int perPage = 0;
            int totalPages = 0;
            const int okStatusCode = (int)HttpStatusCode.OK;
            do
            {
                page++;
                var jsonPageRequestResult = await RequestJsonPageAsync<T, U>(query, page);
                if (jsonPageRequestResult.statusCode != okStatusCode)
                {
                    throw new LastFmApiError
                    {
                        StatusCode = jsonPageRequestResult.statusCode,
                        Reason = jsonPageRequestResult.reason
                    };
                }     
                var jsonPage = jsonPageRequestResult.body;
                totalPages = jsonPage.Attributes.TotalPages;
                perPage = jsonPage.Attributes.PerPage;
                resultList.AddRange(jsonPage.Content);
            } while (page < totalPages && resultList.Count < maxCount - perPage);

            return resultList;           
        }

        private async Task<ApiResponse<T>> RequestJsonPageAsync<T, U>(string query, int page)
            where T : class, ILastFmJsonResponse<U>
        {
            string content;
            T body = default(T);
            var response = await httpClient.GetAsync(query + $"&page={page}");
            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();
                JObject jObject = (JObject)JsonConvert.DeserializeObject(content);
                body = jObject.First.First.ToObject<T>();
            }
            return new ApiResponse<T>
            {
                statusCode = (int)response.StatusCode,
                reason = response.ReasonPhrase,
                body = body,
            };
        }

        private struct ApiResponse<T>
        {
            public int statusCode;

            public string reason;

            public T body;
        }
    }
}
