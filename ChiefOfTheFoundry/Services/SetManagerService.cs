using ChiefOfTheFoundry.DataAccess;
using ChiefOfTheFoundry.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChiefOfTheFoundry.Services
{
    public interface ISetManagerService
    {
        IEnumerable<MtgSet> GetSetsById(string[] ids);
    }
    public class SetManagerService : ISetManagerService
    {
        IMetaCardAccessor _metaCardAccessor;
        IMtgSetAccessor _setAccessor;

        public SetManagerService(IMetaCardAccessor metaCardAccessor,
            IMtgSetAccessor setAccessor)
        {
            _metaCardAccessor = metaCardAccessor;
            _setAccessor = setAccessor;
        }

        public IEnumerable<MtgSet> GetSetsById(string[] ids)
        {
            FilterDefinition<MtgSet> filterById = Builders<MtgSet>.Filter.In(s => s.Id, ids);
            
            return _setAccessor.GetMtgSets(filterById);
        }
    }
}
