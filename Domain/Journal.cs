using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Journal
    {
        public int id { get; set; }
        public int nametable { get; set; }
        public int usercaptureid { get; set; }
        public Usercapture? Usercapture { get; set; }
        public DateTime datetimechange { get; set; }
        public int idobject { get; set; }
        public string description { get; set; }

    }
}
