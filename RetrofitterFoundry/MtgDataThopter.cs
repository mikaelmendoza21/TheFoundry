using ChiefOfTheFoundry.DataAccess;
using ChiefOfTheFoundry.Models;
using ChiefOfTheFoundry.MtgApi;
using MongoDB.Driver;
using MtgApiManager.Lib.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RetrofitterFoundry
{
    public static class MtgDataThopter
    {
        private const int waitTimeInSeconds = 15;

        public static void AddSetsToDb(MtgSetAccessor setAccessor, MetaCardAccessor metaCardAccessor, MtgCardAccessor cardAccessor,  Logger logger, DateTime? startDate)
        {
            string setInProgress = string.Empty;
            List<MtgApiManager.Lib.Model.Set> sets;

            try
            {
                if (startDate.HasValue)
                {
                    sets = SetFinder.GetAllSetsSinceDate(startDate.Value);
                    if (sets.Count > 0)
                    {
                        logger.Info($"[AddSetsToDb] {sets.Count} new sets found since {startDate.Value.ToShortDateString()}");
                    }
                }
                else
                {
                    sets = SetFinder.GetAllSets();
                    logger.Info($"[AddSetsToDb] {sets.Count} total sets found.");
                }
                logger.Info($"[MtgDataThopter][AddSetsToDb] {sets.Count} new sets found.");

                List<MtgSet> newSets = new List<MtgSet>();
                foreach (Set currentSet in sets)
                {
                    if (setAccessor.GetMTGSetByName(currentSet.Name) != null)
                        continue; // Skip set if it exists

                    setInProgress = currentSet.Name;
                    MtgSet set = new MtgSet(currentSet);

                    if (currentSet.OnlineOnly.HasValue && currentSet.OnlineOnly.Value)
                        continue; // Skip online-only sets

                    newSets.Add(setAccessor.Create(set));
                }

                if (startDate.HasValue && newSets.Count > 0)
                {
                    logger.Info($"[AddSetsToDb] adding cards from new sets.");
                    AddAllCardsFromSets(newSets, setAccessor, metaCardAccessor, cardAccessor, logger);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, $"[AddSetsToDb] An error occurred. [SetInProgress={setInProgress}] [Error Message: {e.Message}]");
                logger.Error(e, $"[AddSetsToDb] Trace: {e.StackTrace}");
            }
        }

        public static void AddAllCardsFromSets(List<MtgSet> newSets, MtgSetAccessor setAccessor, MetaCardAccessor metaCardAccessor, MtgCardAccessor cardAccessor, Logger logger)
        {
            foreach (MtgSet set in newSets)
            {
                int page = 0;
                List<Card> cards = CardFinder.GetNextHundredCardsInSet(set.Name, page);

                while (cards?.Count > 0)
                {
                    System.Threading.Thread.Sleep(waitTimeInSeconds * 1000);

                    foreach (Card currentCard in cards)
                    {
                        MtgDataThopter.AddCard(metaCardAccessor, cardAccessor, set, currentCard);
                    }

                    logger.Info($"[AddAllCardsFromSets] Set: {set.Name}. Page: {page}. Total Cards in set processed = {page * 100}");

                    page++;
                    cards = CardFinder.GetNextHundredCardsInSet(set.Name, page);
                }
            }
        }

        public static void AddCard(MetaCardAccessor metaService, MtgCardAccessor cardService, MtgSet set, MtgApiManager.Lib.Model.Card card)
        {
            MetaCard existingMetaCard = metaService.GetMetaCardByName(card.Name);
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
                MetaCard newMetaCard = new MetaCard(card, new List<string>() { set.Id });
                existingMetaCard = metaService.Create(newMetaCard);
            }

            MtgCard mtgCard = new MtgCard(card, set.Id)
            {
                MetaCardID = existingMetaCard.Id
            };
            cardService.Create(mtgCard);
        }

        public static void UpdateCards(MetaCardAccessor metaAccessor, MtgCardAccessor mtgCardAccessor, MtgSetAccessor setAccessor, Logger logger)
        {
            FilterDefinition<MetaCard> missingImageFilter = Builders<MetaCard>.Filter
                .Where(c => c.ImageUrl == MtgConstants.DefaultImageUrl);

            FilterDefinition<MtgCard> missingImageCardFilter = Builders<MtgCard>.Filter
                .Where(c => c.ImageUrl == MtgConstants.DefaultImageUrl);

            List<MetaCard> outdatedMetaCards = metaAccessor.GetMetaCards(missingImageFilter);
            logger.Info($"Found {outdatedMetaCards.Count()} metacards out of date");
            if (outdatedMetaCards.Count > 0)
            {
                int metaCardsUpdated = 0;
                foreach (MetaCard metaCard in outdatedMetaCards)
                {
                    if (metaCard.Name == null)
                    {
                        logger.Info($"Null card name. MetacardId: {metaCard.Id}");
                        continue;
                    }

                    // Update MetaCard definition
                    Thread.Sleep(waitTimeInSeconds);
                    List<MtgApiManager.Lib.Model.Card> cardVersions = CardFinder.GetAllCardVersionsByName(metaCard.Name);
                    if (cardVersions.Count > 0)
                    {
                        logger.Info($"Processing {metaCard.Name}");
                        bool isUpdateAvailable = UpdateMetaCard(cardVersions, metaCard, metaAccessor);

                        // Update outdated versions of the MetaCard
                        if (isUpdateAvailable)
                        {
                            logger.Info($"Updating {metaCard.Name}");
                            metaCardsUpdated++;

                            IEnumerable<MtgCard> outdatedCardVersions = mtgCardAccessor.GetMtgCards(missingImageCardFilter);
                            if (outdatedCardVersions.Count() > 0)
                            {
                                logger.Info($"Metacard has no MtgCards [{metaCard.Id} - {metaCard.Name}]");
                                UpdateCards(cardVersions, outdatedCardVersions, mtgCardAccessor, setAccessor);
                            }
                            else
                            {
                                logger.Info($"Metacard has no MtgCards [{metaCard.Id} - {metaCard.Name}]");
                            }
                        }
                    }
                }

                logger.Info($"Updated {metaCardsUpdated} Metacards.");
            }
            // Outdated MtgCards where MetaCard is up to date
            IEnumerable<MtgCard> outdatedMtgCards = mtgCardAccessor.GetMtgCards(missingImageCardFilter);
            logger.Info($"Found {outdatedMtgCards.Count()} mtgCards out of date");
            if (outdatedMtgCards.Count() > 0)
            {
                IEnumerable<string> cardNamesToUpdate = outdatedMtgCards.GroupBy(x => x.MetaCardID)
                    .Select(c => c.FirstOrDefault().Name);
                foreach(string cardNameToUpdate in cardNamesToUpdate)
                {
                    // TODO: could optimize how the Groupings are used \(.,.)/
                    List<MtgApiManager.Lib.Model.Card> cardVersions = CardFinder.GetAllCardVersionsByName(cardNameToUpdate);
                    IEnumerable<MtgCard> outdatedCardVersions = outdatedMtgCards.Where(c => c.Name == cardNameToUpdate);
                    if (cardVersions != null &&
                        cardVersions.Count > 0 &&
                        outdatedCardVersions.Count() > 0)
                    {
                        logger.Info($"Processing {cardNameToUpdate}");
                        UpdateCards(cardVersions, outdatedCardVersions, mtgCardAccessor, setAccessor);
                    }
                }
            }

            logger.Info($"MtgDataThopter - UpdateCards process finished");
        }

        internal static bool UpdateMetaCard(List<MtgApiManager.Lib.Model.Card> cardVersions, MetaCard metacard, MetaCardAccessor metaCardAccessor)
        {
            Card upToDateCard = cardVersions.FirstOrDefault(c => c.ImageUrl != null);
            if (upToDateCard != null &&
                upToDateCard.ImageUrl != null)
            {
                metacard.ImageUrl = upToDateCard.ImageUrl.ToString();
                metaCardAccessor.Update(metacard);
                return true; // Updated
            }

            return false; // No data to update with
        }

        internal static void UpdateCards(List<MtgApiManager.Lib.Model.Card> cardVersions, IEnumerable<MtgCard> mtgCards, MtgCardAccessor mtgCardAccessor, MtgSetAccessor setAccessor)
        {
            foreach(MtgCard mtgCard in mtgCards)
            {
                MtgSet set = setAccessor.GetMTGSetById(mtgCard.SetID);
                if (set == null)
                {
                    // TODO: Check if possible error?
                    continue;
                }

                Card cardVersion = cardVersions
                    .FirstOrDefault(c => c.SetName == set.Name &&
                        c.ImageUrl != null &&
                        c.ImageUrl.ToString() != mtgCard.ImageUrl);
                if (cardVersion != null)
                {
                    mtgCard.ImageUrl = cardVersion.ImageUrl.ToString();
                    mtgCardAccessor.Update(mtgCard);
                }
            }
        }
    }
}
