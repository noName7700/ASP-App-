using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBCommunication.Entities
{
    internal class Contract : BaseEntity
    {
        public List<Schedule> Schedules { get; private set; }
        public List<ActCapture> ActsCapture { get; private set; }
        public DateTime ValidityPeriod { get; private set; }
        public DateTime DateConclusion { get; private set; }
    }
}
