using System;
using System.Collections.Generic;
using System.Text;

namespace ChiefOfTheFoundry.DataAccess
{
    public class CollectionDbSettings : ICollectionDbSettings
    {
        public string CollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
