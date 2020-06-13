using ChiefOfTheFoundry.DataAccess;
using ChiefOfTheFoundry.Models;
using ChiefOfTheFoundry.MtgApi;
using MtgApiManager.Lib.Model;
using System;
using System.Collections.Generic;

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

        static void Main(string[] args)
        {
            Console.WriteLine("You've casted Retrofitter Foundry");
            Console.WriteLine("This will populate your local database with MetaCard data");
            logger.Info($"Retroffiter Foundry Started at {DateTime.Now.TimeOfDay}");

            try
            {
                int startPage = 1;
                if (args.Length > 0)
                    startPage = Int32.Parse(args[0]);

                // Seed Sets
                SeedSetsDatabase();

                // Seed Cards
                SeedCardDatabase(startPage);
            }
            catch (Exception e)
            {
                logger.Error($"Retrofitter Foundry was terminated. Error = {e.Message}");
            }

            Console.WriteLine($"Retrofitter Foundry left the field. {DateTime.Now.TimeOfDay}");
            logger.Info("Retrofitter Foundry left the field.");
        }

        private static void SeedSetsDatabase()
        {
            string setInProgress = string.Empty;

            try
            {
                CollectionDbSettings dbSettings = new CollectionDbSettings()
                {
                    CollectionName = SetsCollection,
                    ConnectionString = ConnString,
                    DatabaseName = DbName
                };
                MtgSetAccessor service = new MtgSetAccessor(dbSettings);

                logger.Info("Retrofitter Foundry started process: SeedSetsDatabase");
                Console.WriteLine("Retrofitter Foundry started process: SeedSetsDatabase");

                List<MtgApiManager.Lib.Model.Set> sets = SetFinder.GetAllSets();
                Console.WriteLine($"Retrofitter Foundry found {sets.Count}");

                foreach (Set currentSet in sets)
                {
                    if (service.GetMTGSetByName(currentSet.Name) != null)
                        continue; // Skip set if it exists

                    setInProgress = currentSet.Name;
                    MtgSet set = new MtgSet(currentSet);

                    if (currentSet.OnlineOnly.HasValue && currentSet.OnlineOnly.Value)
                        continue; // Skip online-only sets

                    service.Create(set);
                }

                Console.WriteLine("Retrofitter Foundry finished process: SeedSetsDatabase");
                logger.Info("Retrofitter Foundry finished process: SeedSetsDatabase");
            }
            catch (Exception e)
            {
                logger.Error(e, $"[SeedSetDatabase] An error occurred. [SetInProgress={setInProgress}] Error Message: {e.Message}");
                logger.Error(e, $"[SeedSetDatabase] Trace: {e.StackTrace}");
            }
        }

        private static void SeedCardDatabase(int page = 1)
        {
            int waitTimeInSeconds = 15;
            try
            {
                logger.Info("Retrofitter Foundry started process: SeedMetaCardDatabase");
                Console.WriteLine("Retrofitter Foundry started process: SeedMetaCardDatabase");

                CollectionDbSettings metaCardsDbSettings = new CollectionDbSettings()
                {
                    CollectionName = MetaCardsCollection,
                    ConnectionString = ConnString,
                    DatabaseName = DbName
                };
                MetaCardAccessor metaService = new MetaCardAccessor(metaCardsDbSettings);

                CollectionDbSettings setDbSettings = new CollectionDbSettings()
                {
                    CollectionName = SetsCollection,
                    ConnectionString = ConnString,
                    DatabaseName = DbName
                };
                MtgSetAccessor setService = new MtgSetAccessor(setDbSettings);

                CollectionDbSettings cardDbSettings = new CollectionDbSettings()
                {
                    CollectionName = CardsCollection,
                    ConnectionString = ConnString,
                    DatabaseName = DbName
                };
                MtgCardAccessor cardService = new MtgCardAccessor(cardDbSettings);

                List<Card> cards = CardFinder.GetNextHundredCards(page);

                while (cards?.Count > 0)
                {
                    System.Threading.Thread.Sleep(waitTimeInSeconds * 1000);

                    foreach (Card currentCard in cards)
                    {
                        MtgSet set = setService.GetMTGSetByName(currentCard.SetName);

                        if (set != null)
                        {
                            MetaCard existingMetaCard = metaService.GetMetaCardByName(currentCard.Name);
                            if (existingMetaCard != null &&
                                existingMetaCard.SetIDs != null &&
                                !existingMetaCard.SetIDs.Contains(set.Id))
                            {
                                // Update MetaCard with SetId reference
                                existingMetaCard.SetIDs.Add(set.Id);
                                metaService.Update(existingMetaCard);
                            }
                            else if (existingMetaCard == null)
                            {
                                // Add MetaCard to Db
                                MetaCard newMetaCard = new MetaCard(currentCard, new List<string>() { set.Id });
                                existingMetaCard = metaService.Create(newMetaCard);
                            }

                            MtgCard card = new MtgCard(currentCard, set.Id)
                            {
                                MetaCardID = existingMetaCard.Id
                            };
                            cardService.Create(card);
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
