﻿using AnXinSdk;
using log4net;
using Lumos.Common;
using Lumos.DAL;
using Lumos.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebAppApi.Models;
using WebAppApi.Models.Account;
using WebAppApi.Models.BankCard;
using WebAppApi.Models.CarService;
using WebAppApi.Models.Withdraw;

namespace WebAppApi.Controllers
{
    public class HomeController : Controller
    {
        private string key = "test";
        private string secret = "6ZB97cdVz211O08EKZ6yriAYrHXFBowC";
        private long timespan = (long)(DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalSeconds;
        private string host = "http://localhost:16665";
        //private string host = "http://112.74.179.185";

        private string YBS_key = "ybs_test";
        private string YBS_secret = "6ZB87cdVz222O08EKZ6yri8YrHXFBowA";


        public static string GetQueryString(Dictionary<string, string> parames)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parames);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            // 第二步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder("");  //签名字符串
            StringBuilder queryStr = new StringBuilder(""); //url参数
            if (parames == null || parames.Count == 0)
                return "";

            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key))
                {
                    queryStr.Append("&").Append(key).Append("=").Append(value);
                }
            }

            string s = queryStr.ToString().Substring(1, queryStr.Length - 1);

            return s;
        }


        public static string GetQueryString2(Dictionary<string, string> parames)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parames);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            // 第二步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder("");  //签名字符串
            StringBuilder queryStr = new StringBuilder(""); //url参数
            if (parames == null || parames.Count == 0)
                return "";

            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key))
                {
                    queryStr.Append("&").Append(key).Append("=").Append(value);
                }
            }

            string s = queryStr.ToString().Substring(1, queryStr.Length - 1);

            return s;
        }

        public string BulidOpendId(string merchantCode, string key, string secret)
        {

            byte[] result = Encoding.Default.GetBytes(merchantCode + key + secret);    //tbPass为输入密码的文本框  
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            string s_output = BitConverter.ToString(output).Replace("-", "");

            return s_output;
        }

        private decimal GetDecimal(decimal d)
        {
            return Math.Round(d, 2);
        }


        public ActionResult Index()
        {
            ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


            decimal a = GetDecimal(1.174m);
            decimal a1 = GetDecimal(1.175m);
            decimal a2 = GetDecimal(1.177m);

            //http://www.ys16888.com/yscar_wx/pos/redirect/pos_login.do?key=test&timestamp=1488265033&openId=0C528B67CCBEC807756E33702436B420&sign=7AA66D1A6D2AF9BAC72338D5C49A1E25,不需要URL Encode


            Dictionary<string, string> testUrlP = new Dictionary<string, string>();
            // testUrlP.Add("key", key);
            // testUrlP.Add("timestamp", timespan.ToString());


            string opendId = BulidOpendId("56032", key, secret);

            testUrlP.Add("openId", opendId);

            //string signStr = Md5Signature.Compute(key, secret, timespan, opendId);


            //   string abc = AnXin.GetCertInfoByImagePath("identityCard", @"D:\UplinkCarServicePlatform\UplinkCarServicePlatform\WebAppApi\xmlModel\身份证.jpg");

            // string ou = "";
            // string ou2 = "75c2caf1-dfcc-4f6f-8994-565ea1bc79f2";
            //string a = PassWordHelper.HashPassword("123456");
            // bool b = PassWordHelper.VerifyHashedPassword(a, "123456");



            ViewBag.Title = "Home Page";
            // 1482942631
            // Signature.IsRequestTimeout(1482912188);
            //string signStr = Signature.Compute("test", "6ZB97cdVz211O08EKZ6yriAYrHXFBowC", 1482913508, "userId=1&type=1");

            // http://112.74.179.185/api/account/Personal?deviceId=864895023160011&merchantId=20&userId=1001




            Dictionary<string, string> model = new Dictionary<string, string>();

            string userName = "uplink";
            string passWord = "888888";
            string newPassWord = "888888";
            string deviceId = "869612023700703";
            int userId = 1018;
            int merchantId = 26;
            int posMachineId = 1;
            //model.Add("支付结果", PayResultNotify());
            // model.Add("易办事销账", YBSReceiveNotify());


            // model.Add("支付结果确认", PayConfirm());

            //  model.Add("获取订单列表1", GetOrderList(userId, merchantId, 0, 0));
            //  model.Add("获取订单列表维修和定损", GetOrderList(1004, 23, 0, 0));
            //  model.Add("获取订单列表只定损", GetOrderList(1006, 25, 0, 0));


            // model.Add("获取应付订单", GetPayableList(userId, merchantId, 0));
            //  http://112.74.179.185/api/Order/GetDetails?productType=2013&merchantId=20&userId=1001&orderId=66
            //     model.Add("获取订单详情1", GetOrderDetails(userId, merchantId, 115, Enumeration.ProductType.InsureForCarForInsure));
            //     model.Add("获取订单详情2", GetOrderDetails(userId, merchantId, 121, Enumeration.ProductType.InsureForCarForClaim));

            //model.Add("提交理赔定损单1", SubmitEstimateList(userId, 87));
            // model.Add("提交理赔定损单2", SubmitEstimateList(1028, 34));


            //  model.Add("修改密码", ChangePassword(15, "123456", "123456"));

            //  model.Add("登录接口", Login(userName, passWord, "869612023700703"));

            ///model.Add("检查用户是否存在", CheckUserName("uplink"));
            ///
            //  model.Add("获取忘记密码短信", GetForgetPwdCode("uplink1", "15989287032"));


            // model.Add("获取添加子账户短信", GetAddChildAccountCode(userId, "15989287032"));

            //model.Add("添加子账户", AddChildAccount(userId, merchantId, "邱庆文", "15989287032", "bf1b3357-1276-44b5-8b19-0ceba67e23e3", "959790"));
            //model.Add("获取子账户列表", GetChildAccountList(userId, merchantId,0));


            // model.Add("获取续保列表", GetCarServiceRenewalList(userId, merchantId, 0));


            // model.Add("获取主页数据", GetAccoutHome(userId, merchantId));
            //  model.Add("获取更多推荐产品", GetMoreRecommend(userId, merchantId, 1));


            //model.Add("重置密码接口", ResetPassword(userName, newPassWord, "382001", "5e04ef95-ac41-43a9-942d-a2b41758aef2"));

            //  model.Add("获取保险方案", GetInsurePlan(userId));
            //model.Add("获取保险方案详情1", GetInsurePlanKind(1,1));
            //model.Add("获取保险方案详情2", GetInsurePlanKind(1, 2));
            //model.Add("获取保险方案详情3", GetInsurePlanKind(1, 3));
            //model.Add("获取保险方案详情4", GetInsurePlanKind(1, 4));
            // model.Add("获取保险方案详情5", GetInsurePlanKind(1, 5));
            //model.Add("获取车险公司", GetInsuranceCompany(1));



            //  model.Add("提交投保单", SubmitInsure(userId, merchantId, posMachineId));
            //  model.Add("提交续保单", SubmitRenewal(userId));
            //  model.Add("提交理赔需求1", SubmitClaim(userId, "邱大文", Enumeration.RepairsType.Estimate));
            // model.Add("提交理赔需求2", SubmitClaim(userId, "邱庆文", Enumeration.RepairsType.EstimateRepair));
            // model.Add("获取我的信息", GetPersonal(userId, merchantId, deviceId));
            //   model.Add("提现申请", WithdrawApply(userId, 1));
            //  model.Add("资金明细", GetTransactions(userId, merchantId, 0));
            //  model.Add("获取余额", GetBalance(userId, merchantId));

            //   model.Add("提现申请", WithdrawApply(userId, 1));
            //  model.Add("提现申请确认", WithdrawApplyConfirm(userId, 1));
            // model.Add("提交跟进的投保单", SubmitFollowInsure(userId, 112));
            //  model.Add("提交理赔定损单1", SubmitEstimateList(userId, 24));
            // model.Add("提交理赔定损单2", SubmitEstimateList(userId, 25));
            // model.Add("获取订单", GetOrder(15, 1, 0));


            // model.Add("获取Banner", GetBannerList(1,Enumeration.BannerType.MainHomeTop));
            // model.Add("获取Banner详细", GetBannerDetails("4"));

            // model.Add("获取全部服务", GetExtendedAppList(ExtendedAppType.All));



            //  model.Add("获取全部服务", GetExtendedAppList(ExtendedAppType.All));

            //  model.Add("获取车务服务", GetExtendedAppList(ExtendedAppType.MainHomeCarService));

            //  model.Add("获取推荐服务", GetExtendedAppList(ExtendedAppType.MainHomeRecommend));


            //   model.Add("获取推荐商品", ProductGetHomeRecommend(1,20,1));


            //  model.Add("获取商品", ProductGetList(1, 20, 101, 0,1));

            //model.Add("获取Banner", GetBannerList(Enumeration.BannerType.MainHomeTop));

            //model.Add("获取Banner详细", GetBannerDetails("4"));

            //model.Add("获取银行列表", GetBankList());

            //model.Add("获取银行卡列表", GetBankCardList(userId, merchantId));


            //model.Add("绑定银行卡", BindBankCard(userId, merchantId));

            //model.Add("解绑银行卡", RemoveBankCard(1,userId));
            //string body_data = "";

            //HttpUtil http = new HttpUtil();
            //int a = int.Parse("Dasd");

            //DateTime now = DateTime.Now;
            //var timesSpan = (now - new DateTime(1970, 1, 1, 0, 0, 0));

            //Dictionary<string, string> parames = new Dictionary<string, string>();
            //parames.Add("id", "1110");



            //string signStr = Signature.Compute("DSDADS3423424DFF", "6ZB97cdVz211O08EKZ6yriAYrHXFBowC", (long)timesSpan.TotalSeconds, Signature.GetQueryData(parames));

            //Dictionary<string, string> headers = new Dictionary<string, string>();
            //headers.Add("key", "DSDADS3423424DFF");
            //headers.Add("timestamp", ((long)timesSpan.TotalSeconds).ToString());
            //headers.Add("sign", signStr);


            //// string respon_data1 = http.HttpGet("http://localhost:1664/api/Banner/GetDetails?id=1110", headers);
            ////string respon_data2 = http.HttpGet("http://localhost:1664/api/Banner/GetList", headers);


            ////LoginModel model = new LoginModel();
            ////model.UserName = "ching";
            ////model.Password = "123456";
            ////string a = JsonConvert.SerializeObject(model);
            ////string respon_data3 = http.HttpPostJson("http://localhost:1664/api/Account/Login", Newtonsoft.Json.JsonConvert.SerializeObject(model), headers);


            //LoginModel model1 = new LoginModel();
            //model1.UserName = "chkhhkjhhhing01";
            //model1.Password = "12345jggjgjhjghggjg6";
            //model1.FuselageNumber = "das";
            //string a1 = JsonConvert.SerializeObject(model1);

            //string signStr1 = Signature.Compute("DSDADS3423424DFF", "6ZB97cdVz211O08EKZ6yriAYrHXFBowC", timespan, a1);

            //Dictionary<string, string> headers1 = new Dictionary<string, string>();
            //headers1.Add("key", "DSDADS3423424DFF");
            //headers1.Add("timestamp", ((long)timesSpan.TotalSeconds).ToString());
            //headers1.Add("sign", signStr1);

            //// string a1 = "a1=das&a2=323";

            //string respon_data4 = http.HttpPostJson("http://localhost:1664/api/Account/Login", a1, headers1);

            //// string a5 = "a1=das&a2=323";
            ////  string respon_data5 = http.HttpPostJson("http://localhost:1664/api/Account/Login", a1, headers);


            return View(model);
        }

        public static string stringSort(string str)
        {
            char[] chars = str.ToCharArray();
            List<string> lists = new List<string>();
            foreach (char s in chars)
            {
                lists.Add(s.ToString());
            }
            lists.Sort();//sort默认是从小到大的。显示123456789      

            str = "";
            foreach (string item in lists)
            {
                str += item;
            }
            return str;
        }

        public string YBSReceiveNotify()
        {
            CrossoffAccountRequest model = new CrossoffAccountRequest();


            // string a1 = JsonConvert.SerializeObject(model);
            string a1 = "{\"request\":{\"transactioncode\":\"02\",\"bankcardnumber\":\"  \",\"cardtype\":\"DE\",\"merchantcode\":\"000567\",\"channel\":\"02\",\"serialnumber\":\"2017022210577794\",\"terminalnumber\":\"9999999B\",\"money\":\"3\",\"paytype\":\"01\",\"datetime\":\"20170222170331\"}}";



            string signStr = Signature.Compute(YBS_key, YBS_secret, timespan, a1);


            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", YBS_key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpPostJson("" + host + "/api/YBS/ReceiveNotify", a1, headers);

            return result;

        }

        public string PayResultNotify()
        {
            PayResultModel model = new PayResultModel();


            // string a1 = JsonConvert.SerializeObject(model);

            string a1 = "{\"productType\":\"2011\",\"merchantId\":\"26\",\"userId\":\"1018\",\"orderSn\":\"CI1702281120222042\",\"orderId\":\"2042\",\"params\":{\"merchantId\":\"861440360120020\",\"amount\":\"000000414000\",\"terminalId\":\"9999999B\",\"batchNo\":\"000001\",\"merchantName\":\"银联测试商户\",\"issue\":\"null\",\"merchantNo\":\"null\",\"traceNo\":\"000017\",\"failureReason\":\"\",\"referenceNo\":\"022310580194\",\"type\":\"\",\"result\":\"1\",\"cardNo\":\"6212263602044931384\",\"merchantInfo\":{\"order_no\":\"CI1702281120222042\",\"insurance_company\":\"平安保险\",\"insurance_type\":\"\",\"customer_id_type\":\"01\",\"customer_id\":\"a\",\"customer_sex\":\"\",\"customer_name\":\"a\",\"customer_mobile_no\":\"\",\"customer_birthdate\":\"\",\"insurance_order_no\":\"aaa\",\"car_type\":\"a\",\"car_license\":\"a\",\"car_frame_no\":\"\",\"payer_id_type\":\"\",\"payer_id\":\"\",\"payer_name\":\"\",\"payer_mobile_no\":\"\",\"payer_address\":\"\",\"ybs_mer_code\":\"000567\",\"merchant_id\":\"861440360120020\",\"merchant_name\":\"\",\"phone_no\":\"\",\"cashier_id\":\"\",\"teller_id\":\"45567\"}}";

            string signStr = Signature.Compute(key, secret, timespan, a1);


            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpPostJson("" + host + "/api/Order/PayResultNotify", a1, headers);

            return result;

        }


        public string PayConfirm()
        {
            PayConfirmModel model = new PayConfirmModel();
            model.UserId = 1001;
            model.MerchantId = 20;
            model.OrderId = 217;
            model.OrderSn = "CI1702072325220217";
            model.ProductType = Enumeration.ProductType.InsureForCarForRenewal;
            model.Params = new { offerId = 232, shippingAddress = "jhjh" };


            //PayConfirmModel model = new PayConfirmModel();
            //model.UserId = 1096;
            //model.MerchantId = 111;
            //model.OrderId = 150;
            //model.OrderSn = "CI1702162039180266";
            //model.ProductType = Enumeration.ProductType.PosMachineDepositRent;
            //model.Params = new { rentMonths = 3 };


            string a1 = "{\"productType\":\"301\",\"merchantId\":\"46\",\"userId\":\"1048\",\"orderSn\":\"DR1702241513240331\",\"orderId\":\"331\",\"params\":{\"rentMonths\":\"12\",\"merchantInfo\":{}}}";

            // string a1 = JsonConvert.SerializeObject(model);

            string signStr = Signature.Compute(key, secret, timespan, a1);


            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpPostJson("" + host + "/api/Order/PayConfirm", a1, headers);

            return result;

        }


        public string GetAccoutHome(int userId, int merchantId)
        {
            Dictionary<string, string> parames = new Dictionary<string, string>();
            parames.Add("userId", userId.ToString());
            parames.Add("merchantId", merchantId.ToString());

            string signStr = Signature.Compute(key, secret, timespan, Signature.GetQueryData(parames));

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpGet("" + host + "/api/Account/Home?userId=" + userId + "&merchantId=" + merchantId, headers);

            return result;

        }

        public string GetMoreRecommend(int userId, int merchantId, int pageIndex)
        {
            Dictionary<string, string> parames = new Dictionary<string, string>();
            parames.Add("userId", userId.ToString());
            parames.Add("merchantId", merchantId.ToString());
            parames.Add("pageIndex", pageIndex.ToString());

            string signStr = Signature.Compute(key, secret, timespan, Signature.GetQueryData(parames));

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpGet("" + host + "/api/Product/GetMoreRecommend?userId=" + userId + "&merchantId=" + merchantId + "&pageIndex=" + pageIndex, headers);

            return result;

        }


        public string Login(string username, string password, string deviceId)
        {
            LoginModel model1 = new LoginModel();
            model1.UserName = username;
            model1.Password = password;
            model1.DeviceId = deviceId;
            string a1 = JsonConvert.SerializeObject(model1);

            string signStr = Signature.Compute(key, secret, timespan, a1);

            Dictionary<string, string> headers1 = new Dictionary<string, string>();
            headers1.Add("key", key);
            headers1.Add("timestamp", (timespan.ToString()).ToString());
            headers1.Add("sign", signStr);

            // string a1 = "a1=das&a2=323";
            HttpUtil http = new HttpUtil();
            string respon_data4 = http.HttpPostJson("" + host + "/api/Account/Login", a1, headers1);

            return respon_data4;
        }



        public string ResetPassword(string uesrName, string password, string validcode, string token)
        {
            ResetPasswordModel model = new ResetPasswordModel();
            model.UserName = uesrName;
            model.Password = password;
            model.Token = token;
            model.ValidCode = validcode;
            string a1 = JsonConvert.SerializeObject(model);

            string signStr = Signature.Compute(key, secret, timespan, a1);


            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpPostJson("" + host + "/api/Account/ResetPassword", a1, headers);

            return result;

        }

        public string GetForgetPwdCode(string userName, string phone)
        {
            WebAppApi.Models.Sms.GetForgetPwdCodeModel model = new WebAppApi.Models.Sms.GetForgetPwdCodeModel();
            model.UserName = userName;
            model.Phone = phone;
            string a1 = JsonConvert.SerializeObject(model);

            string signStr = Signature.Compute(key, secret, timespan, a1);


            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpPostJson("" + host + "/api/Sms/GetForgetPwdCode", a1, headers);

            return result;

        }


        public string GetAddChildAccountCode(int userId, string phone)
        {
            WebAppApi.Models.Sms.GetAddChildAccountCodeModel model = new WebAppApi.Models.Sms.GetAddChildAccountCodeModel();
            model.UserId = userId;
            model.AccountPhone = phone;
            string a1 = JsonConvert.SerializeObject(model);

            string signStr = Signature.Compute(key, secret, timespan, a1);


            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpPostJson("" + host + "/api/Sms/GetAddChildAccountCode", a1, headers);

            return result;

        }


        public string AddChildAccount(int userId, int merchantId, string fullName, string phone, string token, string validCode)
        {
            AddChildAccountModel model = new AddChildAccountModel();
            model.AccountFullName = fullName;
            model.MerchantId = merchantId;
            model.UserId = userId;
            model.AccountPhone = phone;
            model.AccountPassword = "888888";
            model.Token = token;
            model.ValidCode = validCode;

            string a1 = JsonConvert.SerializeObject(model);

            string signStr = Signature.Compute(key, secret, timespan, a1);


            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpPostJson("" + host + "/api/Account/AddChildAccount", a1, headers);

            return result;

        }

        public string GetBannerList(int userId, Enumeration.BannerType type)
        {
            Dictionary<string, string> parames = new Dictionary<string, string>();
            parames.Add("userId", userId.ToString());
            parames.Add("type", ((int)type).ToString());

            string signStr = Signature.Compute(key, secret, timespan, Signature.GetQueryData(parames));

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpGet("" + host + "/api/Banner/GetList?userId=" + userId.ToString() + "&type=" + ((int)type).ToString(), headers);

            return result;

        }

        public string GetBannerDetails(string id)
        {
            Dictionary<string, string> parames = new Dictionary<string, string>();
            parames.Add("id", id);

            string signStr = Signature.Compute(key, secret, timespan, Signature.GetQueryData(parames));

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpGet("" + host + "/api/Banner/GetDetails?id=" + id, headers);

            return result;

        }

        public string GetExtendedAppList(ExtendedAppType type)
        {
            Dictionary<string, string> parames = new Dictionary<string, string>();
            parames.Add("type", ((int)type).ToString());
            parames.Add("userId", ((int)type).ToString());
            string signStr = Signature.Compute(key, secret, timespan, Signature.GetQueryData(parames));

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpGet("" + host + "/api/ExtendedApp/GetList?type=" + ((int)type).ToString() + "&userId=" + ((int)type).ToString(), headers);

            return result;

        }

        public string GetBankList()
        {

            string signStr = Signature.Compute(key, secret, timespan, null);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpGet("" + host + "/api/Bank/GetList", headers);

            return result;

        }

        public string GetBankCardList(int userId, int merchantId)
        {
            Dictionary<string, string> parames = new Dictionary<string, string>();
            parames.Add("userId", userId.ToString());
            parames.Add("merchantId", merchantId.ToString());
            string signStr = Signature.Compute(key, secret, timespan, Signature.GetQueryData(parames));

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpGet("" + host + "/api/BankCard/GetList?userId=" + userId.ToString() + "&merchantId=" + merchantId, headers);

            return result;

        }

        public string BindBankCard(int userId, int merchantId)
        {

            BindBankCardModel model = new BindBankCardModel();
            model.UserId = userId;
            model.MerchantId = merchantId;
            model.BankId = 1;
            model.BankAccountPhone = "15989287032";
            model.BankAccountName = "邱庆文";
            model.BankAccountNo = "545553232321";
            string a1 = JsonConvert.SerializeObject(model);

            string signStr = Signature.Compute(key, secret, timespan, a1);


            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpPostJson("" + host + "/api/BankCard/Bind", a1, headers);

            return result;

        }

        public string RemoveBankCard(int id, int userId)
        {

            RemoveBankCardModel model = new RemoveBankCardModel();
            model.UserId = userId;
            model.Id = 1;
            string a1 = JsonConvert.SerializeObject(model);

            string signStr = Signature.Compute(key, secret, timespan, a1);


            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpPostJson("" + host + "/api/BankCard/Remove", a1, headers);

            return result;

        }

        public string ChangePassword(int userId, string oldPassword, string newPassword)
        {

            ChangePasswordModel model = new ChangePasswordModel();
            model.UserId = userId;
            model.OldPassword = oldPassword;
            model.NewPassword = newPassword;
            string a1 = JsonConvert.SerializeObject(model);

            string signStr = Signature.Compute(key, secret, timespan, a1);


            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpPostJson("" + host + "/api/Account/ChangePassword", a1, headers);

            return result;

        }

        public string GetInsurePlan(int userId)
        {

            Dictionary<string, string> parames = new Dictionary<string, string>();
            parames.Add("userId", userId.ToString());
            string signStr = Signature.Compute(key, secret, timespan, Signature.GetQueryData(parames));

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpGet("" + host + "/api/CarService/GetInsurePlan?userId=" + userId.ToString(), headers);

            return result;

        }

        public string GetInsurePlanKind(int userId, int insurePlanId)
        {

            Dictionary<string, string> parames = new Dictionary<string, string>();
            parames.Add("userId", userId.ToString());
            parames.Add("insurePlanId", insurePlanId.ToString());
            string signStr = Signature.Compute(key, secret, timespan, Signature.GetQueryData(parames));

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpGet("" + host + "/api/CarService/GetInsurePlanKind?userId=" + userId.ToString() + "&insurePlanId=" + insurePlanId.ToString(), headers);

            return result;

        }


        public string GetInsuranceCompany(int userId)
        {

            Dictionary<string, string> parames = new Dictionary<string, string>();
            parames.Add("userId", userId.ToString());
            string signStr = Signature.Compute(key, secret, timespan, Signature.GetQueryData(parames));

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpGet("" + host + "/api/CarService/GetInsuranceCompany?userId=" + userId.ToString(), headers);

            return result;

        }


        public string GetImagesBase64String(string url)
        {
            System.IO.MemoryStream m1 = new System.IO.MemoryStream();
            System.Drawing.Bitmap bp1 = new System.Drawing.Bitmap(url);
            bp1.Save(m1, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] b1 = m1.GetBuffer();
            string base64string1 = Convert.ToBase64String(b1);


            return base64string1;
        }

        public string SubmitInsure(int userId, int merchantId, int posMachineId)
        {

            string base64string1 = GetImagesBase64String(@"d:\a.jpg");
            string base64string2 = GetImagesBase64String(@"d:\b.jpg");
            string base64string3 = GetImagesBase64String(@"d:\c.jpg");
            string base64string4 = GetImagesBase64String(@"d:\d.jpg");

            SubmitInsureModel model = new SubmitInsureModel();
            model.UserId = userId;
            model.MerchantId = merchantId;
            model.PosMachineId = posMachineId;
            model.Type = Enumeration.ProductType.InsureForCarForInsure;
            model.InsurePlanId = 1;
            model.InsuranceCompanyId = new int[3] { 1, 2, 3 };


            List<InsureKindModel> insureKindModel = new List<InsureKindModel>();
            insureKindModel.Add(new InsureKindModel() { Id = 1, Value = "", Details = "", IsWaiverDeductible = false });
            insureKindModel.Add(new InsureKindModel() { Id = 2, Value = "", Details = "", IsWaiverDeductible = false });
            insureKindModel.Add(new InsureKindModel() { Id = 3, Value = "2000", Details = "", IsWaiverDeductible = true });
            insureKindModel.Add(new InsureKindModel() { Id = 4, Value = "20w", Details = "", IsWaiverDeductible = true });
            insureKindModel.Add(new InsureKindModel() { Id = 5, Value = "30w", Details = "", IsWaiverDeductible = true });
            insureKindModel.Add(new InsureKindModel() { Id = 6, Value = "40w", Details = "", IsWaiverDeductible = true });
            insureKindModel.Add(new InsureKindModel() { Id = 7, Value = "3000", Details = "" });
            insureKindModel.Add(new InsureKindModel() { Id = 8, Value = "国产", Details = "" });
            insureKindModel.Add(new InsureKindModel() { Id = 9, Value = "1000", Details = "" });
            insureKindModel.Add(new InsureKindModel() { Id = 10, Value = "2000", Details = "" });
            insureKindModel.Add(new InsureKindModel() { Id = 11, Value = "3000", Details = "" });
            insureKindModel.Add(new InsureKindModel() { Id = 12, Value = "4000", Details = "" });
            insureKindModel.Add(new InsureKindModel() { Id = 13, Value = "5000", Details = "购买机动" });
            insureKindModel.Add(new InsureKindModel() { Id = 14, Value = "100/天", Details = "" });
            insureKindModel.Add(new InsureKindModel() { Id = 15, Value = "6000", Details = "" });
            insureKindModel.Add(new InsureKindModel() { Id = 16, Value = "7000", Details = "" });
            insureKindModel.Add(new InsureKindModel() { Id = 17, Value = "8000", Details = "" });


            model.InsureKind = insureKindModel;
            model.ClientRequire = "帮忙买保险";

            string a1 = JsonConvert.SerializeObject(model);

            if (a1.IndexOf("ImgData") > -1)
            {
                int x = a1.IndexOf("ImgData");
                a1 = a1.Substring(0, x - 2);
                a1 += "}";
            }


            ImageModel CZ_CL_XSZ_Img = new ImageModel() { Type = ".jpg", Data = base64string1 };
            ImageModel CZ_SFZ_Img = new ImageModel() { Type = ".jpg", Data = base64string2 };
            ImageModel CCSJM_WSZM_Img = new ImageModel() { Type = ".jpg", Data = base64string3 };
            ImageModel YCZ_CLDJZ_Img = new ImageModel() { Type = ".jpg", Data = base64string4 };

            model.ImgData = new Dictionary<string, ImageModel>();

            model.ImgData.Add("CZ_CL_XSZ_Img", CZ_CL_XSZ_Img);
            model.ImgData.Add("CZ_SFZ_Img", CZ_SFZ_Img);
            model.ImgData.Add("CCSJM_WSZM_Img", CCSJM_WSZM_Img);
            model.ImgData.Add("YCZ_CLDJZ_Img", YCZ_CLDJZ_Img);

            string signStr = Signature.Compute(key, secret, timespan, a1);

            a1 = JsonConvert.SerializeObject(model);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpPostJson("" + host + "/api/CarService/SubmitInsure", a1, headers);

            return result;

        }

        public string SubmitRenewal(int userId)
        {

            string base64string1 = GetImagesBase64String(@"d:\a.jpg");
            string base64string2 = GetImagesBase64String(@"d:\b.jpg");
            string base64string3 = GetImagesBase64String(@"d:\c.jpg");
            string base64string4 = GetImagesBase64String(@"d:\d.jpg");

            SubmitInsureModel model = new SubmitInsureModel();
            model.UserId = userId;
            model.Type = Enumeration.ProductType.InsureForCarForRenewal;
            model.InsurePlanId = 1;
            model.InsuranceCompanyId = new int[1] { 1 };


            List<InsureKindModel> insureKindModel = new List<InsureKindModel>();
            insureKindModel.Add(new InsureKindModel() { Id = 1, Value = "", Details = "", IsWaiverDeductible = false });
            insureKindModel.Add(new InsureKindModel() { Id = 2, Value = "", Details = "", IsWaiverDeductible = false });
            insureKindModel.Add(new InsureKindModel() { Id = 3, Value = "2000", Details = "", IsWaiverDeductible = true });
            insureKindModel.Add(new InsureKindModel() { Id = 4, Value = "20w", Details = "", IsWaiverDeductible = true });
            insureKindModel.Add(new InsureKindModel() { Id = 5, Value = "30w", Details = "", IsWaiverDeductible = true });
            insureKindModel.Add(new InsureKindModel() { Id = 6, Value = "40w", Details = "", IsWaiverDeductible = true });
            insureKindModel.Add(new InsureKindModel() { Id = 7, Value = "3000", Details = "" });
            insureKindModel.Add(new InsureKindModel() { Id = 8, Value = "国产", Details = "" });
            insureKindModel.Add(new InsureKindModel() { Id = 9, Value = "1000", Details = "" });
            insureKindModel.Add(new InsureKindModel() { Id = 10, Value = "2000", Details = "" });
            insureKindModel.Add(new InsureKindModel() { Id = 11, Value = "3000", Details = "" });
            insureKindModel.Add(new InsureKindModel() { Id = 12, Value = "4000", Details = "" });
            insureKindModel.Add(new InsureKindModel() { Id = 13, Value = "5000", Details = "购买机动" });
            insureKindModel.Add(new InsureKindModel() { Id = 14, Value = "100/天", Details = "" });
            insureKindModel.Add(new InsureKindModel() { Id = 15, Value = "6000", Details = "" });
            insureKindModel.Add(new InsureKindModel() { Id = 16, Value = "7000", Details = "" });
            insureKindModel.Add(new InsureKindModel() { Id = 17, Value = "8000", Details = "" });


            model.InsureKind = insureKindModel;
            model.ClientRequire = "帮忙买保险";

            string a1 = JsonConvert.SerializeObject(model);

            if (a1.IndexOf("ImgData") > -1)
            {
                int x = a1.IndexOf("ImgData");
                a1 = a1.Substring(0, x - 2);
                a1 += "}";
            }


            ImageModel CZ_CL_XSZ_Img = new ImageModel() { Type = ".jpg", Data = base64string1 };
            ImageModel CZ_SFZ_Img = new ImageModel() { Type = ".jpg", Data = base64string2 };
            ImageModel CCSJM_WSZM_Img = new ImageModel() { Type = ".jpg", Data = base64string3 };
            ImageModel YCZ_CLDJZ_Img = new ImageModel() { Type = ".jpg", Data = base64string4 };

            model.ImgData = new Dictionary<string, ImageModel>();

            model.ImgData.Add("CZ_CL_XSZ_Img", CZ_CL_XSZ_Img);
            model.ImgData.Add("CZ_SFZ_Img", CZ_SFZ_Img);
            model.ImgData.Add("CCSJM_WSZM_Img", CCSJM_WSZM_Img);
            model.ImgData.Add("YCZ_CLDJZ_Img", YCZ_CLDJZ_Img);

            string signStr = Signature.Compute(key, secret, timespan, a1);

            a1 = JsonConvert.SerializeObject(model);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpPostJson("" + host + "/api/CarService/SubmitInsure", a1, headers);

            return result;

        }


        public string SubmitFollowInsure(int userId, int orderId)
        {


            string base64string1 = GetImagesBase64String(@"d:\a.jpg");
            string base64string2 = GetImagesBase64String(@"d:\b.jpg");
            string base64string3 = GetImagesBase64String(@"d:\c.jpg");
            string base64string4 = GetImagesBase64String(@"d:\d.jpg");

            SubmitFollowInsureModel model = new SubmitFollowInsureModel();
            model.UserId = userId;
            model.OrderId = orderId;


            string a1 = JsonConvert.SerializeObject(model);


            if (a1.IndexOf("ImgData") > -1)
            {
                int x = a1.IndexOf("ImgData");
                a1 = a1.Substring(0, x - 2);
                a1 += "}";
            }

            string signStr = Signature.Compute(key, secret, timespan, a1);


            ImageModel ZJ1_Img = new ImageModel() { Type = ".jpg", Data = base64string1 };
            ImageModel ZJ2_Img = new ImageModel() { Type = ".jpg", Data = base64string2 };
            ImageModel ZJ3_Img = new ImageModel() { Type = ".jpg", Data = base64string3 };
            ImageModel ZJ4_Img = new ImageModel() { Type = ".jpg", Data = base64string4 };

            model.ImgData = new Dictionary<string, ImageModel>();

            model.ImgData.Add("ZJ1_Img", ZJ1_Img);
            model.ImgData.Add("ZJ2_Img", ZJ2_Img);
            model.ImgData.Add("ZJ3_Img", ZJ3_Img);
            model.ImgData.Add("ZJ4_Img", ZJ4_Img);


            a1 = JsonConvert.SerializeObject(model);



            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpPostJson("" + host + "/api/CarService/SubmitFollowInsure", a1, headers);

            return result;

        }

        public string GetOrder(int userId, int merchantId, int status)
        {

            Dictionary<string, string> parames = new Dictionary<string, string>();
            parames.Add("userId", userId.ToString());
            parames.Add("merchantId", merchantId.ToString());
            parames.Add("status", status.ToString());
            string signStr = Signature.Compute(key, secret, timespan, Signature.GetQueryData(parames));

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpGet("" + host + "/api/Order/GetList?userId=" + userId.ToString() + "&merchantId=" + merchantId.ToString() + "&status= " + status.ToString(), headers);

            return result;

        }

        public string WithdrawApply(int userId, int bankCardId)
        {

            WithdrawApplyModel model1 = new WithdrawApplyModel();
            model1.UserId = userId;
            model1.BankCardId = bankCardId;
            model1.Confirm = true;
            Random r = new Random();
            int num = r.Next(1, 100);
            model1.Amount = num;
            string a1 = JsonConvert.SerializeObject(model1);

            string signStr = Signature.Compute(key, secret, timespan, a1);

            Dictionary<string, string> headers1 = new Dictionary<string, string>();
            headers1.Add("key", key);
            headers1.Add("timestamp", (timespan.ToString()).ToString());
            headers1.Add("sign", signStr);

            // string a1 = "a1=das&a2=323";
            HttpUtil http = new HttpUtil();
            string respon_data4 = http.HttpPostJson("" + host + "/api/Withdraw/Apply", a1, headers1);

            return respon_data4;

        }


        public string WithdrawApplyConfirm(int userId, int bankCardId)
        {

            WithdrawApplyModel model1 = new WithdrawApplyModel();
            model1.UserId = userId;
            model1.BankCardId = bankCardId;
            model1.Confirm = true;
            Random r = new Random();
            int num = r.Next(1, 100);
            model1.Amount = num;
            string a1 = JsonConvert.SerializeObject(model1);

            string signStr = Signature.Compute(key, secret, timespan, a1);

            Dictionary<string, string> headers1 = new Dictionary<string, string>();
            headers1.Add("key", key);
            headers1.Add("timestamp", (timespan.ToString()).ToString());
            headers1.Add("sign", signStr);

            // string a1 = "a1=das&a2=323";
            HttpUtil http = new HttpUtil();
            string respon_data4 = http.HttpPostJson("" + host + "/api/Withdraw/Apply", a1, headers1);

            return respon_data4;

        }



        public string GetTransactions(int userId, int merchantId, int pageIndex)
        {

            Dictionary<string, string> parames = new Dictionary<string, string>();
            parames.Add("userId", userId.ToString());
            parames.Add("merchantId", merchantId.ToString());
            parames.Add("pageIndex", pageIndex.ToString());
            string signStr = Signature.Compute(key, secret, timespan, Signature.GetQueryData(parames));

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpGet("" + host + "/api/Fund/GetTransactions?userId=" + userId.ToString() + "&merchantId=" + merchantId.ToString() + "&pageIndex=" + pageIndex.ToString(), headers);

            return result;

        }

        public string GetBalance(int userId, int merchantId)
        {
            Dictionary<string, string> parames = new Dictionary<string, string>();
            parames.Add("userId", userId.ToString());
            parames.Add("merchantId", merchantId.ToString());
            string signStr = Signature.Compute(key, secret, timespan, Signature.GetQueryData(parames));

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpGet("" + host + "/api/Fund/GetBalance?userId=" + userId.ToString() + "&merchantId=" + merchantId.ToString(), headers);

            return result;

        }


        public string CheckUserName(string userName)
        {

            Dictionary<string, string> parames = new Dictionary<string, string>();
            parames.Add("userName", userName.ToString());
            string signStr = Signature.Compute(key, secret, timespan, Signature.GetQueryData(parames));

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpGet("" + host + "/api/Account/CheckUserName?userName=" + userName.ToString(), headers);

            return result;
        }

        public string ProductGetHomeRecommend(int userId, int merchantId, int pageIndex)
        {

            Dictionary<string, string> parames = new Dictionary<string, string>();
            parames.Add("userId", userId.ToString());
            parames.Add("merchantId", merchantId.ToString());
            parames.Add("pageIndex", pageIndex.ToString());

            string signStr = Signature.Compute(key, secret, timespan, Signature.GetQueryData(parames));

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpGet("" + host + "/api/Product/GetMoreRecommend?userId=" + userId.ToString() + "&merchantId=" + merchantId.ToString() + "&pageIndex=" + pageIndex, headers);

            return result;
        }


        public string ProductGetList(int userId, int merchantId, int type, int pageIndex, int pricearea)
        {

            Dictionary<string, string> parames = new Dictionary<string, string>();
            parames.Add("userId", userId.ToString());
            parames.Add("merchantId", merchantId.ToString());
            parames.Add("pageIndex", pageIndex.ToString());
            parames.Add("type", type.ToString());
            parames.Add("priceArea", pricearea.ToString());
            string signStr = Signature.Compute(key, secret, timespan, Signature.GetQueryData(parames));

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpGet("" + host + "/api/Product/GetList?userId=" + userId.ToString() + "&merchantId=" + merchantId.ToString() + "&pageIndex=" + pageIndex + "&type=" + type + "&priceArea=" + pricearea, headers);

            return result;
        }



        public string SubmitClaim(int userId, string handPerson, Enumeration.RepairsType estimateRepair)
        {

            SubmitClaimModel model1 = new SubmitClaimModel();
            model1.UserId = userId;
            model1.RepairsType = estimateRepair;
            model1.InsuranceCompanyId = 1;
            model1.HandPerson = handPerson;
            model1.HandPersonPhone = "15989287032";
            model1.CarLicenseNumber = "粤AT88899";
            model1.ClientRequire = "我在附近修理";

            string a1 = JsonConvert.SerializeObject(model1);

            string signStr = Signature.Compute(key, secret, timespan, a1);

            Dictionary<string, string> headers1 = new Dictionary<string, string>();
            headers1.Add("key", key);
            headers1.Add("timestamp", (timespan.ToString()).ToString());
            headers1.Add("sign", signStr);

            // string a1 = "a1=das&a2=323";
            HttpUtil http = new HttpUtil();
            string respon_data4 = http.HttpPostJson("" + host + "/api/CarService/SubmitClaim", a1, headers1);

            return respon_data4;

        }

        public string SubmitEstimateList(int userId, int orderId)
        {
            string base64string1 = GetImagesBase64String(@"d:\a.jpg");


            SubmitEstimateListModel model = new SubmitEstimateListModel();
            model.UserId = userId;
            model.OrderId = orderId;

            string a1 = JsonConvert.SerializeObject(model);
            if (a1.IndexOf("ImgData") > -1)
            {
                int x = a1.IndexOf("ImgData");
                a1 = a1.Substring(0, x - 2);
                a1 += "}";
            }

            string signStr = Signature.Compute(key, secret, timespan, a1);

            ImageModel ZJ1_Img = new ImageModel() { Type = ".jpg", Data = base64string1 };

            model.ImgData = new Dictionary<string, ImageModel>();

            model.ImgData.Add("EstimateListImg", ZJ1_Img);

            a1 = JsonConvert.SerializeObject(model);



            Dictionary<string, string> headers1 = new Dictionary<string, string>();
            headers1.Add("key", key);
            headers1.Add("timestamp", (timespan.ToString()).ToString());
            headers1.Add("sign", signStr);

            // string a1 = "a1=das&a2=323";
            HttpUtil http = new HttpUtil();
            string respon_data4 = http.HttpPostJson("" + host + "/api/CarService/SubmitEstimateList", a1, headers1);

            return respon_data4;

        }

        public string GetPersonal(int userId, int merchantId, string deviceId)
        {

            Dictionary<string, string> parames = new Dictionary<string, string>();
            parames.Add("userId", userId.ToString());
            parames.Add("merchantId", merchantId.ToString());
            parames.Add("deviceId", deviceId.ToString());

            string signStr = Signature.Compute(key, secret, timespan, Signature.GetQueryData(parames));

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpGet("" + host + "/api/Account/Personal?userId=" + userId.ToString() + "&merchantId=" + merchantId + "&deviceId=" + deviceId, headers);

            return result;
        }

        public string GetChildAccountList(int userId, int merchantId, int pageIndex)
        {

            Dictionary<string, string> parames = new Dictionary<string, string>();
            parames.Add("userId", userId.ToString());
            parames.Add("merchantId", merchantId.ToString());
            parames.Add("pageIndex", pageIndex.ToString());

            string signStr = Signature.Compute(key, secret, timespan, Signature.GetQueryData(parames));

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpGet("" + host + "/api/Account/GetChildAccountList?userId=" + userId.ToString() + "&merchantId=" + merchantId + "&pageIndex=" + pageIndex, headers);

            return result;
        }

        public string GetCarServiceRenewalList(int userId, int merchantId, int pageIndex)
        {

            Dictionary<string, string> parames = new Dictionary<string, string>();
            parames.Add("userId", userId.ToString());
            parames.Add("merchantId", merchantId.ToString());
            parames.Add("pageIndex", pageIndex.ToString());

            string signStr = Signature.Compute(key, secret, timespan, Signature.GetQueryData(parames));

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpGet("" + host + "/api/CarService/GetRenewal?userId=" + userId.ToString() + "&merchantId=" + merchantId + "&pageIndex=" + pageIndex, headers);

            return result;
        }

        public string GetOrderList(int userId, int merchantId, int pageIndex, int status)
        {

            Dictionary<string, string> parames = new Dictionary<string, string>();
            parames.Add("userId", userId.ToString());
            parames.Add("merchantId", merchantId.ToString());
            parames.Add("pageIndex", pageIndex.ToString());
            parames.Add("status", status.ToString());
            string signStr = Signature.Compute(key, secret, timespan, Signature.GetQueryData(parames));

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpGet("" + host + "/api/Order/GetList?userId=" + userId.ToString() + "&merchantId=" + merchantId + "&pageIndex=" + pageIndex + "&status=" + status, headers);

            return result;
        }

        public string GetPayableList(int userId, int merchantId, int pageIndex)
        {

            Dictionary<string, string> parames = new Dictionary<string, string>();
            parames.Add("userId", userId.ToString());
            parames.Add("merchantId", merchantId.ToString());
            parames.Add("pageIndex", pageIndex.ToString());
            string signStr = Signature.Compute(key, secret, timespan, Signature.GetQueryData(parames));

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpGet("" + host + "/api/Order/GetPayableList?userId=" + userId.ToString() + "&merchantId=" + merchantId + "&pageIndex=" + pageIndex, headers);

            return result;
        }



        public string GetOrderDetails(int userId, int merchantId, int orderId, Enumeration.ProductType productType)
        {

            Dictionary<string, string> parames = new Dictionary<string, string>();
            parames.Add("userId", userId.ToString());
            parames.Add("merchantId", merchantId.ToString());
            parames.Add("orderId", orderId.ToString());
            parames.Add("productType", ((int)productType).ToString());
            string signStr = Signature.Compute(key, secret, timespan, Signature.GetQueryData(parames));

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("key", key);
            headers.Add("timestamp", timespan.ToString());
            headers.Add("sign", signStr);
            HttpUtil http = new HttpUtil();
            string result = http.HttpGet("" + host + "/api/Order/GetDetails?userId=" + userId.ToString() + "&merchantId=" + merchantId + "&orderId=" + orderId + "&productType=" + ((int)productType).ToString(), headers);

            return result;
        }


    }
}
