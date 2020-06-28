using ChiefOfTheFoundry.DataAccess;
using ChiefOfTheFoundry.Models;
using ChiefOfTheFoundry.Models.Inventory;
using MongoDB.Driver;
using MtgApiManager.Lib.Model;
using NLog.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChiefOfTheFoundry.Services
{
    public interface ICardManagerService
    {
        IEnumerable<MetaCard> GetMetaCardsByNameBeginning(string beginningWithChars);
        Task<List<MetaCard>> GetMetaCardsByNameBeginningAsync(string beginningWithChars);
        IEnumerable<MtgCard> GetMtgCardsByMetacardId(string metacardId);
        IEnumerable<CardConstruct> GetCardConstructsFromMetacardId(string metacardId);
        MtgCard GetMtgCardByMetacardAndSet(string metacardId, string setId);
        List<CardConstruct> CreateCopiesFromConstruct(CardConstruct cardConstruct, int numberOfCopies, string notes);
    }

    public class CardManagerService : ICardManagerService
    {
        IMetaCardAccessor _metaCardAccessor;
        IMtgSetAccessor _setAccessor;
        IMtgCardAccessor _mtgCardAccessor;
        ICardConstructAccessor _cardConstructAccesor;

        public CardManagerService(IMetaCardAccessor metaCardAccessor,
            IMtgSetAccessor setAccessor,
            IMtgCardAccessor mtgCardAccessor,
            ICardConstructAccessor cardConstructAccesor)
        {
            _metaCardAccessor = metaCardAccessor;
            _setAccessor = setAccessor;
            _mtgCardAccessor = mtgCardAccessor;
            _cardConstructAccesor = cardConstructAccesor;
        }

        public IEnumerable<MetaCard> GetMetaCardsByNameBeginning(string beginningWithChars)
        {
            FilterDefinition<MetaCard> filterByName = Builders<MetaCard>.Filter
                .Where(c => c.Name.ToUpper().StartsWith(beginningWithChars.ToUpper()));

            return _metaCardAccessor
                .GetMetaCards(filterByName);
        }

        public async Task<List<MetaCard>> GetMetaCardsByNameBeginningAsync(string beginningWithChars)
        {
            FilterDefinition<MetaCard> filterByName = Builders<MetaCard>.Filter
                .Where(c => c.Name.ToUpper().StartsWith(beginningWithChars.ToUpper()));

            return await _metaCardAccessor
                .GetMetaCardsAsync(filterByName);
        }

        public IEnumerable<MtgCard> GetMtgCardsByMetacardId(string metacardId)
        {
            FilterDefinition<MtgCard> filter = Builders<MtgCard>.Filter.Eq(c => c.MetaCardID, metacardId);

            return _mtgCardAccessor.GetMtgCards(filter);
        }

        public MtgCard GetMtgCardByMetacardAndSet(string metacardId, string setId)
        {
            FilterDefinition<MtgCard>[] allFilters = new []
            {
                Builders<MtgCard>.Filter.Eq(c => c.MetaCardID, metacardId),
                Builders<MtgCard>.Filter.Eq(c => c.SetID, setId)
            };
            FilterDefinition<MtgCard> filter = Builders<MtgCard>.Filter.And(allFilters);

            return _mtgCardAccessor.GetMtgCard(filter);
        }

        public IEnumerable<CardConstruct> GetCardConstructsFromMetacardId(string metacardId)
        {
            FilterDefinition<CardConstruct> filter = Builders<CardConstruct>.Filter.Eq(c => c.MetaCardId, metacardId);

            return _cardConstructAccesor.GetCardConstructs(filter);
        }

        public List<CardConstruct> CreateCopiesFromConstruct(CardConstruct cardConstruct, int numberOfCopies, string notes)
        {
            // TODO: Allow setting deckId here
            return _cardConstructAccesor.CreateMultipleCopies(cardConstruct, numberOfCopies, null, notes);
        }
    }
}
