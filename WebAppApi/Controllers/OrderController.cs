﻿using Lumos.BLL;
using Lumos.Entity;
using Lumos.Mvc;
using Lumos.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAppApi.Models.Order;
using Lumos.Entity.AppApi;

namespace WebAppApi.Controllers
{
    public class OrderController : BaseApiController
    {
        [HttpGet]
        public APIResponse GetPayableList(int userId, int merchantId, int pageIndex)
        {
            var order = (from o in CurrentDb.Order
                         where o.MerchantId == merchantId

                         select new { o.Id, o.Sn, o.ProductType, o.Price, o.Status, o.Remarks, o.SubmitTime, o.CompleteTime, o.CancleTime, o.FollowStatus }
                         );


            order = order.Where(m => m.Status == Enumeration.OrderStatus.WaitPay && m.ProductType == Enumeration.ProductType.InsureForCarForClaim);


            int pageSize = 10;

            order = order.OrderByDescending(r => r.SubmitTime).Skip(pageSize * (pageIndex)).Take(pageSize);



            var orderlist = order.ToList();
            List<OrderModel> model = new List<OrderModel>();
            foreach (var m in orderlist)
            {
                OrderModel orderModel = new OrderModel();
                orderModel.Id = m.Id;
                orderModel.Sn = m.Sn;
                orderModel.Product = m.ProductType.GetCnName();
                orderModel.ProductType = m.ProductType;
                orderModel.Status = m.Status;
                orderModel.Price = m.Price;
                orderModel.Remarks = string.Format("应付金额:{0}", m.Price.ToF2Price());//理赔 理赔金额

                var orderToCarClaim = CurrentDb.OrderToCarClaim.Where(c => c.Id == m.Id).FirstOrDefault();
                orderModel.OrderField.Add(new OrderField("保险公司", orderToCarClaim.InsuranceCompanyName));
                orderModel.OrderField.Add(new OrderField("车牌号码", orderToCarClaim.CarPlateNo));
                orderModel.OrderField.Add(new OrderField("工时费", orderToCarClaim.WorkingHoursPrice.ToF2Price()));
                orderModel.OrderField.Add(new OrderField("配件费", orderToCarClaim.AccessoriesPrice.ToF2Price()));

                orderModel.StatusName = "待支付";

                model.Add(orderModel);
            }

            APIResult result = new APIResult() { Result = ResultType.Success, Code = ResultCode.Success, Message = "", Data = model };

            return new APIResponse(result);
        }



