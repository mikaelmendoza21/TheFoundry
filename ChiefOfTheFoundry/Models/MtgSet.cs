using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MtgApiManager.Lib.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChiefOfTheFoundry.Models
{
    public class MtgSet
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }

        public string Block { get; set; }

        public string Code { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime ReleaseDate { get; set; }

        public string IconPath { get; set; }

        public MtgSet(MtgApiManager.Lib.Model.Set set)
        {
            Name = set.Name;
            Block = set.Block;
            Code = set.GathererCode ?? set.Code;

            DateTime releaseDate;
            bool dateIsValid = DateTime.TryParse(set.ReleaseDate, out releaseDate);
            if (dateIsValid)
                ReleaseDate = releaseDate.ToUniversalTime();

            // Add manually later
            IconPath = string.Empty;
        }
    }
}
