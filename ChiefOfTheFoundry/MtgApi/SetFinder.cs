using MtgApiManager.Lib.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MtgApiManager.Lib.Core;
using ChiefOfTheFoundry.Models;

namespace ChiefOfTheFoundry.MtgApi
{
    public static class SetFinder
    {
        public static MtgSet FindSetByName(string name)
        {
            SetService service = new SetService();
            Exceptional<List<MtgApiManager.Lib.Model.Set>> result = service
                .Where(set => set.Name, name)
                .All();

            if (result.IsSuccess && result.Value?[0] != null)
                return new MtgSet(result.Value[0]);

            return null;
        }

        public static List<MtgApiManager.Lib.Model.Set> GetAllSets()
        {
            SetService service = new SetService();

            Exceptional<List<MtgApiManager.Lib.Model.Set>> result = service
                .All();

            if (result.IsSuccess && result.Value.Count > 0)
                return result.Value;

            return null;
        }
    }
}
