using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ApplicationClasses
{
    public class Register
    {
        //public RegMunicipality regMunicipality { get; private set; }
        //public Register()
        //{
        //    regMunicipality = new RegMunicipality();
        //}

        //public void AssignSchedule(string namemun, string nameloc, List<DateTime> startDates,
        //    List<DateTime> endDates, List<int> counts, DateTime dateApproval)
        //{
        //    if (startDates.Count != endDates.Count || endDates.Count != counts.Count)
        //        throw new ArgumentException("Количество периодов не совпадает с количеством животных");
        //    regMunicipality.AssignSchedule(namemun, nameloc, startDates, endDates, counts, dateApproval);
        //}

        //public void ChangeSchedule(string namemun, string nameloc, List<DateTime> startDates,
        //    List<DateTime> endDates, List<int> counts, DateTime dateApproval)
        //{
        //    if (startDates.Count != endDates.Count || endDates.Count != counts.Count)
        //        throw new ArgumentException("Количество периодов не совпадает с количеством животных");
        //    regMunicipality.ChangeSchedule(namemun, nameloc, startDates, endDates, counts, dateApproval);
        //}

        //public void RemoveSchedule(string namemun, string nameloc)
        //{
        //    regMunicipality.RemoveSchedule(namemun, nameloc);
        //}

        //public List<Schedule> GetSchedules(string namemun)
        //{
        //    return regMunicipality.GetSchedules(namemun);
        //}

        //public void CreateContract(string namemun, DateTime validityPeriod, DateTime dateConclusion)
        //{
        //    regMunicipality.CreateContract(namemun, validityPeriod, dateConclusion);
        //}

        //public void ChangeContract(string namemun, DateTime validityPeriod, DateTime dateConclusion)
        //{
        //    regMunicipality.ChangeContract(namemun, validityPeriod, dateConclusion);
        //}

        //public void RemoveContract(string namemun)
        //{
        //    regMunicipality.RemoveContract(namemun);
        //}

        //public Contract GetContract(string namemun)
        //{
        //    return regMunicipality.GetContract(namemun);
        //}

        //public void AddMunicipal(string name, List<Locality> localities = null)
        //{
        //    regMunicipality.CreateMunicipality(name, localities);
        //}

        //public void AddLocality(string namemun, string nameloc, double tariph)
        //{
        //    regMunicipality.AddLocality(namemun, nameloc, tariph);
        //}

        //public void ChangeTariph(string namemun, string nameloc, double tariph)
        //{
        //    regMunicipality.ChangeTariph(namemun, nameloc, tariph);
        //}

        //public List<Municipality> GetMunicipalities()
        //{
        //    return regMunicipality.Municipalities;
        //}

        //public List<Locality> GetLocalities(string namemun)
        //{
        //    return regMunicipality.GetLocalities(namemun);
        //}

        //public void CreateActCapture(string namemun, string nameloc, DateTime dateCapture, string[] characters)
        //{
        //    if (!CheckAllSchedules(namemun))
        //        throw new Exception("Для данного населенного пункта не составлен план-график");
        //    regMunicipality.CreateActCapture(namemun, nameloc, dateCapture, characters);
        //}

        //public void AddAnimal(string namemun, string nameloc, DateTime dateCapture, string characters)
        //{
        //    regMunicipality.AddAnimal(namemun, nameloc, dateCapture, characters);
        //}

        //public List<ActCapture> GetActCaptures(string namemun)
        //{
        //    return regMunicipality.GetActCaptures(namemun);
        //}

        //public double MakeReportMoney(DateTime stardDate, DateTime endDate, string namemun)
        //{
        //    if (!CheckAllSchedules(namemun))
        //        throw new Exception("Не для всех населенный пунктов составлен план-график");
        //    return regMunicipality.MakeReportMoney(stardDate, endDate, namemun);
        //}

        //public Dictionary<int, int> MakeReportSchedule(string namemun)
        //{
        //    if (!CheckAllSchedules(namemun))
        //        throw new Exception("Не для всех населенный пунктов составлен план-график");
        //    return regMunicipality.MakeReportSchedule(namemun);
        //}

        //public bool CheckAllSchedules(string namemun)
        //{
        //    return regMunicipality.CheckAllSchedules(namemun);
        //}
    }
}
