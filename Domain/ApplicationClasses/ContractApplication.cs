using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ApplicationClasses
{
    internal class ContractApplication
    {
        //public Contract(DateTime validityPeriod, DateTime dateConclusion)
        //{
        //    Schedules = new List<Schedule>();
        //    ActsCapture = new List<ActCapture>();
        //    ValidityPeriod = validityPeriod;
        //    DateConclusion = dateConclusion;
        //}

        //public void CreateSchedule(List<DateTime> startDates, List<DateTime> endDates, List<int> counts,
        //    Locality locality, DateTime dateApproval)
        //{
        //    Schedules.Add(new Schedule(startDates, endDates, counts, locality, dateApproval));
        //}

        //public void ChangeSchedule(List<DateTime> startDates, List<DateTime> endDates, List<int> counts,
        //    Locality locality, DateTime dateApproval)
        //{
        //    Schedules.Find(sh => sh.Locality == locality).ChangeSchedule(startDates, endDates, counts, dateApproval);
        //}

        //public void RemoveSchedule(Locality locality)
        //{
        //    Schedules.RemoveAll(sh => sh.Locality == locality);
        //}

        //public void ChangeContract(DateTime validityPeriod, DateTime dateConclusion)
        //{
        //    ValidityPeriod = validityPeriod;
        //    DateConclusion = dateConclusion;
        //}

        //public void CreateActCapture(Locality locality, DateTime dateCapture, string[] characters)
        //{
        //    ActsCapture.Add(new ActCapture(locality, dateCapture, characters));
        //}

        //public void AddAnimal(Locality locality, DateTime dateCapture, string characters)
        //{
        //    ActCapture needAct = ActsCapture.Find(act => act.DateCapture == dateCapture && act.Locality == locality);
        //    needAct.AddAnimal(characters);
        //}

        //public double GetAmount(DateTime startDate, DateTime endDate)
        //{
        //    double totalSum = ActsCapture
        //        .Where(act => act.DateCapture >= startDate && act.DateCapture <= endDate)
        //        .Sum(act => act.Animals.Count * act.Locality.Tariph);
        //    return totalSum;
        //}

        //public int GetCountAnimal(Locality loc)
        //{
        //    int countAnimal = ActsCapture
        //        .Where(act => act.Locality == loc)
        //        .Sum(act => act.Animals.Count);
        //    return countAnimal;
        //}

        //public int CetCountPlanAnimal(Locality loc)
        //{
        //    Schedule schedule = Schedules.Find(sh => sh.Locality == loc);
        //    return schedule.GetCountPlanAnimal();
        //}
    }
}
