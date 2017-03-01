using Lumos.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppApi.Models.Bank;

namespace WebAppApi.Controllers
{
    public class BankController : BaseApiController
    {
        [HttpGet]
        public APIResponse GetList()
        {

            var banks = CurrentDb.Bank.ToList();

            List<BankModel> model = new List<BankModel>();

            foreach (var m in banks)
            {
                BankModel imageModel = new BankModel();
                imageModel.Id = m.Id;
                imageModel.Code = m.Code;
                imageModel.Name = m.Name;
                model.Add(imageModel);
            }

            APIResult result = new APIResult() { Result = ResultType.Success, Code = ResultCode.Success, Message = "成功", Data = model };
            return new APIResponse(result);
        }
    }
}