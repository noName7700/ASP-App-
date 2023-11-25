using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class TaskMonth
    {
        public int id { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }    
        public int countanimal { get; set; }
        public int scheduleid {  get; set; }
        public Schedule? Schedule { get; set; }
    }
}
