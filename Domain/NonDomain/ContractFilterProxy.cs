using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.NonDomain
{
    public class ContractFilterProxy : IRegister<Contract>
    {
        public IRegister<Contract> register { get; }
        public ContractFilterProxy(IRegister<Contract> register)
        {
            this.register = register;
        }
        public async Task<List<Contract>> GetAll(Usercapture user, int id = 1)
        {
            if (user.roleid == 2)
                return (await register.GetAll(user)).ToList();
            else if (user.roleid != 2)
                return (await register.GetAll(user)).Where(c => c.municipalityid == user.municipalityid).ToList();
            else
                return new List<Contract>();
        }
    }
}
