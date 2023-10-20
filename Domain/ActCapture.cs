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
        public int animalid { get; set; }
        public Animal? Animal { get; set; }
        public int localityid { get; set; }
        public Locality? Locality { get; set; }
        public int contractid { get; set; }
        public Contract? Contract { get; set; }
    }
}
