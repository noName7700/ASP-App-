using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ActCapture
    {
        public int id { get; set; }
        public DateTime datecapture { get; set; }
        public int idanimal { get; set; }
        public Animal? Animal { get; set; }
        public int idlocality { get; set; }
        public Locality? Locality { get; set; }
        public int idcontract { get; set; }
        public Contract? Contract { get; set; }
    }
}
