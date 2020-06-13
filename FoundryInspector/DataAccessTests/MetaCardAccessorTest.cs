using System;
using System.Collections.Generic;
using System.Text;
using ChiefOfTheFoundry.DataAccess;
using ChiefOfTheFoundry.Models;
using ChiefOfTheFoundry.MtgApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FoundryInspector.DataAccessTests
{
    [TestClass]
    public class CardManagerServiceTest
    {
        [TestMethod]
        public void GetCardByIdTest()
        {
            // Arrange
            MetaCardAccessor metacardAccessor = SharedTestSettings.GetMetaCardAccessor();

            // Act
            MetaCard metaCard = metacardAccessor.GetMetaCardById(SharedTestSettings.SAMPLE_METACARD_OBJECTID);

            // Assert
            Assert.IsNotNull(metaCard);
            Assert.IsTrue(metaCard.Name == SharedTestSettings.SAMPLE_METACARD_NAME);
        }
    }
}