        [HttpGet]
        public APIResponse GetList(int userId, int merchantId, int pageIndex, Enumeration.OrderStatus status)
        {
            var order = (from o in CurrentDb.Order
                         where o.MerchantId == merchantId

                         select new { o.Id, o.Sn, o.ProductType, o.Price, o.Status, o.Remarks, o.SubmitTime, o.CompleteTime, o.CancleTime, o.FollowStatus }
                         );

            if (status != Enumeration.OrderStatus.Unknow)
            {
                order = order.Where(m => m.Status == status && (((int)m.ProductType).ToString().StartsWith("201")));
            }

            int pageSize = 10;

            order = order.OrderByDescending(r => r.SubmitTime).Skip(pageSize * (pageIndex)).Take(pageSize);



            var orderlist = order.ToList();
            List<OrderModel> model = new List<OrderModel>();
            foreach (var m in orderlist)
            {
                OrderModel orderModel = new OrderModel();
                orderModel.Id = m.Id;
                orderModel.Sn = m.Sn;
                orderModel.Product = m.ProductType.GetCnName();
                orderModel.ProductType = m.ProductType;
                orderModel.Status = m.Status;
                orderModel.Price = m.Price;
                switch (m.Status)
                {
                    case Enumeration.OrderStatus.Submitted:
                        #region 已提交
                        orderModel.Remarks = m.SubmitTime.ToUnifiedFormatDateTime();//备注提交时间
                        orderModel.StatusName = "已提交";

                        switch (m.ProductType)
                        {
                            case Enumeration.ProductType.InsureForCarForInsure:
                            case Enumeration.ProductType.InsureForCarForRenewal:

                                var orderToCarInsure = CurrentDb.OrderToCarInsure.Where(c => c.Id == m.Id).FirstOrDefault();
                                orderModel.OrderField.Add(new OrderField("车主姓名", orderToCarInsure.CarOwner.NullToEmpty()));
                                orderModel.OrderField.Add(new OrderField("身份证号码", orderToCarInsure.CarOwnerIdNumber.NullToEmpty()));
                                orderModel.OrderField.Add(new OrderField("车牌号码", orderToCarInsure.CarPlateNo.NullToEmpty()));
                                orderModel.OrderField.Add(new OrderField("状态", "请稍侯，报价中"));

                                break;
                            case Enumeration.ProductType.InsureForCarForClaim:

                                var orderToCarClaim = CurrentDb.OrderToCarClaim.Where(c => c.Id == m.Id).FirstOrDefault();
                                orderModel.OrderField.Add(new OrderField("保险公司", orderToCarClaim.InsuranceCompanyName.NullToEmpty()));
                                orderModel.OrderField.Add(new OrderField("车牌号码", orderToCarClaim.CarPlateNo.NullToEmpty()));
                                orderModel.OrderField.Add(new OrderField("对接人", string.Format("{0},{1}", orderToCarClaim.HandPerson.NullToEmpty(), orderToCarClaim.HandPersonPhone.NullToEmpty())));
                                orderModel.OrderField.Add(new OrderField("状态", "请稍侯，理赔呼叫中"));
                                break;
                        }
                        #endregion
                        break;
                    case Enumeration.OrderStatus.Follow:
                        orderModel.Remarks = m.Remarks;//备注 订单备注
                        orderModel.StatusName = "跟进中";
                        orderModel.FollowStatus = m.FollowStatus;
                        switch (m.ProductType)
                        {
                            case Enumeration.ProductType.InsureForCarForInsure:

                                var orderToCarInsure = CurrentDb.OrderToCarInsure.Where(c => c.Id == m.Id).FirstOrDefault();
                                orderModel.OrderField.Add(new OrderField("车主姓名", orderToCarInsure.CarOwner.NullToEmpty()));
                                orderModel.OrderField.Add(new OrderField("车牌号码", orderToCarInsure.CarPlateNo.NullToEmpty()));

                                var orderToCarInsureOfferCompany = CurrentDb.OrderToCarInsureOfferCompany.Where(c => c.OrderId == m.Id).ToList();
                                foreach (var c in orderToCarInsureOfferCompany)
                                {
                                    orderModel.OrderField.Add(new OrderField(c.InsuranceCompanyName, string.Format("{0}元", c.InsureTotalPrice.ToF2Price())));
                                }

                                break;
                            case Enumeration.ProductType.InsureForCarForRenewal:

                                var orderToCarRenewal = CurrentDb.OrderToCarInsure.Where(c => c.Id == m.Id).FirstOrDefault();



                                orderModel.OrderField.Add(new OrderField("车主姓名", orderToCarRenewal.CarOwner.NullToEmpty()));
                                orderModel.OrderField.Add(new OrderField("车牌号码", orderToCarRenewal.CarPlateNo.NullToEmpty()));

                                var orderToCarRenewalOfferCompany = CurrentDb.OrderToCarInsureOfferCompany.Where(c => c.OrderId == m.Id).ToList();
                                foreach (var c in orderToCarRenewalOfferCompany)
                                {
                                    orderModel.OrderField.Add(new OrderField(c.InsuranceCompanyName, string.Format("{0}元", c.InsureTotalPrice.ToF2Price())));
                                }

                                break;
                            case Enumeration.ProductType.InsureForCarForClaim:

                                var orderToCarClaim = CurrentDb.OrderToCarClaim.Where(c => c.Id == m.Id).FirstOrDefault();
                                orderModel.OrderField.Add(new OrderField("保险公司", orderToCarClaim.InsuranceCompanyName.NullToEmpty()));
                                orderModel.OrderField.Add(new OrderField("车牌号码", orderToCarClaim.CarPlateNo.NullToEmpty()));

                                var merchant = CurrentDb.Merchant.Where(q => q.Id == orderToCarClaim.HandMerchantId).FirstOrDefault();
                                if (orderToCarClaim.HandMerchantType == Enumeration.MerchantType.CarRepair)
                                {
                                    orderModel.OrderField.Add(new OrderField("对接维修厂", merchant.YYZZ_Name.NullToEmpty()));
                                }
                                else 
                                {
                                    orderModel.OrderField.Add(new OrderField("对接商户", merchant.YYZZ_Name.NullToEmpty()));
                                }

                                orderModel.OrderField.Add(new OrderField("进度", ((Enumeration.OrderToCarClaimFollowStatus)orderToCarClaim.FollowStatus).GetCnName()));
                                break;
                        }


                        break;
                    case Enumeration.OrderStatus.WaitPay:
                        switch (m.ProductType)
                        {
                            case Enumeration.ProductType.InsureForCarForInsure:
                                orderModel.Remarks = "";

                                var orderToCarInsure = CurrentDb.OrderToCarInsure.Where(c => c.Id == m.Id).FirstOrDefault();
                                orderModel.OrderField.Add(new OrderField("车主姓名", orderToCarInsure.CarOwner.NullToEmpty()));
                                orderModel.OrderField.Add(new OrderField("车牌号码", orderToCarInsure.CarPlateNo.NullToEmpty()));

                                var orderToCarInsureOfferCompany = CurrentDb.OrderToCarInsureOfferCompany.Where(c => c.OrderId == m.Id).ToList();
                                foreach (var c in orderToCarInsureOfferCompany)
                                {
                                    orderModel.OrderField.Add(new OrderField(c.InsuranceCompanyName, string.Format("{0}元", c.InsureTotalPrice.ToF2Price())));
                                }

                                break;
                            case Enumeration.ProductType.InsureForCarForRenewal:
                                orderModel.Remarks = string.Format("合计:{0}", m.Price.ToPrice());//续保 保单金额

                                var orderToCarRenewal = CurrentDb.OrderToCarInsure.Where(c => c.Id == m.Id).FirstOrDefault();
                                orderModel.OrderField.Add(new OrderField("车主姓名", orderToCarRenewal.CarOwner.NullToEmpty()));
                                orderModel.OrderField.Add(new OrderField("车牌号码", orderToCarRenewal.CarPlateNo.NullToEmpty()));

                                var orderToCarRenewalOfferCompany = CurrentDb.OrderToCarInsureOfferCompany.Where(c => c.OrderId == m.Id).ToList();
                                foreach (var c in orderToCarRenewalOfferCompany)
                                {
                                    orderModel.OrderField.Add(new OrderField(c.InsuranceCompanyName, string.Format("{0}元", c.InsureTotalPrice.ToF2Price())));
                                }

                                break;
                            case Enumeration.ProductType.InsureForCarForClaim:
                                orderModel.Remarks = string.Format("应付金额:{0}元", m.Price.ToF2Price());//理赔 理赔金额

                                var orderToCarClaim = CurrentDb.OrderToCarClaim.Where(c => c.Id == m.Id).FirstOrDefault();
                                orderModel.OrderField.Add(new OrderField("保险公司", orderToCarClaim.InsuranceCompanyName.NullToEmpty()));
                                orderModel.OrderField.Add(new OrderField("车牌号码", orderToCarClaim.CarPlateNo.NullToEmpty()));
                                orderModel.OrderField.Add(new OrderField("对接人", string.Format("{0},{1}", orderToCarClaim.HandPerson.NullToEmpty(), orderToCarClaim.HandPersonPhone.NullToEmpty())));
                                orderModel.OrderField.Add(new OrderField("定损单总价", string.Format("{0}元", orderToCarClaim.EstimatePrice.ToF2Price())));

                                break;
                        }
                        orderModel.StatusName = "待支付";
                        break;
                    case Enumeration.OrderStatus.Completed:
                        orderModel.Remarks = m.CompleteTime.ToUnifiedFormatDateTime();//备注完成时间
                        orderModel.StatusName = "已完成";

                        switch (m.ProductType)
                        {
                            case Enumeration.ProductType.InsureForCarForInsure:


                                var orderToCarInsure = CurrentDb.OrderToCarInsure.Where(c => c.Id == m.Id).FirstOrDefault();
                                orderModel.OrderField.Add(new OrderField("车主姓名", orderToCarInsure.CarOwner.NullToEmpty()));
                                orderModel.OrderField.Add(new OrderField("身份证号码", orderToCarInsure.CarOwnerIdNumber.NullToEmpty()));
                                orderModel.OrderField.Add(new OrderField("车牌号码", orderToCarInsure.CarPlateNo.NullToEmpty()));
                                orderModel.OrderField.Add(new OrderField(orderToCarInsure.InsuranceCompanyName, string.Format("{0}元", orderToCarInsure.Price.ToF2Price())));


                                break;
                            case Enumeration.ProductType.InsureForCarForRenewal:
                                orderModel.Remarks = string.Format("合计:{0}", m.Price);//续保 保单金额

                                var orderToCarRenewal = CurrentDb.OrderToCarInsure.Where(c => c.Id == m.Id).FirstOrDefault();
                                orderModel.OrderField.Add(new OrderField("车主姓名", orderToCarRenewal.CarOwner.NullToEmpty()));
                                orderModel.OrderField.Add(new OrderField("身份证号码", orderToCarRenewal.CarOwnerIdNumber.NullToEmpty()));
                                orderModel.OrderField.Add(new OrderField("车牌号码", orderToCarRenewal.CarPlateNo.NullToEmpty()));
                                orderModel.OrderField.Add(new OrderField(orderToCarRenewal.InsuranceCompanyName, string.Format("{0}元", orderToCarRenewal.Price.ToF2Price())));


                                break;
                            case Enumeration.ProductType.InsureForCarForClaim:
                                orderModel.Remarks = string.Format("合计:{0}", m.Price);//理赔 理赔金额

                                var orderToCarClaim = CurrentDb.OrderToCarClaim.Where(c => c.Id == m.Id).FirstOrDefault();

                                orderModel.OrderField.Add(new OrderField("车牌号码", orderToCarClaim.CarPlateNo.NullToEmpty()));
                                orderModel.OrderField.Add(new OrderField("工时费", orderToCarClaim.WorkingHoursPrice.ToF2Price()));
                                orderModel.OrderField.Add(new OrderField("配件费", orderToCarClaim.AccessoriesPrice.ToF2Price()));
                                orderModel.OrderField.Add(new OrderField("合计", orderToCarClaim.Price.ToF2Price()));

                                break;
                        }

                        break;
                    case Enumeration.OrderStatus.Cancled:
                        orderModel.Remarks = m.CancleTime.ToUnifiedFormatDateTime();//备注完成时间
                        orderModel.StatusName = "已取消";

                        orderModel.OrderField.Add(new OrderField(m.Remarks, ""));

                        break;
                }


                model.Add(orderModel);
            }



            APIResult result = new APIResult() { Result = ResultType.Success, Code = ResultCode.Success, Message = "", Data = model };

            return new APIResponse(result);
        }

