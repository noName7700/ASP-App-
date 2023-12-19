using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Status : IComparable<Status>, IComparable
    {
        public int id { get; set; }
        public string name { get; set; }

        public int CompareTo(Status? other)
        {
            return string.Compare(name, other.name);
        }

        public int CompareTo(object? obj)
        {
            Status objMun = (Status)obj;
            return this.CompareTo(objMun);
        }
    }
}
