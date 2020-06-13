using ChiefOfTheFoundry.Models.Inventory;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChiefOfTheFoundry.DataAccess
{
    public interface IDeckAccessor
    {
        Deck GetDeckById(string id);
        Deck GetDeckByName(string name);
        IEnumerable<Deck> GetDecks(FilterDefinition<Deck> filter);
        Deck Create(Deck deck);
        void Update(Deck inDeck);
        void Remove(Deck deck);
        void Remove(string deckId);
    }

    public class DeckAccessor : IDeckAccessor
    {
        private readonly IMongoCollection<Deck> _decks;

        public DeckAccessor(ICollectionDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _decks = database.GetCollection<Deck>(settings.CollectionName);
        }

        public Deck GetDeckById(string id)
        {
            return _decks.Find(deck => deck.Id == id)
                .FirstOrDefault();
        }

        public Deck GetDeckByName(string name)
        {
            return _decks
                .Find(deck => deck.Id.ToUpper() == name.ToUpper())
                .FirstOrDefault();
        }

        public IEnumerable<Deck> GetDecks(FilterDefinition<Deck> filter)
        {
            return _decks.Find(filter)
                .ToEnumerable();
        }

        public Deck Create(Deck deck)
        {
            Deck existingDeck = GetDeckByName(deck.Name);

            if (existingDeck != null)
                throw new Exception($"Cannot create duplicate decks. Found existing deck with name '{deck.Name}'");

            _decks.InsertOne(deck);

            return deck;
        }

        public void Update(Deck inDeck)
        {
            _decks.ReplaceOne(deck => deck.Id == inDeck.Id, inDeck);
        }

        public void Remove(Deck deck)
        {
            _decks.DeleteOne(d => d.Id == deck.Id);
        }
        
        public void Remove(string deckId)
        {
            _decks.DeleteOne(d => d.Id == deckId);
        }
    }
}
