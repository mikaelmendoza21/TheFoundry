﻿using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChiefOfTheFoundry.Models
{
    public struct MtgConstants
    {
        public const string DefaultImageUrl = "/img/mtg-card-back.jpg";
    }

    /// <summary>
    /// Defines a Card by name. Basic definition of a card instance, does not contain specific set, cost information.
    /// </summary>
    public class MetaCard : MasterMtgCard
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string ImageUrl { get; set; }
        public List<string> SetIDs { get; set; }

        /* Constructors */
        public MetaCard(string name, string manaCost, string text, string type, Uri imageUrl, List<string> setIDs)
        {
            Name = name;
            ManaCost = manaCost;
            Text = text;
            Type = type;
            ImageUrl = imageUrl.ToString() ?? MtgConstants.DefaultImageUrl;
            SetIDs = setIDs;
        }

        public MetaCard(MtgApiManager.Lib.Model.Card cardInstance, List<string> setIds)
        {
            Name = cardInstance.Name;
            ManaCost = cardInstance.ManaCost;
            Text = cardInstance.OriginalText;
            Type = cardInstance.Type;
            ImageUrl = cardInstance.ImageUrl.ToString() ?? MtgConstants.DefaultImageUrl;
            SetIDs = setIds;
        }
    }
}
