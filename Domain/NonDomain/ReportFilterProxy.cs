using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.NonDomain
{
    public class ReportFilterProxy : IRegister<Report>
    {
        public IRegister<Report> register { get; }
        public ReportFilterProxy(IRegister<Report> register)
        {
            this.register = register;
        }
        public async Task<List<Report>> GetAll(Usercapture user, int id = 1)
        {
            if (user.roleid != 2)
                return (await register.GetAll(user, id)).Where(r => r.municipalityid == user.municipalityid).ToList();
            else if (user.roleid == 2)
                return (await register.GetAll(user, id)).ToList();
            else
                return new List<Report>();
        }
    }
}
