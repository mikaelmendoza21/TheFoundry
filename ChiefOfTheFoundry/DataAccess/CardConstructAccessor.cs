using ChiefOfTheFoundry.Models.Inventory;
using MongoDB.Driver;
using MtgApiManager.Lib.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChiefOfTheFoundry.DataAccess
{
    public interface ICardConstructAccessor
    {
        CardConstruct GetCardInCollection(string id);
        CardConstruct GetCardConstruct(FilterDefinition<CardConstruct> filter);
        List<CardConstruct> GetCardCopies(string mtgCardId);
        IEnumerable<CardConstruct> GetCardConstructs(FilterDefinition<CardConstruct> filter);
        CardConstruct Create(CardConstruct construct);
        List<CardConstruct> CreateMultipleCopies(CardConstruct construct, int numberOfCopies, string deckId, string notes);
        void Update(CardConstruct constructIn);
        void Delete(CardConstruct construct);
        void Delete(string id);
    }

    public class CardConstructAccessor : ICardConstructAccessor
    {
        private readonly IMongoCollection<CardConstruct> _cardCollection;

        public CardConstructAccessor(ICollectionDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _cardCollection = database.GetCollection<CardConstruct>(settings.CollectionName);
        }

        public CardConstruct GetCardInCollection(string id)
        {
            return _cardCollection
                .Find<CardConstruct>(c => c.Id == id)
                .FirstOrDefault();
        }

        public CardConstruct GetCardConstruct(FilterDefinition<CardConstruct> filter)
        {
            return _cardCollection
                .Find(filter)
                .FirstOrDefault();
        }

        public IEnumerable<CardConstruct> GetCardConstructs(FilterDefinition<CardConstruct> filter)
        {
            return _cardCollection
                .Find(filter)
                .ToEnumerable();
        }

        public List<CardConstruct> GetCardCopies(string mtgCardId)
        {

            return _cardCollection
                .Find<CardConstruct>(c => c.MtgCardId == mtgCardId)
                .ToList();
        }

        public CardConstruct Create(CardConstruct construct)
        {
            // TODO: Validate MetaCard Id and MtgCard Id referenced
            _cardCollection.InsertOne(construct);

            return construct;
        }

        public List<CardConstruct> CreateMultipleCopies(CardConstruct construct, int numberOfCopies, string deckId, string notes)
        {
            List<CardConstruct> allCopies = new List<CardConstruct>();
            for (int i = 0; i < numberOfCopies; i++)
            {
                CardConstruct clone = new CardConstruct(construct.MetaCardId, construct.MtgCardId, deckId: deckId, notes: notes);
                allCopies.Add(clone);
            }

            _cardCollection.InsertMany(allCopies);

            return allCopies;
        }

        public void Update(CardConstruct constructIn)
        {
            _cardCollection.ReplaceOne(c => c.Id == constructIn.Id, constructIn);
        }

        public void Delete(CardConstruct construct)
        {
            _cardCollection.DeleteOne(c => c.Id == construct.Id);
        }

        public void Delete(string id)
        {
            _cardCollection.DeleteOne(c => c.Id == id);
        }
    }
}
