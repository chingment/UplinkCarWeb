using Lumos.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebAppApi.Controllers
{

    public class ValuesController : ApiController
    {
        // GET api/values
        public APIResponse Get()
        {
            int a = int.Parse("Dasd");
            List<object> ob = new List<object>();
            ob.Add(new { Name = "测试一", Age = new { NameAAA = new { das="@3"}, Date = DateTime.Now, price = 34.33 } });
            ob.Add(new { Name = "测试一", Age = "{\"result\":\"Exception\",\"code\":\"00001\",\"message\":\"NULL\"}" });
            ob.Add(new { Name = "测试一", Age = 20 });
            ob.Add(new { Name = "测试一", Age = 20 });
            ob.Add(new { Name = "测试一", Age = 20 });

            APIResult result = new APIResult() { Result = ResultType.Exception, Code = ResultCode.Failure, Message = "NULL", Data = ob };
            
            return new APIResponse(result);
        }

        // GET api/values/5
        public APIResponse Get(int id)
        {
            APIResult result = new APIResult() { Result = ResultType.Exception, Code = ResultCode.Failure, Message = "NULL" };

            return new APIResponse(result);
        }

        public APIResponse Post([FromBody]string value)
        {
            APIResult result = new APIResult() { Result = ResultType.Exception, Code = ResultCode.Failure, Message = "NULL" };

            return new APIResponse(result);
        }

    }
}
