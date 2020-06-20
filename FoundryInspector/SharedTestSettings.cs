using ChiefOfTheFoundry.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoundryInspector
{
    public static class SharedTestSettings
    {
        // DbSettings
        public const string CONN_STRING = "mongodb://localhost:27017";
        public const string DB_NAME = "FoundryDb";
        public const string SET_COLLECTION = "Sets";
        public const string METACARD_COLLECTION = "MetaCards";
        public const string CARD_COLLECTION = "Cards";
        public const string CARDCONSTRUCT_COLLECTION = "CardCollection";
        public const string DECK_COLLECTION = "Decks";

        // Sample MetaCard
        public const string SAMPLE_METACARD_NAME = "Chief of the Foundry";
        public const string SAMPLE_METACARD_OBJECTID = "5ed5e403facabe0be4e6e9e8"; // Use values form your own Db

        public static MetaCardAccessor GetMetaCardAccessor()
        {
            CollectionDbSettings metaCardsDbSettings = new CollectionDbSettings()
            {
                CollectionName = SharedTestSettings.METACARD_COLLECTION,
                ConnectionString = SharedTestSettings.CONN_STRING,
                DatabaseName = SharedTestSettings.DB_NAME
            };
            return new MetaCardAccessor(metaCardsDbSettings);
        }

        public static MtgSetAccessor GetMtgSetAccessor()
        {
            CollectionDbSettings setDbSettings = new CollectionDbSettings()
            {
                CollectionName = SharedTestSettings.SET_COLLECTION,
                ConnectionString = SharedTestSettings.CONN_STRING,
                DatabaseName = SharedTestSettings.DB_NAME
            };
            return new MtgSetAccessor(setDbSettings);
        }
    }
}
