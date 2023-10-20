using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Contract
    {
        public int id { get; set; }
        public DateTime validityperiod { get; set; }
        public DateTime dateconclusion { get; set; }
        public int municipalityid { get; set; }
        public Municipality? Municipality { get; set; }
    }
}
