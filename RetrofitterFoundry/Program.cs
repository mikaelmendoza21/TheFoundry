using ChiefOfTheFoundry.DataAccess;
using ChiefOfTheFoundry.Models;
using ChiefOfTheFoundry.MtgApi;
using Microsoft.Extensions.CommandLineUtils;
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
        /// Accepts optional argument '--seed' or '-s' to seed whole database.
        /// </summary>
        static void Main(string[] args)
        {
            var app = new CommandLineApplication();

            // This should be the name of the executable itself.
            // the help text line "Usage: ConsoleArgs" uses this
            app.Name = "RetroffiterFoundry";

            var isSeedOption = app.Option("-s|--seed",
                    "Some option value",
                    CommandOptionType.NoValue);

            app.OnExecute(() =>
            {
                Console.WriteLine("You've casted Retrofitter Foundry");
                Console.WriteLine("This will populate your local database with MetaCard data");
                logger.Info($"Retroffiter Foundry Started at {DateTime.Now.TimeOfDay}");

                try
                {
                    // Seed Sets
                    bool isSeed = isSeedOption.HasValue();

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

                    // Get Sets
                    SeedSetsDatabase(isSeed, setAccessor, metaCardAccessor, mtgCardAccessor);

                    // Seed Cards (only if doing full seed)
                    if (isSeed)
                    {
                        SeedCardDatabase(metaCardAccessor, setAccessor, mtgCardAccessor);
                    }
                    else
                    {
                        // Update cards with new info (if missing - example: Images Urls)
                        try
                        {
                            logger.Info($"[RetrofitterFoundry] Looking for any updated data for existing cards missing info");
                            MtgDataThopter.UpdateCards(metaCardAccessor, mtgCardAccessor, setAccessor, logger);
                        }
                        catch (Exception e)
                        {
                            logger.Error($"[RetrofitterFoundry] An error occurred updating existing card. Error = {e.Message}");
                            logger.Error($"Trace: {e.StackTrace}");
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error($"Retrofitter Foundry was terminated. Error = {e.Message}");
                    logger.Error($"Trace: {e.StackTrace}");
                }

                Console.WriteLine($"Retrofitter Foundry left the field. Press any key to end.");
                Console.ReadLine();
                logger.Info("Retrofitter Foundry left the field.");

                return 0;
            });

            try
            {
                // This begins the actual execution of the application
                Console.WriteLine("ConsoleArgs app executing...");
                app.Execute(args);
            }
            catch (CommandParsingException ex)
            {
                // You'll always want to catch this exception, otherwise it will generate a messy and confusing error for the end user.
                // the message will usually be something like:
                // "Unrecognized command or argument '<invalid-command>'"
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to execute application: {0}", ex.Message);
            }
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
