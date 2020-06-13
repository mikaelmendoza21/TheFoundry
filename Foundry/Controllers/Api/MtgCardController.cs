using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChiefOfTheFoundry.DataAccess;
using ChiefOfTheFoundry.Models;
using ChiefOfTheFoundry.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoundryApi.Api.Controllers
{
    [Route("api/mtgcards")]
    [ApiController]
    public class MtgCardController : Controller
    {
        private readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IMetaCardAccessor _metaCardAccessor;
        private readonly IMtgSetAccessor _setAccessor;
        private readonly IMtgCardAccessor _mtgCardAccessor;
        private readonly ICardConstructAccessor _cardConstructAccesor;
        private readonly CardManagerService _cardManagerService;

        public MtgCardController(IMetaCardAccessor metaCardAccessor,
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

        [HttpGet]
        [Route("byMetacardAndSet")]
        public IActionResult GetMtgCardByMetacardAndSet(string metacardId, string setId)
        {
            MtgCard cardFound = _cardManagerService.GetMtgCardByMetacardAndSet(metacardId, setId);
            if (cardFound != null) 
            {
                return Json(cardFound);
            }
            else
            {
                return BadRequest($"No card found for metacard Id {metacardId} in set with Id {setId}");
            }
        }
    }
}
