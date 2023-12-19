using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Role : IComparable<Role>, IComparable
    {
        public int id {  get; set; }
        public string name { get; set; }
        public int CompareTo(Role? other)
        {
            return string.Compare(name, other.name);
        }

        public int CompareTo(object? obj)
        {
            Role objRol = (Role)obj;
            return this.CompareTo(objRol);
        }
    }
}
