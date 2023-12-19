using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.NonDomain
{
    public interface IRegister<T>
    {
        Task<List<T>> GetAll(Usercapture user, int id = 1);
    }
}
