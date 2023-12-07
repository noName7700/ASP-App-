using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Organization
    {
        public int id { get; set; }
        public string name { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
        public int localityid { get; set; }
        public Locality? Locality { get; set; }
    }
}
