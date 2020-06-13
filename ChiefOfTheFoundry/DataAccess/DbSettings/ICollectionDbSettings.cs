using System;
using System.Collections.Generic;
using System.Text;

namespace ChiefOfTheFoundry.DataAccess
{
    public interface ICollectionDbSettings
    {
        string CollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
