using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBCommunication.Entities
{
    internal class Municipality : BaseEntity
    {
        public string Name { get; private set; }
        private List<Contract> contracts = null;
        public List<Locality> Localities { get; private set; }
    }
}
