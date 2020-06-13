using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Models
{
    public class BaseViewModel
    {
        public string ApiHostname;

        public BaseViewModel(IConfiguration configuration)
        {
            ApiHostname = configuration.GetValue<string>("FoundryApiHostname");
        }
    }
}
