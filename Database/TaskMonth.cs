using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class TaskMonth
    {
        public int Id { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        public int countanimal { get; set; }
    }
}
