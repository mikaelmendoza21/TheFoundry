using System;
using System.Collections.Generic;
using System.Text;

namespace ChiefOfTheFoundry.Models
{
    /// <summary>
    /// Defines the basics of a MTG Card. All card have this shared definition.
    /// </summary>
    public abstract class MasterMtgCard
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public string ManaCost { get; set; }
    }
}
