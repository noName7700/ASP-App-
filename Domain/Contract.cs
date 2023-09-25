using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Contract
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public Schedule Schedule { get; set; }
        public int ActCaptureId { get; set; }
        public ActCapture ActCapture { get; set; }
        public DateTime ValidityPeriod { get; set; }
        public DateTime DateConclusion { get; set; }
    }
}
