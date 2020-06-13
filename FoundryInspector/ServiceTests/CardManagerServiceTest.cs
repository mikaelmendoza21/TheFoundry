using ChiefOfTheFoundry.DataAccess;
using ChiefOfTheFoundry.Models;
using ChiefOfTheFoundry.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoundryInspector.ServiceTests
{
    [TestClass]
    public class CardManagerServiceTest
    {
        [TestMethod]
        public void GetMetaCardsByNameBeginningTest()
        {
            // Arrange
            string nameStartsWith = SharedTestSettings.SAMPLE_METACARD_NAME.Substring(0, 3);
            MetaCardAccessor metaCardAccessor = SharedTestSettings.GetMetaCardAccessor();
            CardManagerService cardManagerService = new CardManagerService(metaCardAccessor, null, null, null);

            // Act
            IEnumerable<MetaCard> metaCardsStartingWithSubstring = cardManagerService.GetMetaCardsByNameBeginning(nameStartsWith);

            // Assert
            Assert.IsNotNull(metaCardsStartingWithSubstring);
            Assert.IsTrue(metaCardsStartingWithSubstring.Count() > 0);

            List<MetaCard> result = metaCardsStartingWithSubstring.ToList();
            Assert.IsTrue(result.Any(c => c.Name == SharedTestSettings.SAMPLE_METACARD_NAME));
        }

        [TestMethod]
        public void GetMetaCardsByNameBeginningAsyncTest()
        {
            // Arrange
            string nameStartsWith = SharedTestSettings.SAMPLE_METACARD_NAME.Substring(0, 3);
            MetaCardAccessor metaCardAccessor = SharedTestSettings.GetMetaCardAccessor();
            CardManagerService cardManagerService = new CardManagerService(metaCardAccessor, null, null, null);

            // Act
            Task<List<MetaCard>> metaCardsStartingWithSubstring = cardManagerService.GetMetaCardsByNameBeginningAsync(nameStartsWith);

            // Assert
            Assert.IsNotNull(metaCardsStartingWithSubstring);
            List<MetaCard> result = metaCardsStartingWithSubstring.Result;
            Assert.IsTrue(result.Count > 0);
            Assert.IsTrue(result.Any(c => c.Name == SharedTestSettings.SAMPLE_METACARD_NAME));
        }
    }
}
