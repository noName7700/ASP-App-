using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.NonDomain
{
    public class OrganizationFilterProxy : IRegister<Organization>
    {
        public IRegister<Organization> register { get; }
        public OrganizationFilterProxy(IRegister<Organization> register)
        {
            this.register = register;
        }
        public async Task<List<Organization>> GetAll(Usercapture user, int id = 1)
        {
            if (user.roleid == 1)
                return (await register.GetAll(user)).Where(r => r.localityid == user.localityid).ToList();
            else if (user.roleid == 2)
                return (await register.GetAll(user)).ToList();
            else if (user.roleid == 5 || user.roleid == 6 || user.roleid == 7)
                return (await register.GetAll(user)).Where(r => r.Locality.municipalityid == user.municipalityid).ToList();
            else
                return new List<Organization>();
        }
    }
}
