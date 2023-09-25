using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBCommunication.Entities
{
    internal class ActCapture : BaseEntity
    {
        public DateTime DateCapture { get; private set; }
        public List<Animal> Animals { get; private set; }
        public Locality Locality { get; private set; }
    }
}
