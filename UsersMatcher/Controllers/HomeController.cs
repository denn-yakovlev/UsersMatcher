using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UsersMatcher.Models;
using UsersMatcher.Logic;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace UsersMatcher.Controllers
{

    public class HomeController : Controller
    {
        private IMemoryCache _cache;

        public HomeController(IMemoryCache cache)
        {
            _cache = cache;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ViewResult> HandleFormAsync(Form form)
        {
            UsersMatchResult result;
            bool isCached = _cache.TryGetValue(form.Name, out result);
            if (!isCached)
            {
                UsersMatcherMain.TargetUserName = form.Name;
                UsersMatcherMain.Metric = SimilarityMetrices.Tanimoto;
                try
                {
                    result = await UsersMatcherMain.GetSimilarityAsync();
                    _cache.Set(form.Name, result, new MemoryCacheEntryOptions
                    {
                        // cache entry expires in 10 minutes
                        AbsoluteExpirationRelativeToNow = new TimeSpan(0, 10, 0)
                    });
                }
                catch (LastFmApiError err)
                {
                    return View("_Error", err);
                }

            }
            return View("_Result", result);
        }
    }
}