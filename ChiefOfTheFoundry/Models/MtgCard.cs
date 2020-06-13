using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChiefOfTheFoundry.Models
{
    /// <summary>
    /// Represents a printed card. Specific set, artwork version.
    /// </summary>
    public class MtgCard : MasterMtgCard
    {
        private static Uri DefaultImage = new Uri("https://www.pcgamesn.com/wp-content/uploads/2019/06/mtg-arena-core-set-2020.jpg");

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string MetaCardID { get; set; }
        public Uri ImageUrl { get; set; }
        public string SetID { get; set; }
        public List<string> Types { get; set; }
        public List<string> Colors { get; set; }
        public List<string> ColorIdentity { get; set; }
        public List<string> Variations { get; set; }

        public MtgCard(MtgApiManager.Lib.Model.Card cardInstance, string setID)
        {
            Name = cardInstance.Name;
            ManaCost = cardInstance.ManaCost;
            Text = cardInstance.Text;
            Type = cardInstance.Type;
            Types = cardInstance.Types?.ToList();
            ImageUrl = cardInstance.ImageUrl ?? DefaultImage;
            SetID = setID;
            Colors = cardInstance.Colors?.ToList();
            ColorIdentity = cardInstance.ColorIdentity?.ToList();
            Variations = cardInstance.Variations?.ToList();
        }
    }
}
