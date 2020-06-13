using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChiefOfTheFoundry.Models.Inventory
{
    /// <summary>
    /// A physical deck, made up of cards in a personal MTG collection.
    /// </summary>
    public class Deck
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> CardIds { get; set; }
        public List<string> SideboardIds { get; set; }
        public string GameFormat { get; set; }
        public string Notes { get; set; }

        public Deck(List<string> cards, List<string> sideboard, string name, string gameFormat, string notes = "")
        {
            Name = name;
            CardIds = cards;
            SideboardIds = sideboard;
            GameFormat = gameFormat;
            Notes = notes;
        }

        public Deck(List<MtgCard> cards, List<MtgCard> sideboard, string name, string gameFormat, string notes = "")
        {
            Name = name;
            CardIds = cards.Select(c => c.Id).ToList();
            SideboardIds = sideboard.Select(s => s.Id).ToList();
            GameFormat = gameFormat;
            Notes = notes;
        }
    }
}
