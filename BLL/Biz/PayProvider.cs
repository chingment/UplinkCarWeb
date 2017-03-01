using Lumos.Entity;
using Lumos.Entity.AppApi;
using Lumos.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lumos.BLL
{
    public class YiBanShiOrderInfo
    {
        public string order_no { get; set; }

        public string insurance_company { get; set; }

        public string insurance_type { get; set; }

        public string customer_id_type { get; set; }

        public string customer_id { get; set; }

        public string customer_sex { get; set; }

        public string customer_name { get; set; }

        public string customer_mobile_no { get; set; }

        public string customer_birthdate { get; set; }

        public string insurance_order_no { get; set; }

        public string car_type { get; set; }

        public string car_license { get; set; }

        public string car_frame_no { get; set; }

        public string payer_id_type { get; set; }

        public string payer_id { get; set; }

        public string payer_name { get; set; }

        public string payer_mobile_no { get; set; }

        public string payer_address { get; set; }

        public string ybs_mer_code { get; set; }

        public string merchant_id { get; set; }

        public string merchant_name { get; set; }

        public string phone_no { get; set; }

        public string cashier_id { get; set; }

        public string teller_id { get; set; }
    }

    public class YiBanShiPayOrder
    {
        public YiBanShiPayOrder()
        {
            this.confirmField = new List<OrderField>();
        }

        public string prodcut { get; set; }

        public string transName { get; set; }

        public string amount { get; set; }

        public YiBanShiOrderInfo orderInfo { get; set; }

        public List<OrderField> confirmField
        {
            get;
            set;
        }

    }


    public enum ResultNotifyParty
    {
        [Remark("未知")]
        Unknow = 0,
        [Remark("App")]
        App = 1,
        [Remark("易办事")]
        Ybs = 2,
    }


    public class PayProvider : BaseProvider
    {

        private decimal GetPayDecimal(decimal d)
        {
            return Math.Round(d, 2);
        }

        public CustomJsonResult Confirm(int operater, PayConfirmModel model)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                YiBanShiPayOrder yOrder = new YiBanShiPayOrder();
                YiBanShiOrderInfo yOrderInfo = new YiBanShiOrderInfo();
                MerchantPosMachine merchantPosMachine = null;
                CalculateRent calculateRent = null;
                YbsMerchantModel ybs_mer = null;
                switch (model.ProductType)
                {
                    case Enumeration.ProductType.InsureForCarForInsure:
                    case Enumeration.ProductType.InsureForCarForRenewal:

                        #region  投保和续保
                        var orderToCarInsure = CurrentDb.OrderToCarInsure.Where(m => m.Sn == model.OrderSn).FirstOrDefault();

                        var payCarInsureConfirmParams = Newtonsoft.Json.JsonConvert.DeserializeObject<PayCarInsureConfirmParams>(model.Params.ToString());


                        var orderToCarInsureOfferCompany = CurrentDb.OrderToCarInsureOfferCompany.Where(m => m.Id == payCarInsureConfirmParams.OfferId).FirstOrDefault();

                        orderToCarInsure.InsuranceCompanyId = orderToCarInsureOfferCompany.InsuranceCompanyId;
                        orderToCarInsure.InsureImgUrl = orderToCarInsureOfferCompany.InsureImgUrl;
                        orderToCarInsure.InsuranceCompanyName = orderToCarInsureOfferCompany.InsuranceCompanyName;
                        orderToCarInsure.InsuranceOrderId = orderToCarInsureOfferCompany.InsuranceOrderId;
                        orderToCarInsure.CommercialPrice = orderToCarInsureOfferCompany.CommercialPrice == null ? 0 : orderToCarInsureOfferCompany.CommercialPrice.Value;
                        orderToCarInsure.TravelTaxPrice = orderToCarInsureOfferCompany.TravelTaxPrice == null ? 0 : orderToCarInsureOfferCompany.TravelTaxPrice.Value;
                        orderToCarInsure.CompulsoryPrice = orderToCarInsureOfferCompany.CompulsoryPrice == null ? 0 : orderToCarInsureOfferCompany.CompulsoryPrice.Value;

                        orderToCarInsure.ShippingAddress = payCarInsureConfirmParams.ShippingAddress;
                        orderToCarInsure.Price = orderToCarInsureOfferCompany.InsureTotalPrice.Value;

                        CurrentDb.SaveChanges();

                        var insuranceCompany = CurrentDb.InsuranceCompany.Where(m => m.Id == orderToCarInsureOfferCompany.InsuranceCompanyId).FirstOrDefault();


                        yOrder.prodcut = orderToCarInsure.ProductName;
                        yOrder.transName = "消费";
                        yOrder.amount = orderToCarInsure.Price.ToF2Price().Replace(".", "").PadLeft(12, '0');

                        yOrder.confirmField.Add(new OrderField("订单编号", orderToCarInsure.Sn.NullToEmpty()));
                        yOrder.confirmField.Add(new OrderField("保险公司", orderToCarInsure.InsuranceCompanyName.NullToEmpty()));
                        yOrder.confirmField.Add(new OrderField("车主姓名", orderToCarInsure.CarOwner.NullToEmpty()));
                        yOrder.confirmField.Add(new OrderField("车牌号码", orderToCarInsure.CarPlateNo.NullToEmpty()));
                        yOrder.confirmField.Add(new OrderField("支付金额", string.Format("{0}元", orderToCarInsure.Price.ToF2Price())));


                        yOrderInfo.order_no = orderToCarInsure.Sn.NullToEmpty();
                        yOrderInfo.insurance_company = orderToCarInsure.InsuranceCompanyName.NullToEmpty();
                        yOrderInfo.insurance_type = "";
                        yOrderInfo.customer_id_type = "01";
                        yOrderInfo.customer_id = orderToCarInsure.CarOwnerIdNumber.NullToEmpty();
                        yOrderInfo.customer_sex = "";
                        yOrderInfo.customer_name = orderToCarInsure.CarOwner.NullToEmpty();
                        yOrderInfo.customer_mobile_no = "";
                        yOrderInfo.customer_birthdate = "";
                        yOrderInfo.insurance_order_no = orderToCarInsure.InsuranceOrderId.NullToEmpty();
                        yOrderInfo.car_type = orderToCarInsure.CarVechicheType.NullToEmpty();
                        yOrderInfo.car_license = orderToCarInsure.CarPlateNo.NullToEmpty();
                        yOrderInfo.car_frame_no = "";
                        yOrderInfo.payer_id_type = "";
                        yOrderInfo.payer_id = "";
                        yOrderInfo.payer_name = "";
                        yOrderInfo.payer_mobile_no = "";
                        yOrderInfo.payer_address = "";


                        ybs_mer = BizFactory.Ybs.GetCarInsureMerchantInfo(insuranceCompany.Id, insuranceCompany.YBS_MerchantId, insuranceCompany.YBS_MerchantCode);

                        yOrderInfo.ybs_mer_code = ybs_mer.ybs_mer_code;
                        yOrderInfo.merchant_id = ybs_mer.merchant_id;

                        yOrderInfo.merchant_name = "";
                        yOrderInfo.phone_no = "";
                        yOrderInfo.cashier_id = "";
                        yOrderInfo.teller_id = "45567";//暂定

                        yOrder.orderInfo = yOrderInfo;


                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "确认成功", yOrder);
                        #endregion

                        break;
                    case Enumeration.ProductType.InsureForCarForClaim:

                        #region 理赔
                        var orderToCarClaim = CurrentDb.OrderToCarClaim.Where(m => m.Sn == model.OrderSn).FirstOrDefault();


                        yOrder.prodcut = orderToCarClaim.ProductName;
                        yOrder.transName = "消费";

                        yOrder.amount = orderToCarClaim.Price.ToF2Price().Replace(".", "").PadLeft(12, '0');

                        yOrder.confirmField.Add(new OrderField("订单编号", orderToCarClaim.Sn.NullToEmpty()));
                        yOrder.confirmField.Add(new OrderField("保险公司", orderToCarClaim.InsuranceCompanyName));
                        yOrder.confirmField.Add(new OrderField("车牌号码", orderToCarClaim.CarPlateNo));
                        yOrder.confirmField.Add(new OrderField("支付金额", string.Format("{0}元", orderToCarClaim.Price.NullToEmpty())));


                        yOrderInfo.order_no = orderToCarClaim.Sn;
                        yOrderInfo.customer_id_type = "";
                        yOrderInfo.customer_id = "";
                        yOrderInfo.customer_sex = "";
                        yOrderInfo.customer_name = "";
                        yOrderInfo.customer_mobile_no = "";
                        yOrderInfo.customer_birthdate = "";
                        yOrderInfo.payer_id_type = "";
                        yOrderInfo.payer_id = "";
                        yOrderInfo.payer_name = "";
                        yOrderInfo.payer_mobile_no = "";
                        yOrderInfo.payer_address = "";



                        ybs_mer = BizFactory.Ybs.GetCarClaimMerchantInfo();
                        yOrderInfo.ybs_mer_code = ybs_mer.ybs_mer_code;
                        yOrderInfo.merchant_id = ybs_mer.merchant_id;



                        yOrderInfo.merchant_name = "";
                        yOrderInfo.phone_no = "";
                        yOrderInfo.cashier_id = "";
                        yOrderInfo.teller_id = "";

                        yOrder.orderInfo = yOrderInfo;

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "确认成功", yOrder);
                        #endregion

                        break;
                    case Enumeration.ProductType.PosMachineDepositRent:

                        #region 押金租金


                        var payDepositRentParams = Newtonsoft.Json.JsonConvert.DeserializeObject<PayDepositRentConfirmParams>(model.Params.ToString());

                        var orderToDepositRent = CurrentDb.OrderToDepositRent.Where(m => m.Sn == model.OrderSn).FirstOrDefault();

                        merchantPosMachine = CurrentDb.MerchantPosMachine.Where(m => m.MerchantId == orderToDepositRent.MerchantId && m.PosMachineId == orderToDepositRent.PosMachineId).FirstOrDefault();

                        calculateRent = new CalculateRent(merchantPosMachine.Rent);

                        orderToDepositRent.MonthlyRent = calculateRent.MonthlyRent;
                        orderToDepositRent.RentMonths = payDepositRentParams.RentMonths;
                        orderToDepositRent.RentTotal = calculateRent.GetRent(payDepositRentParams.RentMonths);
                        orderToDepositRent.RentVersion = calculateRent.Version;


                        orderToDepositRent.Price = orderToDepositRent.Deposit + orderToDepositRent.RentTotal;
                        orderToDepositRent.RentDueDate = this.DateTime.AddMonths(payDepositRentParams.RentMonths);

                        yOrder.prodcut = orderToDepositRent.ProductName;
                        yOrder.transName = "消费";
                        yOrder.amount = orderToDepositRent.Price.ToF2Price().Replace(".", "").PadLeft(12, '0');

                        yOrder.confirmField.Add(new OrderField("订单编号", orderToDepositRent.Sn.NullToEmpty()));
                        yOrder.confirmField.Add(new OrderField("押金", string.Format("{0}元", orderToDepositRent.Deposit.ToF2Price())));
                        yOrder.confirmField.Add(new OrderField("租金", string.Format("{0}元", orderToDepositRent.RentTotal.ToF2Price())));
                        yOrder.confirmField.Add(new OrderField("到期时间", orderToDepositRent.RentDueDate.ToUnifiedFormatDate()));
                        yOrder.confirmField.Add(new OrderField("支付金额", string.Format("{0}元", orderToDepositRent.Price.NullToEmpty())));


                        yOrderInfo.order_no = orderToDepositRent.Sn;
                        yOrderInfo.customer_id_type = "";
                        yOrderInfo.customer_id = "";
                        yOrderInfo.customer_sex = "";
                        yOrderInfo.customer_name = "";
                        yOrderInfo.customer_mobile_no = "";
                        yOrderInfo.customer_birthdate = "";
                        yOrderInfo.payer_id_type = "";
                        yOrderInfo.payer_id = "";
                        yOrderInfo.payer_name = "";
                        yOrderInfo.payer_mobile_no = "";
                        yOrderInfo.payer_address = "";

                        ybs_mer = BizFactory.Ybs.GetDepositRentMerchantInfo();
                        yOrderInfo.ybs_mer_code = ybs_mer.ybs_mer_code;
                        yOrderInfo.merchant_id = ybs_mer.merchant_id;


                        yOrderInfo.merchant_name = "";
                        yOrderInfo.phone_no = "";
                        yOrderInfo.cashier_id = "";
                        yOrderInfo.teller_id = "";

                        yOrder.orderInfo = yOrderInfo;

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "确认成功", yOrder);
                        #endregion

                        break;
                    case Enumeration.ProductType.PosMachineRent:

                        #region 租金

                        var orderToRent = CurrentDb.OrderToDepositRent.Where(m => m.Sn == model.OrderSn).FirstOrDefault();

                        var payRentParams = Newtonsoft.Json.JsonConvert.DeserializeObject<PayDepositRentConfirmParams>(model.Params.ToString());

                        merchantPosMachine = CurrentDb.MerchantPosMachine.Where(m => m.MerchantId == orderToRent.MerchantId && m.PosMachineId == orderToRent.PosMachineId).FirstOrDefault();

                        calculateRent = new CalculateRent(merchantPosMachine.Rent);

                        orderToRent.MonthlyRent = calculateRent.MonthlyRent;
                        orderToRent.RentMonths = payRentParams.RentMonths;
                        orderToRent.RentTotal = calculateRent.GetRent(payRentParams.RentMonths);
                        orderToRent.RentVersion = calculateRent.Version;
                        orderToRent.Price = orderToRent.Deposit + orderToRent.RentTotal;
                        orderToRent.RentDueDate = merchantPosMachine.RentDueDate.Value.AddMonths(payRentParams.RentMonths);

                        yOrderInfo.order_no = orderToRent.Sn;
                        yOrder.transName = "消费";
                        yOrder.amount = orderToRent.Price.ToF2Price().Replace(".", "").PadLeft(12, '0');

                        yOrder.confirmField.Add(new OrderField("订单编号", orderToRent.Sn.NullToEmpty()));
                        yOrder.confirmField.Add(new OrderField("租金", string.Format("{0}元", orderToRent.RentTotal.ToF2Price())));
                        yOrder.confirmField.Add(new OrderField("续期", string.Format("{0}个月", payRentParams.RentMonths)));
                        yOrder.confirmField.Add(new OrderField("到期时间", orderToRent.RentDueDate.ToUnifiedFormatDate()));
                        yOrder.confirmField.Add(new OrderField("支付金额", string.Format("{0}元", orderToRent.Price.NullToEmpty())));


                        yOrderInfo.order_no = orderToRent.Sn;
                        yOrderInfo.customer_id_type = "";
                        yOrderInfo.customer_id = "";
                        yOrderInfo.customer_sex = "";
                        yOrderInfo.customer_name = "";
                        yOrderInfo.customer_mobile_no = "";
                        yOrderInfo.customer_birthdate = "";
                        yOrderInfo.payer_id_type = "";
                        yOrderInfo.payer_id = "";
                        yOrderInfo.payer_name = "";
                        yOrderInfo.payer_mobile_no = "";
                        yOrderInfo.payer_address = "";

                        ybs_mer = BizFactory.Ybs.GetDepositRentMerchantInfo();
                        yOrderInfo.ybs_mer_code = ybs_mer.ybs_mer_code;
                        yOrderInfo.merchant_id = ybs_mer.merchant_id;


                        yOrderInfo.merchant_name = "";
                        yOrderInfo.phone_no = "";
                        yOrderInfo.cashier_id = "";
                        yOrderInfo.teller_id = "";

                        yOrder.orderInfo = yOrderInfo;

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "确认成功", yOrder);
                        #endregion

                        break;
                }

                CurrentDb.SaveChanges();
                ts.Complete();
            }

            return result;
        }


        public CustomJsonResult ResultNotify(int operater, ResultNotifyParty notifyParty, object model)
        {
            CustomJsonResult result = new CustomJsonResult();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    switch (notifyParty)
                    {
                        case ResultNotifyParty.App:
                            result = App_ResultNotify(operater, (PayResultModel)model);
                            break;
                        case ResultNotifyParty.Ybs:
                            result = YBS_ResultNotify(operater, (YBS_ReceiveNotifyLog)model);
                            break;
                    }

                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("{0}订单号结果反馈发生异常,原因：{1}", notifyParty.GetCnName(), ex.StackTrace);

                result = new CustomJsonResult(ResultType.Exception, ResultCode.Exception, "支付失败");
            }

            return result;

        }

        private CustomJsonResult App_ResultNotify(int operater, PayResultModel model)
        {
            CustomJsonResult result = new CustomJsonResult();

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    OrderPayResultNotifyLog resultLog = new OrderPayResultNotifyLog();
                    resultLog.SysOrderId = model.OrderId;
                    resultLog.SysOrderSn = model.OrderSn;
                    resultLog.CreateTime = this.DateTime;
                    resultLog.Creator = model.UserId;

                    if (model.Params.Result == PayResultType.Success)
                    {
                        resultLog.Result = "1";
                        resultLog.OrderNo = model.Params.MerchantInfo.order_no.NullStringToNullObject();
                        resultLog.MerchantId = model.Params.MerchantId.NullStringToNullObject();
                        resultLog.Amount = model.Params.Amount.NullStringToNullObject();
                        resultLog.TerminalId = model.Params.TerminalId.NullStringToNullObject();
                        resultLog.MerchantNo = model.Params.MerchantNo.NullStringToNullObject();
                        resultLog.BatchNo = model.Params.BatchNo.NullStringToNullObject();
                        resultLog.MerchantName = model.Params.MerchantName.NullStringToNullObject();
                        resultLog.Issue = model.Params.Issue.NullStringToNullObject();
                        resultLog.TraceNo = model.Params.TraceNo.NullStringToNullObject();
                        resultLog.ReferenceNo = model.Params.ReferenceNo.NullStringToNullObject();
                        resultLog.Type = model.Params.Type.NullStringToNullObject();
                        resultLog.CardNo = model.Params.CardNo.NullStringToNullObject();


                        switch (model.ProductType)
                        {
                            case Enumeration.ProductType.InsureForCarForInsure:
                            case Enumeration.ProductType.InsureForCarForRenewal:

                                result = PayCarInsureCompleted(operater, model.OrderSn);

                                break;
                            case Enumeration.ProductType.InsureForCarForClaim:

                                result = PayCarClaimCompleted(operater, model.OrderSn);

                                break;
                            case Enumeration.ProductType.PosMachineDepositRent:

                                result = PayDepositRentCompleted(operater, model.OrderSn);

                                break;
                            case Enumeration.ProductType.PosMachineRent:

                                result = PayRentCompleted(operater, model.OrderSn);

                                break;
                        }

                    }
                    else if (model.Params.Result == PayResultType.Failure)
                    {

                        resultLog.Result = "2";
                        resultLog.FailureReason = model.Params.FailureReason;

                        Log.WarnFormat("订单:{0},支付失败，原因：{1}", model.OrderSn, model.Params.FailureReason);

                        result = new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付失败");
                    }

                    CurrentDb.OrderPayResultNotifyLog.Add(resultLog);
                    CurrentDb.SaveChanges();

                    ts.Complete();

                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("订单号({0})结果反馈发生异常，原因：{1}", model.OrderSn, ex.StackTrace);

                result = new CustomJsonResult(ResultType.Exception, ResultCode.Exception, "支付失败");
            }

            return result;
        }

        private CustomJsonResult YBS_ResultNotify(int operater, YBS_ReceiveNotifyLog receiveNotifyLog)
        {
            CustomJsonResult result = new CustomJsonResult();

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    YBS_CrossoffAccountRespone responeModel = new YBS_CrossoffAccountRespone();

                    receiveNotifyLog.CreateTime = this.DateTime;
                    receiveNotifyLog.Creator = operater;


                    var order = CurrentDb.Order.Where(m => m.Sn == receiveNotifyLog.Paymentnumber).FirstOrDefault();
                    if (order != null)
                    {
                        responeModel.serialnumber = receiveNotifyLog.Serialnumber;
                        responeModel.transactioncode = "02";
                        responeModel.datetime = receiveNotifyLog.Datetime;
                        responeModel.merchantcode = "654321";
                        responeModel.money = order.Price.ToF2Price();
                        responeModel.paymentnumber = order.Sn;
                        responeModel.username = order.Contact;
                        responeModel.username2 = "";
                        responeModel.businessname = "";
                        responeModel.returncode = "01";
                        responeModel.returnmsg = "交易成功";

                        switch (order.ProductType)
                        {
                            case Enumeration.ProductType.InsureForCarForInsure:
                            case Enumeration.ProductType.InsureForCarForRenewal:

                                result = PayCarInsureCompleted(operater, order.Sn);

                                break;
                            case Enumeration.ProductType.InsureForCarForClaim:

                                result = PayCarClaimCompleted(operater, order.Sn);

                                break;
                            case Enumeration.ProductType.PosMachineDepositRent:

                                result = PayDepositRentCompleted(operater, order.Sn);

                                break;
                            case Enumeration.ProductType.PosMachineRent:

                                result = PayRentCompleted(operater, order.Sn);

                                break;
                        }
                    }

                    CurrentDb.YBS_ReceiveNotifyLog.Add(receiveNotifyLog);
                    CurrentDb.SaveChanges();

                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("订单号({0})结果反馈发生异常，易办事调用原因：{1}", receiveNotifyLog.Paymentnumber, ex.StackTrace);

                result = new CustomJsonResult(ResultType.Exception, ResultCode.Exception, "支付失败");
            }

            return result;
        }

        private CustomJsonResult PayCarInsureCompleted(int operater, string orderSn)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var orderToCarInsure = CurrentDb.OrderToCarInsure.Where(m => m.Sn == orderSn).FirstOrDefault();


                if (orderToCarInsure.Status == Enumeration.OrderStatus.Completed)
                {
                    ts.Complete();
                    return new CustomJsonResult(ResultType.Success, ResultCode.Success, "该订单已经支付完成");
                }

                if (orderToCarInsure.Status != Enumeration.OrderStatus.WaitPay)
                {
                    ts.Complete();
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单未在就绪支付状态");
                }

                var commissionRate = CurrentDb.CarInsureCommissionRate.Where(m => (m.Type == Enumeration.CommissionRateType.Uplink && m.ReferenceId == orderToCarInsure.InsuranceCompanyId) || (m.Type == Enumeration.CommissionRateType.YiBanShi && m.ReferenceId == 1) || (m.Type == Enumeration.CommissionRateType.InsuranceCompany && m.ReferenceId == orderToCarInsure.InsuranceCompanyId)).ToList();


                var uplink_CommissionRate = commissionRate.Where(m => m.Type == Enumeration.CommissionRateType.Uplink).FirstOrDefault();


                var uplink_Commission_Commercial = orderToCarInsure.CommercialPrice * (uplink_CommissionRate.Commercial / 100);
                var uplink_Commission_Compulsory = orderToCarInsure.CompulsoryPrice * (uplink_CommissionRate.Compulsory / 100);
                var uplink_Commission = GetPayDecimal(uplink_Commission_Commercial + uplink_Commission_Compulsory);

                var uplinkFund = CurrentDb.Fund.Where(m => m.UserId == (int)Enumeration.UserAccount.Uplink).FirstOrDefault();

                uplinkFund.Balance += uplink_Commission;
                uplinkFund.Mender = operater;
                uplinkFund.LastUpdateTime = this.DateTime;

                var uplinkFundTrans = new Transactions();
                uplinkFundTrans.UserId = uplinkFund.UserId;
                uplinkFundTrans.ChangeAmount = uplink_Commission;
                uplinkFundTrans.Balance = uplinkFund.Balance;
                uplinkFundTrans.Type = Enumeration.TransactionsType.CarInsureCommission;
                uplinkFundTrans.Description = string.Format("投保订单佣金收益，交强险:{0}元，商业险:{1}:元，合计:{2}元", uplink_Commission_Compulsory.ToPrice(), uplink_Commission_Commercial.ToPrice(), uplink_Commission.ToPrice());
                uplinkFundTrans.Creator = operater;
                uplinkFundTrans.CreateTime = this.DateTime;
                CurrentDb.Transactions.Add(uplinkFundTrans);
                CurrentDb.SaveChanges();
                uplinkFundTrans.Sn = Sn.Build(SnType.Transactions, uplinkFundTrans.Id);
                CurrentDb.SaveChanges();

                var yiBanShi_CommissionRate = commissionRate.Where(m => m.Type == Enumeration.CommissionRateType.YiBanShi).FirstOrDefault();


                var yiBanShi_Commission_Commercial = orderToCarInsure.CommercialPrice * (yiBanShi_CommissionRate.Commercial / 100);
                var yiBanShi_Commission_Compulsory = orderToCarInsure.CompulsoryPrice * (yiBanShi_CommissionRate.Compulsory / 100);
                var yiBanShi_Commission = GetPayDecimal(yiBanShi_Commission_Commercial + yiBanShi_Commission_Compulsory);

                var YiBanShiFund = CurrentDb.Fund.Where(m => m.UserId == (int)Enumeration.UserAccount.YiBanShi).FirstOrDefault();


                YiBanShiFund.Balance += uplink_Commission;
                YiBanShiFund.Mender = operater;
                YiBanShiFund.LastUpdateTime = this.DateTime;

                var YiBanShiFundTrans = new Transactions();
                YiBanShiFundTrans.UserId = YiBanShiFund.UserId;
                YiBanShiFundTrans.ChangeAmount = yiBanShi_Commission;
                YiBanShiFundTrans.Balance = YiBanShiFund.Balance;
                YiBanShiFundTrans.Type = Enumeration.TransactionsType.CarInsureCommission;
                YiBanShiFundTrans.Description = string.Format("投保订单佣金收益，交强险:{0}元，商业险:{1}:元，合计:{2}元", yiBanShi_Commission_Compulsory.ToPrice(), yiBanShi_Commission_Commercial.ToPrice(), yiBanShi_Commission.ToPrice());
                YiBanShiFundTrans.Creator = operater;
                YiBanShiFundTrans.CreateTime = this.DateTime;
                CurrentDb.Transactions.Add(YiBanShiFundTrans);
                CurrentDb.SaveChanges();
                YiBanShiFundTrans.Sn = Sn.Build(SnType.Transactions, YiBanShiFundTrans.Id);
                CurrentDb.SaveChanges();


                var merchant_CommissionRate = commissionRate.Where(m => m.Type == Enumeration.CommissionRateType.InsuranceCompany).FirstOrDefault();


                var merchant_Commission_Commercial = orderToCarInsure.CommercialPrice * (merchant_CommissionRate.Commercial / 100);
                var merchant_Commission_Compulsory = orderToCarInsure.CompulsoryPrice * (merchant_CommissionRate.Compulsory / 100);
                var merchant_Commission = GetPayDecimal(merchant_Commission_Commercial + merchant_Commission_Compulsory);

                var merchantFund = CurrentDb.Fund.Where(m => m.UserId == orderToCarInsure.UserId).FirstOrDefault();


                merchantFund.Balance += merchant_Commission;
                merchantFund.Mender = operater;
                merchantFund.LastUpdateTime = this.DateTime;

                var merchantFundTrans = new Transactions();
                merchantFundTrans.UserId = merchantFund.UserId;
                merchantFundTrans.ChangeAmount = merchant_Commission;
                merchantFundTrans.Balance = merchantFund.Balance;
                merchantFundTrans.Type = Enumeration.TransactionsType.CarInsureCommission;
                merchantFundTrans.Description = string.Format("投保订单佣金收益，交强险:{0}元，商业险:{1}:元，合计:{2}元", merchant_Commission_Compulsory.ToPrice(), merchant_Commission_Commercial.ToPrice(), merchant_Commission.ToPrice());
                merchantFundTrans.Creator = operater;
                merchantFundTrans.CreateTime = this.DateTime;
                CurrentDb.Transactions.Add(merchantFundTrans);
                CurrentDb.SaveChanges();
                merchantFundTrans.Sn = Sn.Build(SnType.Transactions, merchantFundTrans.Id);
                CurrentDb.SaveChanges();

                orderToCarInsure.MerchantCommission = merchant_Commission;
                orderToCarInsure.YiBanShiCommission = yiBanShi_Commission;
                orderToCarInsure.CommissionVersion = "2017.02.28";
                orderToCarInsure.UplinkCommission = uplink_Commission;
                orderToCarInsure.Status = Enumeration.OrderStatus.Completed;
                orderToCarInsure.PayTime = this.DateTime;
                orderToCarInsure.CompleteTime = this.DateTime;
                orderToCarInsure.LastUpdateTime = this.DateTime;
                orderToCarInsure.Mender = operater;

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "该订单支付结果反馈成功");

            }

            return result;
        }

        private CustomJsonResult PayDepositRentCompleted(int operater, string orderSn)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var orderToDepositRent = CurrentDb.OrderToDepositRent.Where(m => m.Sn == orderSn).FirstOrDefault();


                if (orderToDepositRent.Status == Enumeration.OrderStatus.Completed)
                {
                    ts.Complete();
                    return new CustomJsonResult(ResultType.Success, ResultCode.Success, "该订单已经支付完成");
                }



                if (orderToDepositRent.Status != Enumeration.OrderStatus.WaitPay)
                {
                    ts.Complete();
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单未在就绪支付状态");
                }

                var UplinkFund = CurrentDb.Fund.Where(m => m.UserId == (int)Enumeration.UserAccount.Uplink).FirstOrDefault();
                UplinkFund.Balance += orderToDepositRent.Price;
                UplinkFund.Mender = operater;
                UplinkFund.LastUpdateTime = this.DateTime;

                var UplinkFundTrans = new Transactions();
                UplinkFundTrans.UserId = UplinkFund.UserId;
                UplinkFundTrans.ChangeAmount = orderToDepositRent.Price;
                UplinkFundTrans.Balance = UplinkFund.Balance;
                UplinkFundTrans.Type = Enumeration.TransactionsType.DepositRent;
                UplinkFundTrans.Description = string.Format("押金租金订单，押金:{0}元，租金:{1}:元，合计:{2}元", orderToDepositRent.Deposit.ToPrice(), orderToDepositRent.RentTotal.ToPrice(), orderToDepositRent.Price.ToPrice());
                UplinkFundTrans.Creator = operater;
                UplinkFundTrans.CreateTime = this.DateTime;
                CurrentDb.Transactions.Add(UplinkFundTrans);
                CurrentDb.SaveChanges();
                UplinkFundTrans.Sn = Sn.Build(SnType.Transactions, UplinkFundTrans.Id);
                CurrentDb.SaveChanges();


                var merchantPosMachine = CurrentDb.MerchantPosMachine.Where(m => m.MerchantId == orderToDepositRent.MerchantId && m.PosMachineId == orderToDepositRent.PosMachineId).FirstOrDefault();

                merchantPosMachine.DepositPayTime = this.DateTime;
                merchantPosMachine.Deposit = orderToDepositRent.Deposit;
                merchantPosMachine.RentDueDate = orderToDepositRent.RentDueDate;
                merchantPosMachine.Status = Enumeration.MerchantPosMachineStatus.Normal;
                merchantPosMachine.LastUpdateTime = this.DateTime;
                merchantPosMachine.Mender = operater;

                var merchantAudit = CurrentDb.BizProcessesAudit.Where(m => m.AduitReferenceId == orderToDepositRent.MerchantId && m.AduitType == Enumeration.BizProcessesAuditType.MerchantAudit).FirstOrDefault();
                if (merchantAudit == null)
                {
                    BizFactory.BizProcessesAudit.Add(operater, Enumeration.BizProcessesAuditType.MerchantAudit, merchantPosMachine.MerchantId, Enumeration.MerchantAuditStatus.WaitPrimaryAudit, "");
                }


                orderToDepositRent.Status = Enumeration.OrderStatus.Completed;
                orderToDepositRent.PayTime = this.DateTime;
                orderToDepositRent.CompleteTime = this.DateTime;
                orderToDepositRent.LastUpdateTime = this.DateTime;
                orderToDepositRent.Mender = operater;

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "该订单支付结果反馈成功");
            }

            return result;
        }

        private CustomJsonResult PayRentCompleted(int operater, string orderSn)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var orderToRent = CurrentDb.OrderToDepositRent.Where(m => m.Sn == orderSn).FirstOrDefault();


                if (orderToRent.Status == Enumeration.OrderStatus.Completed)
                {
                    ts.Complete();
                    return new CustomJsonResult(ResultType.Success, ResultCode.Success, "该订单已经支付完成");
                }


                if (orderToRent.Status != Enumeration.OrderStatus.WaitPay)
                {
                    ts.Complete();
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单未在就绪支付状态");
                }

                var UplinkFund = CurrentDb.Fund.Where(m => m.UserId == (int)Enumeration.UserAccount.Uplink).FirstOrDefault();
                UplinkFund.Balance += orderToRent.Price;
                UplinkFund.Mender = operater;
                UplinkFund.LastUpdateTime = this.DateTime;

                var UplinkFundTrans = new Transactions();
                UplinkFundTrans.UserId = UplinkFund.UserId;
                UplinkFundTrans.ChangeAmount = orderToRent.Price;
                UplinkFundTrans.Balance = UplinkFund.Balance;
                UplinkFundTrans.Type = Enumeration.TransactionsType.Rent;
                UplinkFundTrans.Description = string.Format("租金订单:{0}:元", orderToRent.Price.ToPrice());
                UplinkFundTrans.Creator = operater;
                UplinkFundTrans.CreateTime = this.DateTime;
                CurrentDb.Transactions.Add(UplinkFundTrans);
                CurrentDb.SaveChanges();
                UplinkFundTrans.Sn = Sn.Build(SnType.Transactions, UplinkFundTrans.Id);
                CurrentDb.SaveChanges();


                var merchantPosMachine = CurrentDb.MerchantPosMachine.Where(m => m.MerchantId == orderToRent.MerchantId && m.PosMachineId == orderToRent.PosMachineId).FirstOrDefault();
                merchantPosMachine.RentDueDate = orderToRent.RentDueDate;
                merchantPosMachine.Status = Enumeration.MerchantPosMachineStatus.Normal;
                merchantPosMachine.LastUpdateTime = this.DateTime;
                merchantPosMachine.Mender = operater;


                orderToRent.Status = Enumeration.OrderStatus.Completed;
                orderToRent.PayTime = this.DateTime;
                orderToRent.CompleteTime = this.DateTime;
                orderToRent.LastUpdateTime = this.DateTime;
                orderToRent.Mender = operater;


                BizFactory.Merchant.GetRentOrder(orderToRent.MerchantId, orderToRent.PosMachineId);

                CurrentDb.SaveChanges();

                ts.Complete();
                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "该订单支付结果反馈成功");
            }

            return result;
        }

        private CustomJsonResult PayCarClaimCompleted(int operater, string orderSn)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var orderToCarClaim = CurrentDb.OrderToCarClaim.Where(m => m.Sn == orderSn).FirstOrDefault();


                if (orderToCarClaim.Status == Enumeration.OrderStatus.Completed)
                {
                    ts.Complete();
                    return new CustomJsonResult(ResultType.Success, ResultCode.Success, "该订单已经支付完成");
                }


                if (orderToCarClaim.Status != Enumeration.OrderStatus.WaitPay)
                {
                    ts.Complete();
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单未在就绪支付状态");
                }


                CalculateCarClaimPayPrice calculateCarClaimPayPrice = new CalculateCarClaimPayPrice(orderToCarClaim.WorkingHoursPrice, orderToCarClaim.AccessoriesPrice);


                decimal uplink_Commission = calculateCarClaimPayPrice.UplinkCommission;
                decimal merchant_Commission = calculateCarClaimPayPrice.MerchantCommission;

                var merchant = CurrentDb.Merchant.Where(m => m.Id == orderToCarClaim.HandMerchantId).FirstOrDefault();



                var merchantFund = CurrentDb.Fund.Where(m => m.UserId == merchant.UserId).FirstOrDefault();


                merchantFund.Balance += merchant_Commission;
                merchantFund.Mender = operater;
                merchantFund.LastUpdateTime = this.DateTime;

                var merchantFundTrans = new Transactions();
                merchantFundTrans.UserId = merchantFund.UserId;
                merchantFundTrans.ChangeAmount = merchant_Commission;
                merchantFundTrans.Balance = merchantFund.Balance;
                merchantFundTrans.Type = Enumeration.TransactionsType.CarClaimCommission;
                merchantFundTrans.Description = string.Format("理赔订单佣金收益，理赔总金额:{0}元，应付金额:{1}:元，佣金:{2}元", orderToCarClaim.EstimatePrice.ToF2Price(), orderToCarClaim.Price.ToF2Price(), merchant_Commission.ToF2Price());
                merchantFundTrans.Creator = operater;
                merchantFundTrans.CreateTime = this.DateTime;
                CurrentDb.Transactions.Add(merchantFundTrans);
                CurrentDb.SaveChanges();
                merchantFundTrans.Sn = Sn.Build(SnType.Transactions, merchantFundTrans.Id);
                CurrentDb.SaveChanges();



                var uplinkFund = CurrentDb.Fund.Where(m => m.UserId == (int)Enumeration.UserAccount.Uplink).FirstOrDefault();

                uplinkFund.Balance += uplink_Commission;
                uplinkFund.Mender = operater;
                uplinkFund.LastUpdateTime = this.DateTime;

                var uplinkFundTrans = new Transactions();
                uplinkFundTrans.UserId = uplinkFund.UserId;
                uplinkFundTrans.ChangeAmount = uplink_Commission;
                uplinkFundTrans.Balance = uplinkFund.Balance;
                uplinkFundTrans.Type = Enumeration.TransactionsType.CarClaimCommission;
                uplinkFundTrans.Description = string.Format("理赔订单佣金收益，理赔总金额:{0}元，应付金额:{1}:元，佣金:{2}元", orderToCarClaim.EstimatePrice.ToF2Price(), orderToCarClaim.Price.ToF2Price(), uplink_Commission.ToF2Price());
                uplinkFundTrans.Creator = operater;
                uplinkFundTrans.CreateTime = this.DateTime;
                CurrentDb.Transactions.Add(uplinkFundTrans);
                CurrentDb.SaveChanges();
                uplinkFundTrans.Sn = Sn.Build(SnType.Transactions, uplinkFundTrans.Id);
                CurrentDb.SaveChanges();


                orderToCarClaim.Status = Enumeration.OrderStatus.Completed;
                orderToCarClaim.PayTime = this.DateTime;
                orderToCarClaim.CompleteTime = this.DateTime;
                orderToCarClaim.LastUpdateTime = this.DateTime;
                orderToCarClaim.Mender = operater;
                orderToCarClaim.CommissionVersion = calculateCarClaimPayPrice.Version;

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "该订单支付结果反馈成功");
            }

            return result;
        }
    }
}
