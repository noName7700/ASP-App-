using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBCommunication.Entities
{
    internal class Locality : BaseEntity
    {
        public string Name { get; private set; }
        public double Tariph { get; private set; }
    }
}
