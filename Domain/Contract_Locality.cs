﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Contract_Locality
    {
        public int id { get; set; }
        public int idcontract { get; set; }
        public Contract? Contract { get; set; }
        public int idlocality { get; set; }
        public Locality? Locality { get; set; }
        public double tariph { get; set; }
    }
}