using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChiefOfTheFoundry.DataAccess;
using ChiefOfTheFoundry.Models;
using ChiefOfTheFoundry.Services;
using Foundry.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Foundry.Controllers
{
    [Route("collection")]
    public class CollectionManagerController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IMetaCardAccessor _metaCardAccessor;
        private readonly IMtgCardAccessor _mtgCardAccessor;
        private readonly ICardManagerService _cardManagerService;
        private readonly ISetManagerService _setManagerService;

        public CollectionManagerController(IConfiguration configuration,
            IMetaCardAccessor metaCardAccessor,
            IMtgCardAccessor mtgCardAccessor,
            ICardManagerService cardManagerService,
            ISetManagerService setManagerService)
        {
            _configuration = configuration;
            _metaCardAccessor = metaCardAccessor;
            _mtgCardAccessor = mtgCardAccessor;
            _cardManagerService = cardManagerService;
            _setManagerService = setManagerService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("SearchCollection");
        }

        [HttpGet]
        [Route("search")]
        public IActionResult SearchCollection()
        {
            BaseViewModel model = new BaseViewModel(_configuration);
            return View(model);
        }

        [HttpGet]
        [Route("copiesInCollection")]
        public IActionResult GetCardCopiesInCollection(string metacardId)
        {
            MetaCard metaCard = _metaCardAccessor.GetMetaCardById(metacardId);
            ViewBag.Metacard = metaCard;

            BaseViewModel model = new BaseViewModel(_configuration);
            return View(model);
        }
    }
}
