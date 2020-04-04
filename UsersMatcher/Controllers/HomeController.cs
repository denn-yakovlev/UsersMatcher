using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using UsersMatcher.Models;

namespace UsersMatcher.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View("Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> HandleForm(User user)
        {
            var client = new HttpClient();
            var apiKey = "48cdcb584772ebb3935f8f6289dc9d0b";
            UriBuilder builder = new UriBuilder()
            {
                Host = "ws.audioscrobbler.com",
                Path = "2.0",
                Query = $"method=user.gettopalbums&user={user.Name}&api_key={apiKey}&format=json"
            };
            HttpResponseMessage response = await client.GetAsync(builder.Uri);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return $"{response.StatusCode}";
            };
        }
    }
}