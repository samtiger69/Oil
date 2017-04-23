using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiTest.Models
{
    public class BaseResponse<T>
    {
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
    }
}