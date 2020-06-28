using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Foundry.Models;
using ChiefOfTheFoundry.Models;
using ChiefOfTheFoundry.Services;
using ChiefOfTheFoundry.DataAccess;
using ChiefOfTheFoundry.Models.Inventory;

namespace Foundry.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMetaCardAccessor _metaCardAccessor;
        private readonly IMtgCardAccessor _mtgCardAccessor;
        private readonly ICardManagerService _cardManagerService;
        private readonly ISetManagerService _setManagerService;

        public HomeController(IConfiguration configuration,
            IMetaCardAccessor metaCardAccessor,
            IMtgCardAccessor mtgCardAccessor,
            ICardManagerService cardManagerService,
            ISetManagerService setManagerService,
            ILogger<HomeController> logger)
        {
            _configuration = configuration;
            _metaCardAccessor = metaCardAccessor;
            _mtgCardAccessor = mtgCardAccessor;
            _cardManagerService = cardManagerService;
            _setManagerService = setManagerService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            BaseViewModel model = new BaseViewModel(_configuration);
            return View(model);
        }

        [HttpGet]
        [Route("searchToAdd")]
        public IActionResult SearchCard()
        {
            BaseViewModel model = new BaseViewModel(_configuration);
            return View(model);
        }

        [HttpGet]
        [Route("selectSet")]
        public IActionResult SelectSet(string cardName, string metacardId)
        {
            if (string.IsNullOrEmpty(metacardId))
            {
                return BadRequest("Error: Invalid metacard id");
            }

            MetaCard metaCard = _metaCardAccessor.GetMetaCardById(metacardId);
            if (metaCard == null)
            {
                metaCard = _metaCardAccessor.GetMetaCardByName(cardName);
                if (metaCard == null)
                {
                    return BadRequest($"Error: couldn't find card with name '{cardName}'");
                }
            }

            IEnumerable<MtgSet> sets = _setManagerService.GetSetsById(metaCard.SetIDs.ToArray());
            if (sets == null || sets.Count() <= 0)
            {
                return BadRequest($"No sets found for card {cardName}");
            }

            ViewBag.Sets = sets.ToList();
            ViewBag.MetacardId = metaCard.Id;
            ViewBag.CardName = metaCard.Name;

            BaseViewModel model = new BaseViewModel(_configuration);
            return View(model);
        }

        [HttpGet]
        [Route("reviewToAdd")]
        public IActionResult ReviewBeforeAdding(string metacardId, string setId, string cardName)
        {
            if (string.IsNullOrEmpty(cardName) || string.IsNullOrEmpty(metacardId))
            {
                return BadRequest($"Invalid Card Inf. Card name {cardName}.");
            }

            MetaCard metaCard = _metaCardAccessor.GetMetaCardById(metacardId);
            if (metaCard == null)
            {
                return BadRequest($"Couldn't find card with id {metacardId}");
            }
            MtgCard card = _cardManagerService.GetMtgCardByMetacardAndSet(metacardId, setId);
            if (card == null)
            {
                return BadRequest($"Could not find card for set with id {setId}");
            }

            ViewBag.Card = card;

            BaseViewModel model = new BaseViewModel(_configuration);
            return View(model);
        }

        [HttpGet]
        [Route("addCopies")]
        public IActionResult AddCardCopies(string mtgCardId, int numberOfCopies = 1, string notes = "")
        {
            if (string.IsNullOrEmpty(mtgCardId) || numberOfCopies < 1)
            {
                return BadRequest($"Error: invalid request. MtgCardId='{mtgCardId}'. Number of Copies {numberOfCopies}");
            }

            MtgCard mtgCard = _mtgCardAccessor.GetMtgCardById(mtgCardId);
            if (mtgCard == null)
            {
                return BadRequest($"Error: invalid request. MtgCardId='{mtgCardId}'. Number of Copies {numberOfCopies}");
            }
            
            CardConstruct cardConstruct = new CardConstruct(mtgCard);
            _cardManagerService.CreateCopiesFromConstruct(cardConstruct, numberOfCopies, notes);
            
            ViewBag.Card = mtgCard;
            ViewBag.NumberOfCopies = numberOfCopies;

            BaseViewModel model = new BaseViewModel(_configuration);
            return View(model);
        }

        [Route("error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            BaseViewModel model = new BaseViewModel(_configuration);
            return View(model);
        }
    }
}
