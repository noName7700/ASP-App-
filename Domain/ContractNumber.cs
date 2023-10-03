using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ContractNumber
    {
        public int id { get; set; }
        public DateTime validityperiod {  get; set; }
        public DateTime dateconclusion {  get; set; }

        public ContractNumber(DateTime validityperiod, DateTime dateconclusion)
        {
            this.validityperiod = validityperiod;
            this.dateconclusion = dateconclusion;
        }
    }
}
