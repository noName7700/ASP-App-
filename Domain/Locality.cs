using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Locality : IComparable<Locality>, IComparable
    {
        public int id { get; set; }
        public string name { get; set; }
        public int municipalityid { get; set; }
        public Municipality? Municipality { get; set; }

        public int CompareTo(Locality? other)
        {
            return string.Compare(name, other.name);
        }

        public int CompareTo(object? obj)
        {
            Locality objMun = (Locality)obj;
            return this.CompareTo(objMun);
        }
    }
}
