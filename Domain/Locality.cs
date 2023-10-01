using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Locality
    {
        public int id { get; set; }
        public string name { get; set; }
        public double tariph { get; set; }

        public Locality(string name, double tariph)
        {
            this.name = name;
            this.tariph = tariph;
        }
    }
}
