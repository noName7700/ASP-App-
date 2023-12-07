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
        public int contract_localityid { get; set; }
        public Contract_Locality? Contract_Locality { get; set; }
        public DateTime dateapproval { get; set; }
    }
}
