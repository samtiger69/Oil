using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiTest.Models
{
    public class Register : Login
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public string Branch { get; set; }
    }
}