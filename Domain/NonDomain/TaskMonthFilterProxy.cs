using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.NonDomain
{
    public class TaskMonthFilterProxy : IRegister<TaskMonth>
    {
        public IRegister<TaskMonth> register { get; }
        public TaskMonthFilterProxy(IRegister<TaskMonth> register)
        {
            this.register = register;
        }
        public async Task<List<TaskMonth>> GetAll(Usercapture user, int id = 1)
        {
            if (user.roleid == 1)
                return (await register.GetAll(user, id)).Where(r => r.Schedule.Contract_Locality.organizationid == user.organizationid).ToList();
            else if (user.roleid == 2)
                return (await register.GetAll(user, id)).ToList();
            else if (user.roleid == 5 || user.roleid == 6 || user.roleid == 7)
                return (await register.GetAll(user, id)).Where(r => r.Schedule.Contract_Locality.Contract.municipalityid == user.municipalityid).ToList();
            else
                return new List<TaskMonth>();
        }
    }
}
