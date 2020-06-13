using ChiefOfTheFoundry.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChiefOfTheFoundry.DataAccess
{
    public interface IMetaCardAccessor
    {
        MetaCard GetOneMetaCard();
        MetaCard GetMetaCardById(string id);
        MetaCard GetMetaCardByName(string name);
        IEnumerable<MetaCard> GetMetaCards(FilterDefinition<MetaCard> filter);
        Task<List<MetaCard>> GetMetaCardsAsync(FilterDefinition<MetaCard> filter);
        MetaCard Create(MetaCard card);
        void Update(MetaCard cardIn);
        void Remove(MetaCard cardIn);
        void Remove(string id);
    }

    public class MetaCardAccessor : IMetaCardAccessor
    {
        private readonly IMongoCollection<MetaCard> _metaCards;

        public MetaCardAccessor(ICollectionDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _metaCards = database.GetCollection<MetaCard>(settings.CollectionName);
        }

        // GETTERS
        public MetaCard GetOneMetaCard()
        {
            return _metaCards
                .Find<MetaCard>(card => true)
                .FirstOrDefault();
        }

        public MetaCard GetMetaCardById(string id)
        {
            return _metaCards.Find<MetaCard>(card => card.Id == id)
            .FirstOrDefault();
        }

        public MetaCard GetMetaCardByName(string name)
        {
            return _metaCards
                .Find<MetaCard>(card => card.Name.ToUpper() == name.ToUpper())
                .FirstOrDefault();
        }

        public IEnumerable<MetaCard> GetMetaCards(FilterDefinition<MetaCard> filter)
        {
            return _metaCards
                .Find(filter)
                .ToEnumerable();
        }
        public async Task<List<MetaCard>> GetMetaCardsAsync(FilterDefinition<MetaCard> filter)
        {
            return await _metaCards
                .Find(filter)
                .ToListAsync();
        }

        // Modifiers
        public MetaCard Create(MetaCard card)
        {
            MetaCard existingCard = GetMetaCardByName(card.Name);

            if(existingCard == null)
                _metaCards.InsertOne(card);

            return existingCard ?? card;
        }

        public void Update(MetaCard cardIn)
        {
            _metaCards.ReplaceOne(card => card.Id == cardIn.Id, cardIn);
        }

        public void Remove(MetaCard cardIn)
        {
            _metaCards.DeleteOne(card => card.Id == cardIn.Id);
        }

        public void Remove(string id)
        {
            _metaCards.DeleteOne(card => card.Id == id);
        }
    }
}
