using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public class Municipality
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ContractId { get; set; }
        public Contract Contract { get; set; }
        public int LocalityId { get; set; }
        public Locality Locality { get; set; }
    }
}
