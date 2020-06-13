using MtgApiManager.Lib.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MtgApiManager.Lib.Core;
using ChiefOfTheFoundry.Models;
using MtgApiManager.Lib.Model;

namespace ChiefOfTheFoundry.MtgApi
{
    public static class CardFinder
    {
        public const int DefaultPageSize = 100;
        public static MetaCard FindMetaCardByName(string name)
        {
            CardService service = new CardService();
            Exceptional<List<MtgApiManager.Lib.Model.Card>> result = service
                .Where(c => c.Name, name)
                .All();

            if (result.IsSuccess && result.Value?[0] != null)
            {
                List<string> setIds = new List<string>();

                foreach(Card cardInstance in result.Value)
                {
                    MtgSet set = SetFinder.FindSetByName(cardInstance.SetName);
                    if (set != null)
                    {
                        setIds.Add(set.Id);
                    }
                }

                return new MetaCard(result.Value[0], setIds.Distinct().ToList());
            }

            return null;
        }

        public static List<MtgApiManager.Lib.Model.Card> GetNextHundredCards(int pageNumber = 0)
        {
            CardService service = new CardService();

            Exceptional<List<MtgApiManager.Lib.Model.Card>> result = service
                .Where(cards => cards.Page, pageNumber)
                .Where(cards => cards.PageSize, DefaultPageSize)
                .All();

            if (result.IsSuccess && result.Value.Count > 0)
                return result.Value;

            return null;
        }
    }
}
