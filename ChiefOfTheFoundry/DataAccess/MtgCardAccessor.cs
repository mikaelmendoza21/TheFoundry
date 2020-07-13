using ChiefOfTheFoundry.Models;
using MongoDB.Driver;
using NLog.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ChiefOfTheFoundry.DataAccess
{
    public interface IMtgCardAccessor
    {
        MtgCard GetMtgCardByName(string name, string setId);
        MtgCard GetMtgCardById(string id);
        MtgCard GetMtgCard(FilterDefinition<MtgCard> filter);
        List<MtgCard> GetMtgCards(FilterDefinition<MtgCard> filter);
        MtgCard Create(MtgCard card);
        void Update(MtgCard cardIn);
        void Remove(MtgCard setIn);
        void Remove(string id);
    }

    public class MtgCardAccessor : IMtgCardAccessor
    {
        private readonly IMongoCollection<MtgCard> _cards;

        public MtgCardAccessor(ICollectionDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _cards = database.GetCollection<MtgCard>(settings.CollectionName);
        }

        public MtgCard GetMtgCardByName(string name, string setId)
        {
            return _cards
                .Find<MtgCard>(card => card.Name.ToUpper() == name.ToUpper() && card.SetID == setId)
                .FirstOrDefault();
        }

        public MtgCard GetMtgCardById(string id)
        {
            return _cards.Find<MtgCard>(card => card.Id == id)
            .FirstOrDefault();
        }

        public MtgCard GetMtgCard(FilterDefinition<MtgCard> filter)
        {
            return _cards
                .Find(filter)
                .FirstOrDefault();
        }

        public List<MtgCard> GetMtgCards(FilterDefinition<MtgCard> filter)
        {
            return _cards
                .Find(filter)
                .ToEnumerable()
                .ToList();
        }
        
        public MtgCard Create(MtgCard card)
        {
            MtgCard existingCard = GetMtgCardByName(card.Name, card.SetID);
            
            if (existingCard == null)
                _cards.InsertOne(card);
                
            return existingCard ?? card;
        }

        public void Update(MtgCard cardIn)
        {
            _cards.ReplaceOne(card => card.Id == cardIn.Id, cardIn);
        }

        public void Remove(MtgCard cardIn)
        {
            _cards.DeleteOne(card => card.Id == card.Id);
        }

        public void Remove(string id)
        {
            _cards.DeleteOne(card => card.Id == id);
        }
    }
}
