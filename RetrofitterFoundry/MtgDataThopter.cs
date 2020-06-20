using ChiefOfTheFoundry.DataAccess;
using ChiefOfTheFoundry.Models;
using ChiefOfTheFoundry.MtgApi;
using MtgApiManager.Lib.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
