using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Municipality
    {
        public int id { get; set; }
        public int munid { get; set; }
        public MunicipalityName? MunicipalityName { get; set; }
        public int? contractid { get; set; }
        public Contract? Contract { get; set; }
        public int localityid { get; set; }
        public Locality? Locality { get; set; }
    }
}
