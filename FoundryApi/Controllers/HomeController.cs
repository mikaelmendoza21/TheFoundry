using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using FoundryApi.Models;

namespace FoundryApi.Controllers
{
    [ApiController]
    [Route("foundry")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IConfiguration _configuration;

        public HomeController(IConfiguration Configuration, ILogger<HomeController> logger)
        {
            _configuration = Configuration;
            _logger = logger;
        }

        [HttpGet]
        [Route("searchToAdd")]
        public IActionResult SearchCard()
        {
            BaseViewModel model = new BaseViewModel(_configuration);
            return View(model);
        }

        [HttpGet]
        [Route("reviewToAdd")]
        public IActionResult ReviewBeforeAdding(string cardName, string mtgCardId)
        {
            if (string.IsNullOrEmpty(cardName) || string.IsNullOrEmpty(mtgCardId))
            {
                return BadRequest($"Invalid Card Inf. Card name {cardName}.");
            }

            ViewBag.CardName = cardName;
            ViewBag.MtgCardId = mtgCardId;

            BaseViewModel model = new BaseViewModel(_configuration);
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            BaseViewModel model = new BaseViewModel(_configuration);
            return View(model);
        }
    }
}
