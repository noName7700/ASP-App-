using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public class ActCapture
    {
        public DateTime DateCapture { get; private set; }
        public List<Animal> Animals { get; private set; }
        public Locality Locality { get; private set; }

        public ActCapture(Locality locality, DateTime dateCapture, string[] characters)
        {
            Animals = new List<Animal>();
            Locality = locality;
            DateCapture = dateCapture;
            foreach (var character in characters)
                Animals.Add(new Animal(character));
        }

        public void AddAnimal(string character)
        {
            Animals.Add(new Animal(character));
        }
    }
}
