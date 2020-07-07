using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChiefOfTheFoundry.DataAccess;
using ChiefOfTheFoundry.Models;
using ChiefOfTheFoundry.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FoundryApi.Api.Controllers
{
    [ApiController]
    [Route("api/metacard")]
    public class MetaCardController : Controller
    {
        private readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IMetaCardAccessor _metaCardAccessor;
        private readonly CardManagerService _cardManagerService;

        public MetaCardController(IMetaCardAccessor metaCardAccessor)
        {
            _metaCardAccessor = metaCardAccessor;
            _cardManagerService = new CardManagerService(_metaCardAccessor, null, null, null);
        }

        [HttpGet]
        public IEnumerable<MetaCard> Get()
        {
            return new List<MetaCard>()
            {
                _metaCardAccessor.GetOneMetaCard()
            };
        }

        [HttpGet]
        public MetaCard Get(string cardName)
        {
            try
            {
                return _metaCardAccessor.GetMetaCardByName(cardName.Trim());
            }
            catch (Exception e)
            {
                logger.Info($"Error fetching card with name '{cardName}'=> {e.Message} - {e.StackTrace}");
                return null;
            }
        }

        [HttpGet]
        public MetaCard GetById(string id)
        {
            try
            {
                return _metaCardAccessor.GetMetaCardById(id);
            }
            catch (Exception e)
            {
                logger.Info($"Error fetching card with id '{id}'=> {e.Message} - {e.StackTrace}");
                return null;
            }
        }

        [HttpGet]
        [Route("byNameStart")]
        public JsonResult GetMetacardsByNameStart(string substring)
        {
            IEnumerable<MetaCard> matches = _cardManagerService.GetMetaCardsByNameBeginning(substring.Trim());
            if (matches.Count() > 0)
            {
                return Json(matches.ToList());
            }
            else
            {
                return Json(new List<string>());
            }
        }

        [HttpGet]
        [Route("byNameStartLite")]
        public IActionResult GetMetacardsByNameStartLite(string substring)
        {
            IEnumerable<MetaCard> matches = _cardManagerService.GetMetaCardsByNameBeginning(substring.Trim());
            if (matches.Count() > 0)
            {
                return Json(matches
                    .Select(c =>
                    new 
                    {
                        c.Id,
                        c.Name,
                        c.ImageUrl,
                        c.SetIDs
                    })
                    .ToList());
            }
            else
            {
                return Json(new List<string>());
            }
        }
    }
}
