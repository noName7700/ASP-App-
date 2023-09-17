using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public class TaskMonth
    {
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public int CountAnimal { get; private set; }

        public TaskMonth(DateTime startDate, DateTime endDate, int count)
        {
            StartDate = startDate;
            EndDate = endDate;
            CountAnimal = count;
        }
    }
}
