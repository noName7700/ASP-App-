using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class ActCapture
    {
        public int Id { get; set; }
        public DateTime datecapture { get; set; }
        public int AnimalId { get; set; }
        public Animal Animal { get; set; }
        public int LocalityId { get; set; }
        public Locality Locality { get; set; }
    }
}
