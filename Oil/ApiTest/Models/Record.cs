using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiTest.Models
{
    public class Record
    {
        public string HardwareId { get; set; }
        public string HardwareName { get; set; }
        public string HardwareBranch { get; set; }
        public string CountryISO { get; set; }
        public string City { get; set; }
        public string FryerNum { get; set; }
        public string Quality { get; set; }
        public string Password { get; set; }
        public DateTime DateTimeStamp { get; set; }
    }
}