using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class MunicipalityName
    {
        public int id { get; set; }
        public string name { get; set; }

        public MunicipalityName(string name)
        {
            this.name = name;
        }
    }
}
