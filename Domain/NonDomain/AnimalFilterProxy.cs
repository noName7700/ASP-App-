using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.NonDomain
{
    public class AnimalFilterProxy : IRegister<Animal>
    {
        public IRegister<Animal> register { get; }
        public AnimalFilterProxy(IRegister<Animal> register)
        {
            this.register = register;
        }
        public async Task<List<Animal>> GetAll(Usercapture user, int id = 1)
        {
            if (user.roleid == 1)
                return (await register.GetAll(user, id)).Where(r => r.ActCapture.localityid == user.localityid).ToList();
            else if (user.roleid == 2)
                return (await register.GetAll(user, id)).ToList();
            else if (user.roleid == 5 || user.roleid == 6 || user.roleid == 7)
                return (await register.GetAll(user, id)).Where(r => r.ActCapture.Locality.municipalityid == user.municipalityid).ToList();
            else
                return new List<Animal>();
        }
    }
}
