using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.NonDomain
{
    public class Contract_LocalityFilterProxy : IRegister<Contract_Locality>
    {
        public IRegister<Contract_Locality> register { get; }
        public Contract_LocalityFilterProxy(IRegister<Contract_Locality> register)
        {
            this.register = register;
        }

        public async Task<List<Contract_Locality>> GetAll(Usercapture user, int id)
        {
            if (user.roleid != 2)
                return (await register.GetAll(user, id)).Where(r => r.contractid == id && r.Contract.municipalityid == user.municipalityid).ToList();
            else if (user.roleid == 2)
                return (await register.GetAll(user, id)).ToList();
            else
                return new List<Contract_Locality>();
        }
    }
}
