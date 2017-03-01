﻿using Lumos.BLL;
using Lumos.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAppApi.Models.Sms;


namespace WebAppApi.Controllers
{
    public class SmsController : BaseApiController
    {
        [HttpPost]
        public APIResponse GetForgetPwdCode(GetForgetPwdCodeModel model)
        {
            var clientUser = CurrentDb.SysClientUser.Where(m => m.UserName == model.UserName && m.PhoneNumber == model.Phone).FirstOrDefault();
            if (clientUser == null)
            {
                return ResponseResult(ResultType.Failure, ResultCode.Failure, "用户手机不正确");
            }
            string token = "";
            string validCode = "";

            IResult iResult = BizFactory.Sms.SendResetPasswordValidCode(clientUser.Id, clientUser.PhoneNumber, out validCode, out token);

            if (iResult.Result != ResultType.Success)
            {
                return ResponseResult(ResultType.Failure, ResultCode.Failure, "获取短信失败");
            }

            ResetPwdResultModel resultModel = new ResetPwdResultModel();
            resultModel.UserName = model.UserName;
            resultModel.ValidCode = validCode;
            resultModel.Token = token;

            return ResponseResult(ResultType.Success, ResultCode.Success, "获取成功", resultModel);
        }


        [HttpPost]
        public APIResponse GetAddChildAccountCode(GetAddChildAccountCodeModel model)
        {
            var clientUser = CurrentDb.SysClientUser.Where(m => m.UserName == model.AccountPhone).FirstOrDefault();
            if (clientUser != null)
            {
                return ResponseResult(ResultType.Failure, ResultCode.Failure, "该手机号码已经存在");
            }
            string token = "";
            string validCode = "";
            IResult isSuccess = BizFactory.Sms.SendAddChildAccountCode(model.UserId, model.AccountPhone, out validCode, out token);
            if (isSuccess.Result != ResultType.Success)
            {
                return ResponseResult(ResultType.Failure, ResultCode.Failure, "获取短信失败");
            }

            AddChildAccountCodeResultModel resultModel = new AddChildAccountCodeResultModel();
            resultModel.AccountPhone = model.AccountPhone;
            resultModel.ValidCode = validCode;
            resultModel.Token = token;

            return ResponseResult(ResultType.Success, ResultCode.Success, "获取成功", resultModel);
        }

    }
}