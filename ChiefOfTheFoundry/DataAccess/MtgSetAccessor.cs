using ChiefOfTheFoundry.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChiefOfTheFoundry.DataAccess
{
    public interface IMtgSetAccessor
    {
        MtgSet GetMTGSetByName(string name);
        MtgSet GetMTGSetById(string id);
        MtgSet GetLatestReleasedSet();
        IEnumerable<MtgSet> GetMtgSets(FilterDefinition<MtgSet> filter);
        MtgSet Create(MtgSet set);
        void Update(string id, MtgSet setIn);
        void Remove(MtgSet setIn);
        void Remove(string id);
    }

    public class MtgSetAccessor : IMtgSetAccessor
    {
        private readonly IMongoCollection<MtgSet> _sets;

        public MtgSetAccessor(ICollectionDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _sets = database.GetCollection<MtgSet>(settings.CollectionName);
        }

        public MtgSet GetMTGSetByName(string name)
        {
            return _sets
                .Find<MtgSet>(set => set.Name.ToUpper() == name.ToUpper())
                .FirstOrDefault();
        }

        public MtgSet GetMTGSetById(string id)
        {
            return _sets.Find<MtgSet>(set => set.Id == id)
            .FirstOrDefault();
        }

        public MtgSet GetLatestReleasedSet()
        {
            return _sets.
                Find<MtgSet>(Builders<MtgSet>.Filter.Empty)
                .SortByDescending(s => s.ReleaseDate)
                .FirstOrDefault();
        }
        public IEnumerable<MtgSet> GetMtgSets(FilterDefinition<MtgSet> filter)
        {
            return _sets
                .Find(filter)
                .ToEnumerable();
        }

        public MtgSet Create(MtgSet set)
        {
            MtgSet existingSet = GetMTGSetByName(set.Name);
            
            if (existingSet == null)
                _sets.InsertOne(set);
                
            return existingSet ?? set;
        }

        public void Update(string id, MtgSet setIn)
        {
            _sets.ReplaceOne(set => set.Id == id, setIn);
        }

        public void Remove(MtgSet setIn)
        {
            _sets.DeleteOne(set => set.Id == setIn.Id);
        }

        public void Remove(string id)
        {
            _sets.DeleteOne(set => set.Id == id);
        }
    }
}
