using Lumos.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppApi.Models.PosMachine;

namespace WebAppApi.Controllers
{
    public class MerchantPosMachineController : BaseApiController
    {
        // GET: PosMachine
        public APIResponse GetStatus(int userId, string deviceId)
        {
            var posMachine = CurrentDb.PosMachine.Where(m => m.DeviceId == deviceId).FirstOrDefault();

            var merchantPosMachine = CurrentDb.MerchantPosMachine.Where(m => m.UserId == userId && m.PosMachineId == posMachine.Id).FirstOrDefault();

            StatusModel model = new StatusModel();
            model.Deposit = merchantPosMachine.Deposit;
            model.Rent = merchantPosMachine.Rent;
            model.RentDueDate = merchantPosMachine.RentDueDate.Value;
            model.Status = merchantPosMachine.Status;

            APIResult result = new APIResult() { Result = ResultType.Success, Code = ResultCode.Success, Message = "", Data = model };

            return new APIResponse(result);
        }
    }
}