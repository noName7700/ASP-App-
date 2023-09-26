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
        public int taskmonthid { get; set; }
        public TaskMonth? TaskMonth { get; set; }
        public DateTime dateapproval { get; set; }
    }
}
