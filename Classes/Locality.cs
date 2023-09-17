using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public class Locality
    {
        public string Name { get; private set; }
        public double Tariph { get; private set; }

        public Locality(string name, double tariph)
        {
            Name = name;
            Tariph = tariph;
        }

        public void ChangeTariph(double tariph)
        {
            Tariph = tariph;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Locality);
        }

        public bool Equals(Locality locality)
        {
            return Name == locality.Name && Tariph == locality.Tariph;
        }
    }
