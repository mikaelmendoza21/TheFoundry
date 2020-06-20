using ChiefOfTheFoundry.DataAccess;
using ChiefOfTheFoundry.Models;
using ChiefOfTheFoundry.MtgApi;
using MongoDB.Driver;
using MtgApiManager.Lib.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RetrofitterFoundry
{
    class Program
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private const string ConnString = "mongodb://localhost:27017";
        private const string DbName = "FoundryDb";
        private const string SetsCollection = "Sets";
        private const string MetaCardsCollection = "MetaCards";
        private const string CardsCollection = "Cards";

        /// <summary>
        /// Takes a Single argument [0] = 'true' if doing a whole Seed operation. Else performs an update 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("You've casted Retrofitter Foundry");
            Console.WriteLine("This will populate your local database with MetaCard data");
            logger.Info($"Retroffiter Foundry Started at {DateTime.Now.TimeOfDay}");

            try
            {
                CollectionDbSettings metaCardsDbSettings = new CollectionDbSettings()
                {
                    CollectionName = MetaCardsCollection,
                    ConnectionString = ConnString,
                    DatabaseName = DbName
                };
                MetaCardAccessor metaCardAccessor = new MetaCardAccessor(metaCardsDbSettings);

                CollectionDbSettings setDbSettings = new CollectionDbSettings()
                {
                    CollectionName = SetsCollection,
                    ConnectionString = ConnString,
                    DatabaseName = DbName
                };
                MtgSetAccessor setAccessor = new MtgSetAccessor(setDbSettings);

                CollectionDbSettings cardDbSettings = new CollectionDbSettings()
                {
                    CollectionName = CardsCollection,
                    ConnectionString = ConnString,
                    DatabaseName = DbName
                };
                MtgCardAccessor mtgCardAccessor = new MtgCardAccessor(cardDbSettings);

                // Seed Sets
                bool isSeed = false;
                if (args.Length > 0)
                {
                    bool.TryParse(args[0], out isSeed);
                }

                // Get Sets
                SeedSetsDatabase(isSeed, setAccessor, metaCardAccessor, mtgCardAccessor);

                // Seed Cards (only if doing full seed)
                if (isSeed)
                {
                    SeedCardDatabase(metaCardAccessor, setAccessor, mtgCardAccessor);
                }
            }
            catch (Exception e)
            {
                logger.Error($"Retrofitter Foundry was terminated. Error = {e.Message}");
            }

            Console.WriteLine($"Retrofitter Foundry left the field. Press any key to end.");
            Console.ReadLine();
            logger.Info("Retrofitter Foundry left the field.");
        }

        private static void SeedSetsDatabase(bool isSeed, MtgSetAccessor setAccessor, MetaCardAccessor metaCardAccessor, MtgCardAccessor mtgCardAccessor)
        {
            try
            {
                logger.Info("Retrofitter Foundry started process: SeedSetsDatabase");
                Console.WriteLine("Retrofitter Foundry started process: SeedSetsDatabase");

                DateTime? startDate = null;
                if (!isSeed)
                {
                    // Get latest release Date from all Sets
                    startDate = setAccessor.GetLatestReleasedSet().ReleaseDate;
                    Console.WriteLine($"Retrofitter Foundry => updating FoundryDb with any new data since {startDate.Value.ToShortDateString()}");
                    logger.Info($"[RetrofitterFoundry] updating FoundryDb with any new data since {startDate.Value.ToShortDateString()}");
                }
                else
                {
                    Console.WriteLine($"Retrofitter Foundry => seeding whole datase");
                    logger.Info($"[RetrofitterFoundry] seeding whole database");
                }
                MtgDataThopter.AddSetsToDb(setAccessor, metaCardAccessor, mtgCardAccessor, logger, startDate);

                Console.WriteLine("Retrofitter Foundry finished process: SeedSetsDatabase");
                logger.Info("Retrofitter Foundry finished process: SeedSetsDatabase");
            }
            catch (Exception e)
            {
                logger.Error(e, $"[SeedSetDatabase] An error occurred. Error Message: {e.Message}");
                logger.Error(e, $"[SeedSetDatabase] Trace: {e.StackTrace}");
            }
        }

        private static void SeedCardDatabase(MetaCardAccessor metaCardAccessor, MtgSetAccessor setAccessor, MtgCardAccessor mtgCardAccessor)
        {
            int page = 1;
            int waitTimeInSeconds = 15;
            try
            {
                logger.Info("Retrofitter Foundry started process: SeedMetaCardDatabase");
                Console.WriteLine("Retrofitter Foundry started process: SeedMetaCardDatabase");

                List<Card> cards = CardFinder.GetNextHundredCards(page);

                while (cards?.Count > 0)
                {
                    System.Threading.Thread.Sleep(waitTimeInSeconds * 1000);

                    MtgSet set = null;
                    foreach (Card currentCard in cards)
                    {
                        if (set == null || set.Name != currentCard.SetName)
                        {
                            set = setAccessor.GetMTGSetByName(currentCard.SetName);
                        }

                        if (set != null)
                        {
                            MtgDataThopter.AddCard(metaCardAccessor, mtgCardAccessor, set, currentCard);
                        }
                        // else - don't add 'onlineOnly' set cards
                    }

                    Console.WriteLine($"Page: {page}. Total Cards processed = {page * 100}");

                    page++;
                    cards = CardFinder.GetNextHundredCards(page);
                }
                logger.Info("Retrofitter Foundry finished process: SeedMetaCardDatabase");
            }
            catch (Exception e)
            {
                logger.Error(e, $"[SeedMetaCardDatabase] An error occurred. [Page={page}] Error Message: {e.Message}");
                logger.Error(e, $"[SeedMetaCardDatabase] Trace {e.StackTrace}");
            }
        }
    }
}
