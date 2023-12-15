using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Report
    {
        public int id { get; set; }
        public int numreport { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        public string localityname { get; set; }
        public double summ { get; set; }
        public int plancount { get; set; }
        public int factcount { get; set; }
        public DateTime datestatus { get; set; }
        public int statusid { get; set; }
        public Status? Status { get; set; }
        public int municipalityid { get; set; }
        public Municipality? Municipality { get; set; }
    }
}
