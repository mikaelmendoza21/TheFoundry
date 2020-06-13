using ChiefOfTheFoundry.DataAccess;
using ChiefOfTheFoundry.Models;
using ChiefOfTheFoundry.Models.Inventory;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChiefOfTheFoundry.Services
{
    public interface ICollectionManagerService
    {
        IEnumerable<MtgSet> GetSetsFromMetaCard(MetaCard metaCard);
        IEnumerable<CardConstruct> GetCardCopies(string mtgCardId);
        IEnumerable<CardConstruct> GetAllCardsInDeck(string deckId);
    }

    public class CollectionManagerService : ICollectionManagerService
    {
        IMetaCardAccessor _metaCardAccessor;
        IMtgSetAccessor _setAccessor;
        IMtgCardAccessor _mtgCardAccessor;
        ICardConstructAccessor _cardConstructAccesor;
        IDeckAccessor _deckAccessor;

        public CollectionManagerService(IMetaCardAccessor metaCardAccessor,
            IMtgSetAccessor setAccessor,
            IMtgCardAccessor mtgCardAccessor,
            ICardConstructAccessor cardConstructAccesor,
            IDeckAccessor deckAccessor)
        {
            _metaCardAccessor = metaCardAccessor;
            _setAccessor = setAccessor;
            _mtgCardAccessor = mtgCardAccessor;
            _cardConstructAccesor = cardConstructAccesor;
            _deckAccessor = deckAccessor;
        }

        public IEnumerable<MtgSet> GetSetsFromMetaCard(MetaCard metaCard)
        {
            FilterDefinition<MtgSet> findSetsById = Builders<MtgSet>.Filter.In(s => s.Id, metaCard.SetIDs.ToArray());

            return _setAccessor.GetMtgSets(findSetsById);
        }

        public IEnumerable<CardConstruct> GetCardCopies(string mtgCardId)
        {
            if (string.IsNullOrEmpty(mtgCardId))
            {
                FilterDefinition<CardConstruct> findCardsById = Builders<CardConstruct>.Filter.Eq(c => c.MtgCardId, mtgCardId);
                return _cardConstructAccesor.GetCardConstructs(findCardsById);
            }

            return null;
        }

        public IEnumerable<CardConstruct> GetAllCardsInDeck(string deckId)
        {
            Deck deck = _deckAccessor.GetDeckById(deckId);
            if (deck.CardIds?.Count > 0)
            {
                FilterDefinition<CardConstruct> findCardsById = Builders<CardConstruct>.Filter.In(c => c.Id, deck.CardIds.ToArray());
                return _cardConstructAccesor.GetCardConstructs(findCardsById);
            }

            return null;
        }
    }
}