        [HttpPost]
        public APIResponse Pay(PayModel model)
        {
            IResult result = BizFactory.Order.Pay(model.UserId, model.OrderSn);
            return new APIResponse(result);
        }

        [HttpGet]
        public APIResponse GetDetails(int userId, int merchantId, int orderId, Enumeration.ProductType productType)
        {
            if (productType == Enumeration.ProductType.InsureForCarForInsure || productType == Enumeration.ProductType.InsureForCarForRenewal)
            {
                OrderCarInsureDetailsModel model = new OrderCarInsureDetailsModel();
                var orderToCarInsure = CurrentDb.OrderToCarInsure.Where(m => m.Id == orderId).FirstOrDefault();
                if (orderToCarInsure != null)
                {
                    model.Id = orderToCarInsure.Id;
                    model.Sn = orderToCarInsure.Sn;

                    #region 报价公司
                    var orderToCarInsureOfferCompany = CurrentDb.OrderToCarInsureOfferCompany.Where(m => m.OrderId == orderToCarInsure.Id).ToList();
                    foreach (var m in orderToCarInsureOfferCompany)
                    {
                        OrderCarInsureOfferCompanyModel orderCarInsureOfferCompanyModel = new OrderCarInsureOfferCompanyModel();
                        orderCarInsureOfferCompanyModel.InsuranceOfferId = m.Id;
                        orderCarInsureOfferCompanyModel.InsuranceCompanyId = m.InsuranceCompanyId;
                        orderCarInsureOfferCompanyModel.InsuranceCompanyName = m.InsuranceCompanyName;
                        orderCarInsureOfferCompanyModel.InsureImgUrl = m.InsureImgUrl;

                        if (m.CommercialPrice != null && m.TravelTaxPrice != null)
                        {
                            orderCarInsureOfferCompanyModel.CommercialAndTravelTaxPrice = m.CompulsoryPrice.Value + m.TravelTaxPrice.Value;
                        }

                        if (m.CompulsoryPrice != null)
                        {
                            orderCarInsureOfferCompanyModel.CompulsoryPrice = m.CommercialPrice.Value;
                        }

                        if (m.InsureTotalPrice != null)
                        {
                            orderCarInsureOfferCompanyModel.InsureTotalPrice = m.InsureTotalPrice.Value;
                        }

                        model.OfferCompany.Add(orderCarInsureOfferCompanyModel);
                    }
                    #endregion

                    #region 险种
                    var orderToCarInsureOfferKind = CurrentDb.OrderToCarInsureOfferKind.Where(m => m.OrderId == orderToCarInsure.Id).ToList();
                    var carKinds = CurrentDb.CarKind.ToList();
                    if (orderToCarInsureOfferKind != null)
                    {
                        foreach (var m in orderToCarInsureOfferKind)
                        {
                            var carKind = carKinds.Where(q => q.Id == m.KindId).FirstOrDefault();
                            if (carKind != null)
                            {
                                OrderToCarInsureOfferKindModel orderToCarInsureOfferKindModel = new OrderToCarInsureOfferKindModel();
                                orderToCarInsureOfferKindModel.Field = carKind.Name;
                                orderToCarInsureOfferKindModel.Value = m.KindValue + carKind.InputUnit;
                                model.OfferKind.Add(orderToCarInsureOfferKindModel);
                            }
                        }
                    }
                    #endregion


                    #region 车主
                    model.CarOwner = orderToCarInsure.CarOwner.NullToEmpty();
                    model.CarPlateNo = orderToCarInsure.CarPlateNo.NullToEmpty();
                    model.CarOwnerIdNumber = orderToCarInsure.CarOwnerIdNumber.NullToEmpty();
                    #endregion

                    model.InsuranceCompanyId = orderToCarInsure.InsuranceCompanyId;
                    model.InsuranceCompanyName = orderToCarInsure.InsuranceCompanyName;
                    model.InsureImgUrl = orderToCarInsure.InsureImgUrl;

                    model.ShippingAddress = orderToCarInsure.ShippingAddress.NullToEmpty();

                    model.CommercialAndTravelTaxPrice = orderToCarInsure.CommercialPrice + orderToCarInsure.TravelTaxPrice;
                    model.CompulsoryPrice = orderToCarInsure.CompulsoryPrice;
                    model.Price = orderToCarInsure.Price;

                    #region 证件

                    if (orderToCarInsure.CZ_CL_XSZ_ImgUrl != null)
                    {
                        model.ZJ.Add(new ZjModel("行驶证", orderToCarInsure.CZ_CL_XSZ_ImgUrl));
                    }

                    if (orderToCarInsure.CZ_SFZ_ImgUrl != null)
                    {
                        model.ZJ.Add(new ZjModel("身份证", orderToCarInsure.CZ_SFZ_ImgUrl));
                    }

                    if (orderToCarInsure.CCSJM_WSZM_ImgUrl != null)
                    {
                        model.ZJ.Add(new ZjModel("车船税减免/完税证明", orderToCarInsure.CCSJM_WSZM_ImgUrl));
                    }

                    if (orderToCarInsure.YCZ_CLDJZ_ImgUrl != null)
                    {
                        model.ZJ.Add(new ZjModel("验车照/车辆登记证", orderToCarInsure.YCZ_CLDJZ_ImgUrl));
                    }

                    if (orderToCarInsure.ZJ1_ImgUrl != null)
                    {
                        model.ZJ.Add(new ZjModel("其它", orderToCarInsure.ZJ1_ImgUrl));
                    }

                    if (orderToCarInsure.ZJ2_ImgUrl != null)
                    {
                        model.ZJ.Add(new ZjModel("其它", orderToCarInsure.ZJ2_ImgUrl));
                    }

                    if (orderToCarInsure.ZJ3_ImgUrl != null)
                    {
                        model.ZJ.Add(new ZjModel("其它", orderToCarInsure.ZJ3_ImgUrl));
                    }

                    if (orderToCarInsure.ZJ4_ImgUrl != null)
                    {
                        model.ZJ.Add(new ZjModel("其它", orderToCarInsure.ZJ4_ImgUrl));
                    }
                    #endregion

                    model.SubmitTime = orderToCarInsure.SubmitTime;
                    model.CompleteTime = orderToCarInsure.CompleteTime;
                    model.PayTime = orderToCarInsure.PayTime;
                    model.CancleTime = orderToCarInsure.CancleTime;
                    model.Status = orderToCarInsure.Status;
                    model.StatusName = orderToCarInsure.Status.GetCnName();
                    model.FollowStatus = orderToCarInsure.FollowStatus;
                    model.Remarks = orderToCarInsure.Remarks.NullToEmpty();

                    model.ShippingAddressList.Add("地址1");
                    model.ShippingAddressList.Add("地址2");

                }

                APIResult result = new APIResult() { Result = ResultType.Success, Code = ResultCode.Success, Message = "获取成功", Data = model };
                return new APIResponse(result);
            }
            else if (productType == Enumeration.ProductType.InsureForCarForClaim)
            {
                OrderCarClaimDetailsModel model = new OrderCarClaimDetailsModel();
                var orderToCarEstimate = CurrentDb.OrderToCarClaim.Where(m => m.Id == orderId).FirstOrDefault();
                if (orderToCarEstimate != null)
                {
                    model.Id = orderToCarEstimate.Id;
                    model.Sn = orderToCarEstimate.Sn;
                    model.CarOwner = orderToCarEstimate.CarOwner;
                    model.CarPlateNo = orderToCarEstimate.CarPlateNo;
                    model.RepairsType = orderToCarEstimate.RepairsType.GetCnName();
                    model.HandPerson = orderToCarEstimate.HandPerson;
                    model.HandPersonPhone = orderToCarEstimate.HandPersonPhone;
                    model.InsuranceCompanyName = orderToCarEstimate.InsuranceCompanyName;
                    model.InsuranceCompanyId = orderToCarEstimate.InsuranceCompanyId;
                    model.EstimateListImgUrl = orderToCarEstimate.EstimateListImgUrl;
                    model.SubmitTime = orderToCarEstimate.SubmitTime;
                    model.CompleteTime = orderToCarEstimate.CompleteTime;
                    model.PayTime = orderToCarEstimate.PayTime;
                    model.CancleTime = orderToCarEstimate.CancleTime;
                    model.AccessoriesPrice = orderToCarEstimate.AccessoriesPrice;
                    model.WorkingHoursPrice = orderToCarEstimate.WorkingHoursPrice;
                    model.EstimatePrice = orderToCarEstimate.EstimatePrice;
                    model.Price = orderToCarEstimate.Price;
                    model.Status = orderToCarEstimate.Status;
                    model.FollowStatus = orderToCarEstimate.FollowStatus;
                    model.StatusName = orderToCarEstimate.Status.GetCnName();
                    model.Remark = orderToCarEstimate.Remarks;

                    if (orderToCarEstimate.HandMerchantId != null)
                    {
                        var handMerchant = CurrentDb.Merchant.Where(m => m.Id == orderToCarEstimate.HandMerchantId.Value).FirstOrDefault();
                        if (handMerchant != null)
                        {
                            MerchantModel merchantModel = new MerchantModel();
                            merchantModel.Id = handMerchant.Id;
                            merchantModel.Name = handMerchant.YYZZ_Name;
                            merchantModel.Contact = handMerchant.ContactName;
                            merchantModel.ContactAddress = handMerchant.YYZZ_Address;
                            merchantModel.ContactPhone = handMerchant.ContactPhoneNumber;
                            model.EstimateMerchant = merchantModel;

                        }

                    }
                }

                APIResult result = new APIResult() { Result = ResultType.Success, Code = ResultCode.Success, Message = "获取成功", Data = model };
                return new APIResponse(result);
            }
            else
            {
                APIResult result = new APIResult() { Result = ResultType.Failure, Code = ResultCode.Failure, Message = "未知产品类型" };
                return new APIResponse(result);
            }
        }

        [HttpPost]
        public APIResponse Cancle(CancleModel model)
        {

            IResult result = BizFactory.Order.Cancle(model.UserId, model.OrderSn);
            return new APIResponse(result);
        }


        [HttpPost]
        public APIResponse PayConfirm(PayConfirmModel model)
        {
            IResult result = BizFactory.Pay.Confirm(model.UserId, model);

            return new APIResponse(result);
        }

        [HttpPost]
        public APIResponse PayResultNotify(PayResultModel model)
        {
            IResult result = BizFactory.Pay.ResultNotify(model.UserId, ResultNotifyParty.App, model);

            return new APIResponse(result);
        }


    }
}