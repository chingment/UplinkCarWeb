using Lumos.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppApi.Models.Account;


namespace WebAppApi.Controllers
{
    public class MerchantController : BaseApiController
    {
        [HttpGet]
        public APIResponse GetStatus()
        {
            APIResult result = new APIResult() { Result = ResultType.Exception, Code = ResultCode.Failure, Message = "NULL" };
            return new APIResponse(result);
        }

        [HttpPost]
        public APIResponse Activate()
        {
            APIResult result = new APIResult() { Result = ResultType.Exception, Code = ResultCode.Failure, Message = "NULL" };
            return new APIResponse(result);
        }
    }
}