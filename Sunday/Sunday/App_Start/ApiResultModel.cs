using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Sunday.App_Start
{
    public class ApiResultModel
    {
        public HttpStatusCode code { get; set; }
        public Object data { get; set; }
        public string message { get; set; }
    }
}