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
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.Threading;

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

            public async Task<ApiResponse<IList<Album>>> GetTopNAlbumsAsync(string username, int n) =>
                await GetAllAsync<TopAlbums, Album>(username, "user.gettopalbums", n);
            
            public async Task<ApiResponse<IList<User>>> GetAllFriendsAsync(string username) => 
                await GetAllAsync<Friends, User>(username, "user.getfriends");

            private async Task<ApiResponse<IList<U>>> GetAllAsync<T, U>(string username, string apiMethod, int maxCount = int.MaxValue)
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
                    int perPage = jsonPage.Attributes.PerPage;
                    while (page <= totalPages
                        && resultList.Count <= maxCount - perPage)
                    {
                        jsonPageRequestResult = await RequestJsonPageAsync<T, U>(query, page);

                        if (jsonPageRequestResult.statusCode == okStatusCode)
                        {
                            jsonPage = jsonPageRequestResult.body;
                            resultList.AddRange(jsonPage.Content);
                            page++;
                        }
                        else break;                        
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
                var response = await httpClient.GetAsync(query + $"&page={page}");
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

        private static class Similarity
        {
            public static double Tanimoto<T> (ICollection<T> first, ICollection<T> second)
            {
                var intersection = first.Intersect(second);
                var intersectionSize = intersection.Count();
                var union = first.Union(second);
                return (double)intersectionSize / (union.Count() - intersectionSize);
            }
        }

        public IActionResult Index()
        {
            return View("Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ViewResult> HandleFormAsync(Form form)
        {
            var apiKey = "48cdcb584772ebb3935f8f6289dc9d0b";
            var lastFmClient = new LastFmClient(apiKey);
            var albumsResponse = await lastFmClient.GetTopNAlbumsAsync(form.Name, 100);
            var similarityResults = new ConcurrentDictionary<User, double>();
            if (albumsResponse.statusCode == 200)
            {
                var albums = albumsResponse.body;
                var friendsResponse = await lastFmClient.GetAllFriendsAsync(form.Name);               
                if (friendsResponse.statusCode == 200)
                {
                    var friends = friendsResponse.body;
                    //foreach (var friend in friends)
                    //{

                    //}
                    var locker = new object();   
                    Task<ApiResponse<IList<Album>>> task;
                    var tasks = friends.AsParallel().WithDegreeOfParallelism(5).Select(async friend =>
                    {                     
                        lock (locker)
                        {
                            task = lastFmClient.GetTopNAlbumsAsync(friend.Name, 100);
                        }
                        var albumsOfFriendResponse = await task;
                        if (albumsOfFriendResponse.statusCode == 200)
                        {
                            var albumsOfFriend = albumsOfFriendResponse.body;
                            similarityResults.TryAdd(friend, Similarity.Tanimoto(albums, albumsOfFriend));                         
                        }
                    });
                    Task.WaitAll(tasks.AsSequential().ToArray());

                }

            } 
            return View("_Result", similarityResults);
        }

    }
}