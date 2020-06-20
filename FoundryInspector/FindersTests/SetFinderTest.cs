using ChiefOfTheFoundry.MtgApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MtgApiManager.Lib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoundryInspector.FindersTests
{
    [TestClass]
    public class SetFinderTest
    {
        [TestMethod]
        public void GetAllSetsSinceDateTest()
        {
            DateTime date = DateTime.Now.AddYears(-10);
            List<Set> setsInLastTenYears = SetFinder.GetAllSetsSinceDate(date);

            Assert.IsTrue(setsInLastTenYears.Count > 0);
            Assert.IsFalse(setsInLastTenYears.Any(s => DateTime.Parse(s.ReleaseDate) < date));
        }
    }
}
