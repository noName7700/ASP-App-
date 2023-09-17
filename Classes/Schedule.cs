using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public class Schedule
    {
        public Locality Locality { get; private set; }
        public List<TaskMonth> TasksMonth { get; private set; }
        public DateTime DateApproval { get; private set; }
        public Schedule(List<DateTime> startDates, List<DateTime> endDates, List<int> counts,
            Locality loc, DateTime dateApproval)
        {
            TasksMonth = new List<TaskMonth>();
            Locality = loc;
            DateApproval = dateApproval;
            for (int i = 0; i < startDates.Count; i++)
                TasksMonth.Add(new TaskMonth(startDates[i], endDates[i], counts[i]));
        }

        public void ChangeSchedule(List<DateTime> startDates, List<DateTime> endDates, List<int> counts, DateTime dateApproval)
        {
            DateApproval = dateApproval;
            TasksMonth.Clear();
            for (int i = 0; i < startDates.Count; i++)
                TasksMonth.Add(new TaskMonth(startDates[i], endDates[i], counts[i]));
        }

        public int GetCountPlanAnimal()
        {
            int countPlan = TasksMonth.Sum(task => task.CountAnimal);
            return countPlan;
        }
    }
}
