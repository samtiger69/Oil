using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ApiTest.Controllers
{
    public class OilController : ApiController
    {
        [HttpGet]
        public string SayHi()
        {
            return "hi";
        }
    }
}
