using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Schedule
    { 
        public int id { get; set; }
        public int localityid { get; set; }
        public Locality? Locality { get; set; }
        public DateTime dateapproval { get; set; }
        public int contractid { get; set; }
        public Contract? Contract { get; set; }
    }
}
