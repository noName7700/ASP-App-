using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Contract_Locality : IComparable<Contract_Locality>, IComparable
    {
        public int id { get; set; }
        public int contractid { get; set; }
        public Contract? Contract { get; set; }
        public int localityid { get; set; }
        public Locality? Locality { get; set; }
        public double tariph { get; set; }
        public int organizationid { get; set; }
        public Organization? Organization { get; set; }

        public int CompareTo(Contract_Locality? other)
        {
            return Locality.CompareTo(other.Locality);
        }

        public int CompareTo(object? obj)
        {
            Contract_Locality objMun = (Contract_Locality)obj;
            return this.CompareTo(objMun);
        }
    }
}
