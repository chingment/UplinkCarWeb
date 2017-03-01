using Lumos.BLL;
using Lumos.DAL;
using Lumos.DAL.AuthorizeRelay;
using Lumos.Entity;
using Lumos.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAppApi.Models;
using WebAppApi.Models.Account;
using WebAppApi.Models.Banner;
using WebAppApi.Models.Product;

namespace WebAppApi.Controllers
{
    public class AccountController : BaseApiController
    {
        [HttpGet]
        public APIResponse CheckUserName(string userName)
        {
            var clientUser = CurrentDb.SysClientUser.Where(m => m.UserName == userName).FirstOrDefault();
            if (clientUser == null)
            {
                return ResponseResult(ResultType.Failure, ResultCode.FailureUserNameNotExists, "用户名不存在");
            }

            return ResponseResult(ResultType.Success, ResultCode.Success, "有效用户名");
        }

        [HttpPost]
        public APIResponse ResetPassword(ResetPasswordModel model)
        {

            var clientUser = CurrentDb.SysClientUser.Where(m => m.UserName == model.UserName).FirstOrDefault();
            if (clientUser == null)
            {
                return ResponseResult(ResultType.Failure, ResultCode.FailureUserNameNotExists, "用户名不存在");
            }

            var token = CurrentDb.SysSmsSendHistory.Where(m => m.Token == model.Token && m.ValidCode == model.ValidCode && m.IsUse == false && m.ExpireTime >= DateTime.Now).FirstOrDefault();
            if (token == null)
            {
                return ResponseResult(ResultType.Failure, ResultCode.Failure, "验证码错误");
            }

            token.IsUse = true;


            clientUser.PasswordHash = PassWordHelper.HashPassword(model.Password);
            clientUser.LastUpdateTime = DateTime.Now;
            clientUser.Mender = clientUser.Id;
            CurrentDb.SaveChanges();

            return ResponseResult(ResultType.Success, ResultCode.Success, "重置成功");

        }

        [HttpPost]
        public APIResponse Login(LoginModel model)
        {
            var clientUser = CurrentDb.SysClientUser.Where(m => m.UserName == model.UserName).FirstOrDefault();
            if (clientUser == null)
            {
                return ResponseResult(ResultType.Failure, ResultCode.FailureSignIn, "登录失败，用户名不存在");
            }

            if (!PassWordHelper.VerifyHashedPassword(clientUser.PasswordHash, model.Password))
            {
                return ResponseResult(ResultType.Failure, ResultCode.FailureSignIn, "登录失败，用户密码错误");
            }

            LoginResultModel resultModel = new LoginResultModel(clientUser, model.DeviceId);

            return ResponseResult(ResultType.Success, ResultCode.Success, "登录成功", resultModel);
        }

        public APIResponse GetDetails()
        {
            APIResult result = new APIResult() { Result = ResultType.Exception, Code = ResultCode.Failure, Message = "NULL" };
            return new APIResponse(result);
        }

        public APIResponse Edit()
        {
            APIResult result = new APIResult() { Result = ResultType.Exception, Code = ResultCode.Failure, Message = "NULL" };
            return new APIResponse(result);
        }

        [HttpPost]
        public APIResponse ChangePassword(ChangePasswordModel model)
        {

            var clientUser = CurrentDb.SysClientUser.Where(m => m.Id == model.UserId).FirstOrDefault();
            if (clientUser == null)
            {
                return ResponseResult(ResultType.Failure, ResultCode.FailureUserNameNotExists, "用户名不存在");
            }

            if (!PassWordHelper.VerifyHashedPassword(clientUser.PasswordHash, model.OldPassword))
            {
                return ResponseResult(ResultType.Failure, ResultCode.Failure, "修改失败，旧密码错误");
            }

            clientUser.PasswordHash = PassWordHelper.HashPassword(model.NewPassword);
            clientUser.Mender = model.UserId;
            clientUser.LastUpdateTime = DateTime.Now;
            CurrentDb.SaveChanges();

            return ResponseResult(ResultType.Success, ResultCode.Success, "修改成功");
        }

        [HttpGet]
        public APIResponse GetChildAccountList(int userId, int merchantId, int pageIndex)
        {

            var query = (from m in CurrentDb.SysClientUser
                         where m.MerchantId == merchantId && m.ClientAccountType == Enumeration.ClientAccountType.SubAccount
                         select new { m.Id, m.FullName, m.PhoneNumber, m.UserName, m.Status, m.CreateTime });

            int total = query.Count();

            int pageSize = 10;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);


            List<object> list = new List<object>();

            foreach (var item in query)
            {
                list.Add(new
                {
                    Id = item.Id,
                    FullName = item.FullName,
                    Phone = item.PhoneNumber,
                    Status = GetClientStatus(item.Status)
                });
            }

            APIResult result = new APIResult() { Result = ResultType.Success, Code = ResultCode.Success, Message = "获取成功", Data = list };
            return new APIResponse(result);
        }


