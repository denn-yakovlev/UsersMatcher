using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using UsersMatcher.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace UsersMatcher.Controllers
{

    public class HomeController : Controller
    {

        private class LastFmClient
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

            public async Task<ApiResponse<IList<Album>>> GetAllTopAlbumsAsync(string username) =>
                await GetAllAsync<TopAlbums, Album>(username, "user.gettopalbums");
            
            public async Task<ApiResponse<IList<User>>> GetAllFriendsAsync(string username) => 
                await GetAllAsync<Friends, User>(username, "user.getfriends");

            private async Task<ApiResponse<IList<U>>> GetAllAsync<T, U>(string username, string apiMethod)
                where T: class, ILastFmJsonResponse<U>
            {
                List<U> resultList = null;

                var query = $"?method={apiMethod}&user={username}&api_key={apiKey}&format=json";
                var jsonPageRequestResult = await RequestJsonPageAsync<T, U>(query);

                const int okStatusCode = (int)HttpStatusCode.OK;
                if (jsonPageRequestResult.statusCode == okStatusCode)
                {
                    var jsonPage = jsonPageRequestResult.body;

                    resultList = jsonPage.Content;
                    var totalPages = jsonPage.Attributes.TotalPages;
                    var page = 2;
                    while (jsonPageRequestResult.statusCode == okStatusCode
                        && page <= totalPages)
                    {
                        jsonPageRequestResult = await RequestJsonPageAsync<T, U>(query, page);
                        jsonPage = jsonPageRequestResult.body;
                        resultList.AddRange(jsonPage.Content);
                        page++;
                    }
                }

                return new ApiResponse<IList<U>>
                {
                    statusCode = jsonPageRequestResult.statusCode,
                    reason = jsonPageRequestResult.reason,
                    body = resultList,
                };
            }
            
            private async Task<ApiResponse<T>> RequestJsonPageAsync<T, U>(string query, int page = 1)
                where T: class, ILastFmJsonResponse<U>
            {
                var response = await httpClient.GetAsync(query);
                string content;
                T body = default(T);
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
        }

        private struct ApiResponse<T>
        {
            public int statusCode;

            public string reason;

            public T body;
        }


        public IActionResult Index()
        {
            return View("Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<PartialViewResult> HandleFormAsync(Form form)
        {
            var apiKey = "48cdcb584772ebb3935f8f6289dc9d0b";
            var lastFmClient = new LastFmClient(apiKey);
            var albumsResponse = await lastFmClient.GetAllTopAlbumsAsync(form.Name);
            //return albumsResponse.body?.ToString() ?? "NULL";
            if (albumsResponse.body != null)
            {
                var friendsResponse = await lastFmClient.GetAllFriendsAsync(form.Name);
                if (friendsResponse.body != null)
                {
                    foreach (var friend in friendsResponse.body)
                    {

                    }

                }

            }
            return PartialView("_Result", "lalala that's a model"); 
        }

    }
}