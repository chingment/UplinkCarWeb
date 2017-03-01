using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAppApi.Models.Banner;
using Lumos.Entity;
using Lumos.Mvc;
using WebAppApi.Models.Fund;
using Lumos.Common;

namespace WebAppApi.Controllers
{
    public class FundController : BaseApiController
    {
        [HttpGet]
        public APIResponse GetBalance(int userId, int merchantId)
        {
            var fund = CurrentDb.Fund.Where(m => m.UserId == userId).FirstOrDefault();

            var bankCard = CurrentDb.BankCard.Where(m => m.UserId == userId).FirstOrDefault();

            BalanceResultModel resultModel = new BalanceResultModel();
            resultModel.Balance = fund.Balance;

            resultModel.BankCard = new WebAppApi.Models.Fund.BankCard();
            resultModel.BankCard.Id = bankCard.Id;
            resultModel.BankCard.BankName = bankCard.BankName;
            resultModel.BankCard.BankAccountNo = CommonUtils.GetBankAccountTail(bankCard.BankAccountNo);
 
            APIResult result = new APIResult() { Result = ResultType.Success, Code = ResultCode.Success, Message = "获取成功",Data= resultModel };
            return new APIResponse(result);
        }

        [HttpGet]
        public APIResponse GetTransactions(int userId, int merchantId, int pageIndex)
        {

            var query = (from m in CurrentDb.Transactions
                         where m.UserId == userId
                         select new { m.Id, m.Type, m.ChangeAmount, m.Balance, m.CreateTime });

            int total = query.Count();

            int pageSize = 10;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);


            List<object> list = new List<object>();

            foreach (var item in query)
            {
                list.Add(new
                {
                    Id = item.Id,
                    Type = item.Type.GetCnName(),
                    ChangeAmount = item.ChangeAmount,
                    Balance = item.Balance,
                    CreateTime = item.CreateTime
                });
            }

            APIResult result = new APIResult() { Result = ResultType.Success, Code = ResultCode.Success, Message = "", Data = list };

            return new APIResponse(result);
        }


    }
}