using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Organization : IComparable<Organization>, IComparable
    {
        public int id { get; set; }
        public string name { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
        public int localityid { get; set; }
        public Locality? Locality { get; set; }

        public int CompareTo(Organization? other)
        {
            return string.Compare(name, other.name);
        }

        public int CompareTo(object? obj)
        {
            Organization objOrg = (Organization)obj;
            return this.CompareTo(objOrg);
        }
    }
}
