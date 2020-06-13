using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChiefOfTheFoundry.Models.Inventory
{
    /// <summary>
    /// A physical card in an MTG personal collection.
    /// It has references to a MetaCard, as well as a specific version of that card (MtgCard).
    /// </summary>
    public class CardConstruct
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string MetaCardId { get; set; }
        public string MtgCardId { get; set; }
        public string DeckId { get; set; }
        public string Notes { get; set; }

        public CardConstruct(string metaCardId, string mtgCardId, string deckId = "", string notes = "")
        {
            MetaCardId = metaCardId;
            MtgCardId = mtgCardId;
            DeckId = deckId;
            Notes = notes;
        }

        public CardConstruct(MtgCard mtgCard, string deckId = "", string notes = "")
        {
            MetaCardId = mtgCard.MetaCardID;
            MtgCardId = mtgCard.Id;
            DeckId = deckId;
            Notes = notes;
        }
    }
}
