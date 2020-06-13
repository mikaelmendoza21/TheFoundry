using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChiefOfTheFoundry.DataAccess;
using ChiefOfTheFoundry.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MtgApiManager.Lib.Core.Exceptions;

namespace FoundryApi.Api.Controllers
{
    [Route("api/sets")]
    [ApiController]
    public class SetController : Controller
    {
        private readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IMetaCardAccessor _metaCardAccessor;
        private readonly IMtgSetAccessor _setAccessor;
        private readonly SetManagerService _setManagerService;

        public SetController(IMetaCardAccessor metaCardAccessor,
            IMtgSetAccessor setAccessor,
            IMtgCardAccessor mtgCardAccessor,
            ICardConstructAccessor cardConstructAccesor)
        {
            _metaCardAccessor = metaCardAccessor;
            _setAccessor = setAccessor;
            _setManagerService = new SetManagerService(_metaCardAccessor, _setAccessor);
        }

        [HttpGet]
        [Route("getById")]
        public IActionResult GetSetsByIds(string idList)
        {
            if (string.IsNullOrEmpty(idList))
            {
                return BadRequest("No set Ids provided");
            }
            try
            {
                string[] setIds = idList.Split(",");
                return Json(_setManagerService.GetSetsById(setIds));
            }
            catch (Exception e)
            {
                logger.Error($"[GetSetsByIds] IdList:{idList}. Error: {e.Message}");
                throw new Exception("Unable to fetch set data.");
            }
        }
    }
}
