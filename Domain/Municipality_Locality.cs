using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Municipality_Locality
    {
        public int id { get; set; }
        public int munid { get; set; }
        public MunicipalityName? MunicipalityName { get; set; }
        public int localityid { get; set; }
        public Locality? Locality { get; set; }

        public Municipality_Locality(int munid, int localityid)
        {
            this.munid = munid;
            this.localityid = localityid;
        }
    }
}
