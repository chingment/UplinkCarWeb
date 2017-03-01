using Lumos.BLL;
using Lumos.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppApi.Models.Withdraw;

namespace WebAppApi.Controllers
{
    public class WithdrawController: BaseApiController
    {
        [HttpPost]
        public APIResponse Apply(WithdrawApplyModel model)
        {
            IResult result = BizFactory.Withdraw.Apply(model.UserId, model.UserId, model.Confirm,model.BankCardId, model.Amount);
            return new APIResponse(result);
        }
    }
}