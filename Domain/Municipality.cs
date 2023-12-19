using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Municipality : IComparable<Municipality>, IComparable
    {
        public int id { get; set; }
        public string name { get; set; }

        public int CompareTo(Municipality? other)
        {
            return string.Compare(name, other.name);
        }

        public int CompareTo(object? obj)
        {
            Municipality objMun = (Municipality)obj;
            return this.CompareTo(objMun);
        }
    }
}
