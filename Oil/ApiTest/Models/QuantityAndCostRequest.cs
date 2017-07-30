using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiTest.Models
{
    public class QuantityAndCostRequest : FryerGetRequest
    {
        public string FryerId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}