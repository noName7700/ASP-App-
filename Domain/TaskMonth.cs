using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class TaskMonth
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        public int countanimal { get; set; }

        //public TaskMonth(DateTime startDate, DateTime endDate, int countAnimal)
        //{
        //    startdate = startDate;
        //    enddate = endDate;
        //    countanimal = countAnimal;
        //}
    }
}
