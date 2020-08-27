using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace api.Controllers
{
    public class SqlController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet,JsonDateTimeFormat]
        public AjaxResult test(int i = 10) {
            return new AjaxResult
            {
                statusCode = 200,
                data = 2 *1.0 / i
            };
        }
    }
}
