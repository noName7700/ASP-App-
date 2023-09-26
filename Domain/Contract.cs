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
        public int? scheduleid { get; set; }
        public Schedule? Schedule { get; set; }
        public int? actcaptureid { get; set; }
        public ActCapture? ActCapture { get; set; }
        public DateTime validityperiod { get; set; }
        public DateTime dateconclusion { get; set; }
    }
}
