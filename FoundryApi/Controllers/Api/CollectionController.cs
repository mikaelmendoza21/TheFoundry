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
using MtgApiManager.Lib.Core.Exceptions;
using MtgApiManager.Lib.Model;

namespace FoundryApi.Api.Controllers
{
    [ApiController]
    [Route("api/collection")]
    public class CollectionController : Controller
    {
        private readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IMetaCardAccessor _metaCardAccessor;
        private readonly IMtgSetAccessor _setAccessor;
        private readonly IMtgCardAccessor _mtgCardAccessor;
        private readonly ICardConstructAccessor _cardConstructAccesor;
        private readonly CardManagerService _cardManagerService;

        public CollectionController(IMetaCardAccessor metaCardAccessor,
            IMtgSetAccessor setAccessor,
            IMtgCardAccessor mtgCardAccessor,
            ICardConstructAccessor cardConstructAccesor)
        {
            _metaCardAccessor = metaCardAccessor;
            _setAccessor = setAccessor;
            _mtgCardAccessor = mtgCardAccessor;
            _cardConstructAccesor = cardConstructAccesor;
            _cardManagerService = new CardManagerService(_metaCardAccessor, _setAccessor, _mtgCardAccessor, _cardConstructAccesor);
        }

        [HttpPut]
        [Route("card/bySet")]
        public IActionResult AddCardToCollection([FromBody]CardConstruct cardConstruct, int copies = 1)
        {
            try
            {
                List<CardConstruct> createdCards = _cardConstructAccesor.CreateMultipleCopies(cardConstruct, copies);
                return Json(createdCards);
            }
            catch(Exception e)
            {
                logger.Error($"[AddCardToCollection] failed. Error: {e.Message}");
                return StatusCode(500);
            }
        }
    }
}
