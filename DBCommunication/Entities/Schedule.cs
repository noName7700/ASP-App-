using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBCommunication.Entities
{
    internal class Schedule : BaseEntity
    {
        public Locality Locality { get; private set; }
        public List<TaskMonth> TasksMonth { get; private set; }
        public DateTime DateApproval { get; private set; }
    }
}
