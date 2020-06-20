using ChiefOfTheFoundry.DataAccess;
using ChiefOfTheFoundry.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoundryInspector.DataAccessTests
{
    [TestClass]
    public class SetAccessorTest
    {
        [TestMethod]
        public void GetLatestSet()
        {
            // This test only works once Db has been seeded
            MtgSetAccessor setAccessor = SharedTestSettings.GetMtgSetAccessor();
            MtgSet set = setAccessor.GetLatestReleasedSet();

            Assert.IsNotNull(set);
            Assert.IsTrue(set.ReleaseDate > DateTime.Now.AddYears(-2));
        }
    }
}