        private string GetClientStatus(Enumeration.UserStatus status)
        {
            string s = "";
            if (status == Enumeration.UserStatus.Disable)
            {
                return "已停用";
            }
            else if (status == Enumeration.UserStatus.Normal)
            {
                return "正常";
            }

            return s;
        }

        [HttpPost]
        public APIResponse AddChildAccount(AddChildAccountModel model)
        {
            var token = CurrentDb.SysSmsSendHistory.Where(m => m.Token == model.Token && m.ValidCode == model.ValidCode && m.IsUse == false && m.ExpireTime >= DateTime.Now).FirstOrDefault();
            if (token == null)
            {
                return ResponseResult(ResultType.Failure, ResultCode.Failure, "验证码错误");
            }


            var isExists = CurrentDb.SysClientUser.Where(m => m.UserName == model.AccountPhone).FirstOrDefault();
            if (isExists != null)
            {
                return ResponseResult(ResultType.Failure, ResultCode.Failure, "该手机号码已经存在");
            }

            token.IsUse = true;

            SysClientUser sysClientUser = new SysClientUser();
            sysClientUser.UserName = model.AccountPhone;
            sysClientUser.FullName = model.AccountFullName;
            sysClientUser.PhoneNumber = model.AccountPhone;
            sysClientUser.PasswordHash = PassWordHelper.HashPassword(model.AccountPassword);
            sysClientUser.SecurityStamp = Guid.NewGuid().ToString();
            sysClientUser.PhoneNumberConfirmed = true;
            sysClientUser.MerchantId = model.MerchantId;
            sysClientUser.RegisterTime = DateTime.Now;
            sysClientUser.ClientAccountType = Enumeration.ClientAccountType.SubAccount;
            sysClientUser.Status = Enumeration.UserStatus.Normal;
            sysClientUser.CreateTime = DateTime.Now;
            sysClientUser.Creator = model.UserId;
            CurrentDb.SysClientUser.Add(sysClientUser);
            CurrentDb.SaveChanges();


            APIResult result = new APIResult() { Result = ResultType.Success, Code = ResultCode.Success, Message = "添加成功" };
            return new APIResponse(result);
        }

        public APIResponse EditChildAccount(EditChildAccountModel model)
        {
            var sysClientUser = CurrentDb.SysClientUser.Where(m => m.Id == model.AccountId).FirstOrDefault();
            if (sysClientUser == null)
            {
                return ResponseResult(ResultType.Failure, ResultCode.Failure, "找不到该用户");
            }

            if (model.FieldName == "FullName")
            {
                sysClientUser.FullName = model.FieldValue;
            }
            if (model.FieldName == "Status")
            {
                if (model.FieldValue == "1")
                {
                    sysClientUser.Status = Enumeration.UserStatus.Normal;
                }
                else if (model.FieldValue == "2")
                {
                    sysClientUser.Status = Enumeration.UserStatus.Disable;
                }
            }

            CurrentDb.SaveChanges();

            APIResult result = new APIResult() { Result = ResultType.Success, Code = ResultCode.Success, Message = "保存成功" };
            return new APIResponse(result);
        }

        [HttpGet]
        public APIResponse Personal(int userId, int merchantId, string deviceId)
        {

            PersonalResultModel model = new PersonalResultModel();

            var posMachine = CurrentDb.PosMachine.Where(m => m.DeviceId == deviceId).FirstOrDefault();

            if (posMachine == null)
            {
                model.PosMachineStatus = Enumeration.MerchantPosMachineStatus.NotMatch;
                return ResponseResult(ResultType.Failure, ResultCode.Failure, "设备ID不存在");
            }



            var merchant = CurrentDb.Merchant.Where(m => m.Id == merchantId).FirstOrDefault();
            var userAccount = CurrentDb.SysClientUser.Where(m => m.Id == userId).FirstOrDefault();
            var merchantFund = CurrentDb.Fund.Where(m => m.UserId == merchant.UserId && m.MerchantId == merchant.Id).FirstOrDefault();
            var merchantPosMachine = CurrentDb.MerchantPosMachine.Where(m => m.MerchantId == merchantId && m.PosMachineId == posMachine.Id).FirstOrDefault();
            var merchantOrder = CurrentDb.Order.Where(m => m.UserId == merchant.UserId && m.MerchantId == merchant.Id && (m.Status == Enumeration.OrderStatus.Follow || m.Status == Enumeration.OrderStatus.WaitPay&& ((int)m.ProductType).ToString().StartsWith("201"))).ToList();

            //var 
            Log.Info("测试通过1");


            model.PosMachineStatus = merchantPosMachine.Status;

            PersonalInfo personalInfo = new PersonalInfo();

            personalInfo.PhoneNumber = userAccount.PhoneNumber.NullToEmpty();
            personalInfo.FullName = userAccount.FullName.NullToEmpty();
            personalInfo.MerchantName = merchant.YYZZ_Name.NullToEmpty();
            personalInfo.MerchantAddress = merchant.YYZZ_Address.NullToEmpty();

            model.PersonalInfo = personalInfo;

            model.WithdrawRuleUrl = BizFactory.Withdraw.GetWithrawRuleUrl();

            if (merchant != null)
            {
                //资金详细
                if (merchantFund != null)
                {
                    FundDetails fundDetails = new FundDetails();

                    fundDetails.Balance = merchantFund.Balance;
                    fundDetails.Arrearage = merchantFund.Arrearage;

                    model.Fund = fundDetails;
                }

                //订单数据
                if (merchantOrder != null)
                {
                    OrderStatusCount orderStatusCount = new OrderStatusCount();
                    orderStatusCount.Follow = merchantOrder.Where(m => m.Status == Enumeration.OrderStatus.Follow).Count();
                    orderStatusCount.WaitPay = merchantOrder.Where(m => m.Status == Enumeration.OrderStatus.WaitPay).Count();

                    model.OrderStatusCount = orderStatusCount;
                }
                Log.Info("测试通过2");
                //租金
                if (merchantPosMachine != null)
                {
                    model.RentDue = BizFactory.Merchant.GetRentOrder(merchant.Id, posMachine.Id);

                }
                Log.Info("测试通过3");

                //续保数量
                DateTime periodEndMax = DateTime.Now.AddDays(-7);
                model.RenewalCount = CurrentDb.OrderToCarInsure.Where(m => m.MerchantId == merchant.Id && m.Status == Enumeration.OrderStatus.Completed && m.PeriodEnd >= periodEndMax).Count();

            }

            model.CustomerPhone = "020-88888888";

            APIResult result = new APIResult() { Result = ResultType.Success, Code = ResultCode.Success, Message = "获取成功", Data = model };
            return new APIResponse(result);
        }

