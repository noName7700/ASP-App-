using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ApplicationClasses
{
    internal class MunicipalityApplication
    {
        //public Municipality(string name, List<Locality> localities)
        //{
        //    Name = name;
        //    Localities = localities;
        //    contracts = new List<Contract>();
        //}

        //public Contract Contract
        //{
        //    get
        //    {
        //        if (contracts.Count == 0)
        //            throw new Exception("С данным муниципалитетом не заключен контракт");
        //        else
        //        {
        //            Contract lastContract = contracts.Last();
        //            if (lastContract.ValidityPeriod <= DateTime.Now)
        //                throw new Exception("Срок действия контракта истек");
        //            return lastContract;
        //        }
        //    }
        //    private set { }
        //}

        //public List<Contract> Contracts
        //{
        //    get { return contracts; }
        //    private set { }
        //}

        //public void AddLocality(string name, double tariph)
        //{
        //    if (Localities == null)
        //        Localities = new List<Locality>();
        //    Localities.Add(new Locality(name, tariph));
        //}

        //public void ChangeTariph(string nameloc, double tariph)
        //{
        //    Locality locality = Localities.Find(loc => loc.Name == nameloc);
        //    locality.ChangeTariph(tariph);
        //}

        //public void AssignSchedule(string nameloc, List<DateTime> startDates, List<DateTime> endDates,
        //    List<int> counts, DateTime dateApproval)
        //{
        //    Locality locality = Localities.Find(loc => loc.Name == nameloc);
        //    Contract.CreateSchedule(startDates, endDates, counts, locality, dateApproval);
        //}

        //public void ChangeSchedule(string nameloc, List<DateTime> startDates, List<DateTime> endDates,
        //    List<int> counts, DateTime dateApproval)
        //{
        //    Locality locality = Localities.Find(loc => loc.Name == nameloc);
        //    Contract.ChangeSchedule(startDates, endDates, counts, locality, dateApproval);
        //}

        //public void RemoveSchedule(string nameloc)
        //{
        //    Locality locality = Localities.Find(loc => loc.Name == nameloc);
        //    Contract.RemoveSchedule(locality);
        //}

        //public void CreateContract(DateTime validityPeriod, DateTime dateConclusion)
        //{
        //    contracts.Add(new Contract(validityPeriod, dateConclusion));
        //}

        //public void ChangeContract(DateTime validityPeriod, DateTime dateConclusion)
        //{
        //    Contract.ChangeContract(validityPeriod, dateConclusion);
        //}

        //public void RemoveContract()
        //{
        //    contracts.RemoveAt(contracts.Count - 1);
        //}

        //public void CreateActCapture(string nameloc, DateTime dateCapture, string[] characters)
        //{
        //    Locality locality = Localities.Find(loc => loc.Name == nameloc);
        //    Contract.CreateActCapture(locality, dateCapture, characters);
        //}

        //public void AddAnimal(string nameloc, DateTime dateCapture, string characters)
        //{
        //    Locality locality = Localities.Find(loc => loc.Name == nameloc);
        //    Contract.AddAnimal(locality, dateCapture, characters);
        //}

        //public double MakeReportMoney(DateTime startDate, DateTime endDate)
        //{
        //    double totalSum = Contract.GetAmount(startDate, endDate);
        //    return totalSum;
        //}
        //public Dictionary<int, int> MakeReportSchedule()
        //{
        //    Dictionary<int, int> counts = new Dictionary<int, int>();
        //    foreach (var loc in Localities)
        //    {
        //        int countPlan = Contract.CetCountPlanAnimal(loc);
        //        int countCaught = Contract.GetCountAnimal(loc);
        //        counts.Add(countPlan, countCaught);
        //    }
        //    return counts;
        //}

        //public bool CheckAllSchedules()
        //{
        //    return Contract.Schedules.Count == Localities.Count;
        //}
    }
}
