using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiTest.Models
{
    public class RequestRecords
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Branch { get; set; }
        public string Fryer { get; set; }
        public string Quality { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string FoodType { get; set; }
        public string OilType { get; set; }
        public string FryerBrand { get; set; }
    }
}