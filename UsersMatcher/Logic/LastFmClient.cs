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
    public class LastFmClient: IDisposable
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
            var query = $"?method={apiMethod}&user={username}&api_key={apiKey}&format=json";
            int page = 0;
            int perPage = 0;
            int totalPages = 0;
            var resultList = new List<U>();
            try
            {
                do
                {
                    page++;
                    var jsonPage = await RequestJsonPageAsync<T, U>(query, page);
                    totalPages = jsonPage.Attributes.TotalPages;
                    perPage = jsonPage.Attributes.PerPage;
                    resultList.AddRange(jsonPage.Content);
                } while (page < totalPages && resultList.Count < maxCount - perPage);

                return resultList;
            }
            catch (LastFmApiError exc)
            {
                exc.UserName = username;
                throw;
            }  
        }

        private async Task<T> RequestJsonPageAsync<T, U>(string query, int page)
            where T : class, ILastFmJsonResponse<U>
        {
            T body = default(T);
            var response = await httpClient.GetAsync(query + $"&page={page}");
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                JObject jObject = (JObject)JsonConvert.DeserializeObject(content);
                body = jObject.First.First.ToObject<T>();
                return body;
            }
            else
            {
                var errorObj = JsonConvert.DeserializeAnonymousType(
                    content, new { error = 0, message = ""}
                    );
                throw new LastFmApiError((LastFmErrorCode)errorObj.error, errorObj.message);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    httpClient.Dispose();
                }
                disposedValue = true;
            }
        }
         ~LastFmClient()
        {  
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}