        [HttpGet]
        public APIResponse Home(int userId, int merchantId)
        {

            HomeModel model = new HomeModel();


            //获取bannder
            var banner = CurrentDb.SysBanner.Where(m => m.Type == Enumeration.BannerType.MainHomeTop).ToList();

            List<BannerImageModel> bannerImageModel = new List<BannerImageModel>();

            foreach (var m in banner)
            {
                BannerImageModel imageModel = new BannerImageModel();
                imageModel.Id = m.Id;
                imageModel.Title = m.Title;
                imageModel.LinkUrl = SysFactory.Banner.GetLinkUrl(m.Id);
                imageModel.ImgUrl = m.ImgUrl;
                bannerImageModel.Add(imageModel);
            }

            model.Banner = bannerImageModel;


            //获取车务服务
            var carServiceApps = CurrentDb.ExtendedApp.Where(m => m.Type == Enumeration.ExtendedAppType.CarService && m.IsDisplay == true).Take(4).ToList();

            List<ExtendedAppModel> carServiceAppsModel = new List<ExtendedAppModel>();

            foreach (var m in carServiceApps)
            {
                ExtendedAppModel extendedAppModel = new ExtendedAppModel();
                extendedAppModel.Id = m.Id;
                extendedAppModel.Name = m.Name;
                extendedAppModel.ImgUrl = m.ImgUrl;
                extendedAppModel.LinkUrl = m.LinkUrl;
                extendedAppModel.AppKey = m.AppKey;
                extendedAppModel.AppSecret = m.AppSecret;
                carServiceAppsModel.Add(extendedAppModel);
            }


            model.CarService = carServiceAppsModel;


            //获取推荐服务
            var recommendApps = CurrentDb.ExtendedApp.Where(m => m.Type != Enumeration.ExtendedAppType.CarService && m.IsDisplay == true).Take(4).ToList();

            List<ExtendedAppModel> recommendAppsModel = new List<ExtendedAppModel>();

            foreach (var m in recommendApps)
            {
                ExtendedAppModel extendedAppModel = new ExtendedAppModel();
                extendedAppModel.Id = m.Id;
                extendedAppModel.Name = m.Name;
                extendedAppModel.ImgUrl = m.ImgUrl;
                extendedAppModel.LinkUrl = m.LinkUrl;
                extendedAppModel.AppKey = m.AppKey;
                extendedAppModel.AppSecret = m.AppSecret;
                recommendAppsModel.Add(extendedAppModel);
            }


            model.RecommendService = recommendAppsModel;

            //获取推荐商品

            var recommendProduct = (from m in CurrentDb.Product where ((int)m.Type).ToString().StartsWith("101") select new { m.Id, m.Name, m.Price, m.Type, m.CreateTime, m.MainImgUrl }).OrderByDescending(r => r.CreateTime).Skip(0).Take(6).ToList();


            List<RecommendProductModel> recommendProductModel = new List<RecommendProductModel>();

            foreach (var m in recommendProduct)
            {
                RecommendProductModel product = new RecommendProductModel();

                product.Id = m.Id;
                product.Name = m.Name;
                product.Price = m.Price;
                product.Type = m.Type.GetCnName();
                product.ImgUrl = m.MainImgUrl;
                product.LinkUrl = BizFactory.Product.GetLinkUrl(m.Type, m.Id);
                recommendProductModel.Add(product);
            }

            model.RecommendProduct = recommendProductModel;


            APIResult result = new APIResult() { Result = ResultType.Success, Code = ResultCode.Success, Message = "获取成功", Data = model };
            return new APIResponse(result);
        }

    }
}