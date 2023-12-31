﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Usercapture
    {
        public int id { get; set; }
        public string surname { get; set; }
        public string name { get; set; }
        public string patronymic { get; set; }
        public int roleid { get; set; }
        public Role? Role { get; set; }
        public int municipalityid { get; set; }
        public Municipality? Municipality { get; set; }
        public int localityid { get; set; }
        public Locality? Locality { get; set; }
        public int organizationid { get; set; }
        public Organization? Organization { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public bool isadmin { get; set; }
    }
}
