using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBCommunication.Entities
{
    internal class Animal : BaseEntity
    {
        public string Category { get; private set; }
        public string Sex { get; private set; }
        public string Breed { get; private set; }
        public string Size { get; private set; }
        public string Wool { get; private set; }
        public string Color { get; private set; }
        public string Ears { get; private set; }
        public string Tail { get; private set; }
        public string SpecialSigns { get; private set; }
    }
}
