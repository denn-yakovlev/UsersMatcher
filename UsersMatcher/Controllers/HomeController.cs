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
using UsersMatcher.Logic;

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
        public async Task<ViewResult> HandleFormAsync(Form form)
        {
            UsersMatcherMain.TargetUserName = form.Name;
            UsersMatcherMain.Metric = SimilarityMetrices.Tanimoto;
            var resultTask = UsersMatcherMain.GetSimilarity();
            try
            {
                return View("_Result", await resultTask);
            }
            catch (LastFmApiError err)
            {
                return View("_Error", err);
            }
        }

    }
}