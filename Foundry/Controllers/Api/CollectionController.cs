using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChiefOfTheFoundry.DataAccess;
using ChiefOfTheFoundry.Models;
using ChiefOfTheFoundry.Models.Inventory;
using ChiefOfTheFoundry.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Foundry.Controllers.Api
{
    [ApiController]
    [Route("api/collection")]
    public class CollectionController : Controller
    {
        private readonly IMetaCardAccessor _metaCardAccessor;
        private readonly IMtgCardAccessor _mtgCardAccessor;
        private readonly ICardConstructAccessor _cardConstructAccessor;
        private readonly IDeckAccessor _deckAccessor;
        private readonly ICardManagerService _cardManagerService;
        private readonly ICollectionManagerService _collectionManagerService;
        private readonly ISetManagerService _setManagerService;

        public CollectionController(
            IMetaCardAccessor metaCardAccessor,
            IMtgCardAccessor mtgCardAccessor,
            ICardConstructAccessor cardConstructAccessor,
            IDeckAccessor deckAccessor,
            ICardManagerService cardManagerService,
            ICollectionManagerService collectionManagerService,
            ISetManagerService setManagerService)
        {
            _metaCardAccessor = metaCardAccessor;
            _mtgCardAccessor = mtgCardAccessor;
            _cardConstructAccessor = cardConstructAccessor;
            _deckAccessor = deckAccessor;
            _cardManagerService = cardManagerService;
            _collectionManagerService = collectionManagerService;
            _setManagerService = setManagerService;
        }

        [HttpPost]
        [Route("addCardConstructs")]
        public JsonResult AddCardCopies(string mtgCardId, int numberOfCopies = 1)
        {
            if (string.IsNullOrEmpty(mtgCardId) || numberOfCopies < 1)
            {
                return Json(new
                {
                    Error = $"invalid request. MtgCardId='{mtgCardId}'. Number of Copies {numberOfCopies}"
                });
            }

            MtgCard mtgCard = _mtgCardAccessor.GetMtgCardById(mtgCardId);
            if (mtgCard == null)
            {
                return Json(new
                {
                    Error = $"Invalid request. MtgCardId='{mtgCardId}'. Number of Copies {numberOfCopies}"
                });
            }

            CardConstruct cardConstruct = new CardConstruct(mtgCard);
            List<CardConstruct> createdConstructs = _cardManagerService.CreateCopiesFromConstruct(cardConstruct, numberOfCopies);

            return Json(createdConstructs);
        }

        [HttpGet]
        [Route("getCardConstructsByMetacardId")]
        public JsonResult GetCardsInCollection(string metacardId)
        {
            if (string.IsNullOrEmpty(metacardId))
            {
                return Json(new
                {
                    Error = $"Invalid request"
                });
            }

            List<CardConstruct> cardConstructs = _cardManagerService.GetCardConstructsFromMetacardId(metacardId).ToList();
            if (cardConstructs.Count <= 0)
            {
                return Json(new List<string>());
            }

            return Json(cardConstructs.OrderBy(c => c.MtgCardId));
        }

        [HttpDelete]
        [Route("deleteCardCopy")]
        public JsonResult DeleteCardConstruct(string constructId)
        {
            CardConstruct cardConstruct = _cardConstructAccessor.GetCardConstruct(constructId);
            if (cardConstruct == null)
            {
                return Json(new
                {
                    Error = "CardConstruct not found."
                });
            }

            _cardConstructAccessor.Delete(cardConstruct);

            return Json(new
            {
                Success = true
            });
        }
    }
}
