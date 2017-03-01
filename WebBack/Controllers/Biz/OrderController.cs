using Lumos.BLL;
using Lumos.Entity;
using Lumos.Entity.AppApi;
using Lumos.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBack.Models.Biz.Order;

namespace WebBack.Controllers.Biz
{
    public class OrderController : WebBackController
    {
        public ViewResult CarInsureFollow(int id)
        {
            CarInsureFollowViewModel model = new CarInsureFollowViewModel(id);
            return View(model);
        }

        public ViewResult CarClaimFollow(int id)
        {
            CarClaimFollowViewModel model = new CarClaimFollowViewModel(id);
            return View(model);
        }

        public ViewResult CarInsurePay(int id)
        {
            CarInsurePayViewModel model = new CarInsurePayViewModel(id);
            return View(model);
        }

        public ViewResult List(Enumeration.OrderStatus status)
        {
            ListViewModel model = new ListViewModel();
            model.Status = status;
            return View(model);
        }


        public JsonResult GetList(OrderSearchCondition condition)
        {

            Enumeration.OrderStatus status = condition.Status;

            var query = (from o in CurrentDb.Order
                         join m in CurrentDb.Merchant on o.MerchantId equals m.Id
                         where o.PId == null
                         select new { o.Id, m.ClientCode, m.YYZZ_Name, o.Sn, o.ProductType, o.ProductName, o.Price, o.Status, o.Remarks, o.SubmitTime, o.CompleteTime, o.CancleTime, o.FollowStatus, o.ContactPhoneNumber, o.Contact }
                        );


            if (status != Enumeration.OrderStatus.Unknow)
            {
                query = query.Where(m => m.Status == status && (((int)m.ProductType).ToString().StartsWith("201")));
            }

            int pageIndex = condition.PageIndex;
            int pageSize = 10;

            query = query.OrderByDescending(r => r.SubmitTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            int total = query.Count();

            List<object> list = new List<object>();

            foreach (var item in query)
            {
                list.Add(new
                {
                    item.Id,
                    item.ClientCode,
                    item.Sn,
                    item.YYZZ_Name,
                    item.ContactPhoneNumber,
                    item.ProductName,
                    item.ProductType,
                    item.SubmitTime,
                    Status = item.Status.GetCnName()
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list };

            return Json(ResultType.Success, pageEntity, "");

        }

        //public JsonResult GetList(OrderSearchCondition condition)
        //{

        //    Enumeration.OrderStatus status = condition.Status;

        //    var query = (from o in CurrentDb.Order
        //                 join m in CurrentDb.Merchant on o.MerchantId equals m.Id
        //                 where o.PId == null
        //                 select new { o.Id, m.ClientCode, m.YYZZ_Name, o.Sn, o.ProductType, o.Price, o.Status, o.Remarks, o.SubmitTime, o.CompleteTime, o.CancleTime, o.FollowStatus }
        //                );


        //    if (status != Enumeration.OrderStatus.Unknow)
        //    {
        //        query = query.Where(m => m.Status == status && (((int)m.ProductType).ToString().StartsWith("201")));
        //    }

        //    int pageIndex = condition.PageIndex;
        //    int pageSize = 10;

        //    query = query.OrderByDescending(r => r.SubmitTime).Skip(pageSize * (pageIndex)).Take(pageSize);

        //    int total = query.Count();


        //    var querylist = query.ToList();
        //    List<OrderModel> model = new List<OrderModel>();
        //    foreach (var m in querylist)
        //    {
        //        OrderModel orderModel = new OrderModel();
        //        orderModel.Id = m.Id;
        //        orderModel.Sn = m.Sn;
        //        orderModel.Product = m.ProductType.GetCnName();
        //        orderModel.ProductType = m.ProductType;
        //        orderModel.Status = m.Status;
        //        switch (m.Status)
        //        {
        //            case Enumeration.OrderStatus.Submitted:
        //                #region 已提交
        //                orderModel.Remarks = m.SubmitTime.ToUnifiedFormatDateTime();//备注提交时间
        //                orderModel.StatusName = "已提交";

        //                switch (m.ProductType)
        //                {
        //                    case Enumeration.ProductType.InsureForCarForInsure:
        //                        orderModel.OrderField.Add(new OrderField("等待保险公司报价中", ""));
        //                        break;
        //                    case Enumeration.ProductType.InsureForCarForRenewal:
        //                        orderModel.OrderField.Add(new OrderField("等待保险公司报价中", ""));
        //                        break;
        //                    case Enumeration.ProductType.InsureForCarForClaim:
        //                        orderModel.OrderField.Add(new OrderField("等待理赔会叫", ""));
        //                        break;
        //                }
        //                #endregion
        //                break;
        //            case Enumeration.OrderStatus.Follow:
        //                orderModel.Remarks = m.Remarks;//备注 订单备注
        //                orderModel.StatusName = "跟进中";
        //                orderModel.FollowStatus = m.FollowStatus;
        //                switch (m.ProductType)
        //                {
        //                    case Enumeration.ProductType.InsureForCarForInsure:

        //                        var orderToCarInsure = CurrentDb.OrderToCarInsure.Where(c => c.Id == m.Id).FirstOrDefault();
        //                        orderModel.OrderField.Add(new OrderField("被保人", orderToCarInsure.CarOwner));
        //                        orderModel.OrderField.Add(new OrderField("车牌号码", orderToCarInsure.CarPlateNo));

        //                        var orderToCarInsureOfferCompany = CurrentDb.OrderToCarInsureOfferCompany.Where(c => c.OrderId == m.Id).ToList();
        //                        foreach (var c in orderToCarInsureOfferCompany)
        //                        {
        //                            orderModel.OrderField.Add(new OrderField(c.InsuranceCompanyName, c.InsureTotalPrice.ToPrice()));
        //                        }

        //                        break;
        //                    case Enumeration.ProductType.InsureForCarForRenewal:

        //                        var orderToCarRenewal = CurrentDb.OrderToCarInsure.Where(c => c.Id == m.Id).FirstOrDefault();



        //                        orderModel.OrderField.Add(new OrderField("被保人", orderToCarRenewal.CarOwner));
        //                        orderModel.OrderField.Add(new OrderField("车牌号码", orderToCarRenewal.CarPlateNo));

        //                        var orderToCarRenewalOfferCompany = CurrentDb.OrderToCarInsureOfferCompany.Where(c => c.OrderId == m.Id).ToList();
        //                        foreach (var c in orderToCarRenewalOfferCompany)
        //                        {
        //                            orderModel.OrderField.Add(new OrderField(c.InsuranceCompanyName, c.InsureTotalPrice.ToPrice()));
        //                        }

        //                        break;
        //                    case Enumeration.ProductType.InsureForCarForClaim:

        //                        var orderToCarClaim = CurrentDb.OrderToCarClaim.Where(c => c.Id == m.Id).FirstOrDefault();
        //                        orderModel.OrderField.Add(new OrderField("保险公司", orderToCarClaim.InsuranceCompanyName));
        //                        orderModel.OrderField.Add(new OrderField("车牌号码", orderToCarClaim.CarPlateNo));
        //                        orderModel.OrderField.Add(new OrderField("进度", ((Enumeration.OrderToCarClaimFollowStatus)orderToCarClaim.FollowStatus).GetCnName()));
        //                        break;
        //                }


        //                break;
        //            case Enumeration.OrderStatus.WaitPay:
        //                switch (m.ProductType)
        //                {
        //                    case Enumeration.ProductType.InsureForCarForInsure:
        //                        orderModel.Remarks = "";

        //                        var orderToCarInsure = CurrentDb.OrderToCarInsure.Where(c => c.Id == m.Id).FirstOrDefault();
        //                        orderModel.OrderField.Add(new OrderField("被保人", orderToCarInsure.CarOwner));
        //                        orderModel.OrderField.Add(new OrderField("车牌号码", orderToCarInsure.CarPlateNo));

        //                        var orderToCarInsureOfferCompany = CurrentDb.OrderToCarInsureOfferCompany.Where(c => c.OrderId == m.Id).ToList();
        //                        foreach (var c in orderToCarInsureOfferCompany)
        //                        {
        //                            orderModel.OrderField.Add(new OrderField(c.InsuranceCompanyName, c.InsureTotalPrice.ToPrice()));
        //                        }

        //                        break;
        //                    case Enumeration.ProductType.InsureForCarForRenewal:
        //                        orderModel.Remarks = string.Format("合计:{0}", m.Price);//续保 保单金额

        //                        var orderToCarRenewal = CurrentDb.OrderToCarInsure.Where(c => c.Id == m.Id).FirstOrDefault();
        //                        orderModel.OrderField.Add(new OrderField("被保人", orderToCarRenewal.CarOwner));
        //                        orderModel.OrderField.Add(new OrderField("车牌号码", orderToCarRenewal.CarPlateNo));

        //                        var orderToCarRenewalOfferCompany = CurrentDb.OrderToCarInsureOfferCompany.Where(c => c.OrderId == m.Id).ToList();
        //                        foreach (var c in orderToCarRenewalOfferCompany)
        //                        {
        //                            orderModel.OrderField.Add(new OrderField(c.InsuranceCompanyName, c.InsureTotalPrice.ToPrice()));
        //                        }

        //                        break;
        //                    case Enumeration.ProductType.InsureForCarForClaim:
        //                        orderModel.Remarks = string.Format("应付金额:{0}", m.Price.ToF2Price());//理赔 理赔金额

        //                        var orderToCarClaim = CurrentDb.OrderToCarClaim.Where(c => c.Id == m.Id).FirstOrDefault();

        //                        orderModel.OrderField.Add(new OrderField("保险公司", orderToCarClaim.InsuranceCompanyName));
        //                        orderModel.OrderField.Add(new OrderField("车牌号码", orderToCarClaim.CarPlateNo));
        //                        orderModel.OrderField.Add(new OrderField("工时费", orderToCarClaim.WorkingHoursPrice.ToF2Price()));
        //                        orderModel.OrderField.Add(new OrderField("配件费", orderToCarClaim.AccessoriesPrice.ToF2Price()));

        //                        break;
        //                }
        //                orderModel.StatusName = "待支付";
        //                break;
        //            case Enumeration.OrderStatus.Completed:
        //                orderModel.Remarks = m.CompleteTime.ToUnifiedFormatDateTime();//备注完成时间
        //                orderModel.StatusName = "已完成";

        //                switch (m.ProductType)
        //                {
        //                    case Enumeration.ProductType.InsureForCarForInsure:


        //                        var orderToCarInsure = CurrentDb.OrderToCarInsure.Where(c => c.Id == m.Id).FirstOrDefault();
        //                        orderModel.OrderField.Add(new OrderField("被保人", orderToCarInsure.CarOwner));
        //                        orderModel.OrderField.Add(new OrderField("车牌号码", orderToCarInsure.CarPlateNo));
        //                        orderModel.OrderField.Add(new OrderField(orderToCarInsure.InsuranceCompanyName, orderToCarInsure.Price.ToPrice()));


        //                        break;
        //                    case Enumeration.ProductType.InsureForCarForRenewal:
        //                        orderModel.Remarks = string.Format("合计:{0}", m.Price);//续保 保单金额

        //                        var orderToCarRenewal = CurrentDb.OrderToCarInsure.Where(c => c.Id == m.Id).FirstOrDefault();
        //                        orderModel.OrderField.Add(new OrderField("被保人", orderToCarRenewal.CarOwner));
        //                        orderModel.OrderField.Add(new OrderField("车牌号码", orderToCarRenewal.CarPlateNo));
        //                        orderModel.OrderField.Add(new OrderField(orderToCarRenewal.InsuranceCompanyName, orderToCarRenewal.Price.ToPrice()));


        //                        break;
        //                    case Enumeration.ProductType.InsureForCarForClaim:
        //                        orderModel.Remarks = string.Format("合计:{0}", m.Price);//理赔 理赔金额

        //                        var orderToCarClaim = CurrentDb.OrderToCarClaim.Where(c => c.Id == m.Id).FirstOrDefault();

        //                        orderModel.OrderField.Add(new OrderField("车牌号码", orderToCarClaim.CarPlateNo));
        //                        orderModel.OrderField.Add(new OrderField("工时费", orderToCarClaim.WorkingHoursPrice.ToPrice()));
        //                        orderModel.OrderField.Add(new OrderField("配件费", orderToCarClaim.AccessoriesPrice.ToPrice()));
        //                        orderModel.OrderField.Add(new OrderField("合计", orderToCarClaim.Price.ToPrice()));

        //                        break;
        //                }



        //                break;
        //            case Enumeration.OrderStatus.Cancled:
        //                orderModel.Remarks = m.CancleTime.ToUnifiedFormatDateTime();//备注完成时间
        //                orderModel.StatusName = "已取消";

        //                orderModel.OrderField.Add(new OrderField(m.Remarks, ""));

        //                break;
        //        }


        //        model.Add(orderModel);
        //    }

        //    PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = model };

        //    return Json(ResultType.Success, pageEntity, "");

        //}

        //public JsonResult PayTest(PayConfirmModel model)
        //{
        //    CustomJsonResult result = new CustomJsonResult();


        //    switch (model.ProductType)
        //    {
        //        case Enumeration.ProductType.InsureForCarForInsure:
        //        case Enumeration.ProductType.InsureForCarForRenewal:
        //            result = BizFactory.Pay.PayCarInsureCompleted(this.CurrentUserId, model.OrderSn);
        //            break;
        //        case Enumeration.ProductType.InsureForCarForClaim:

        //            break;
        //    }


        //    return result;
        //}

        //public JsonResult PayConfirmTest(PayConfirmModel model)
        //{
        //    CustomJsonResult result = new CustomJsonResult();

        //    result = BizFactory.Pay.Confirm(this.CurrentUserId, model);

        //    return result;
        //}

        public JsonResult Cancle(Order model)
        {
            CustomJsonResult result = new CustomJsonResult();

            result = BizFactory.Order.Cancle(this.CurrentUserId, model.Sn);

            return result;
        }

        public JsonResult SubmitCarInsureFollow(CarInsureFollowViewModel model)
        {
            CustomJsonResult result = new CustomJsonResult();

            result = BizFactory.Order.SubmitFollowInsure(this.CurrentUserId, model.OrderToCarInsure);

            return result;
        }

        public JsonResult SubmitEstimateList(CarClaimFollowViewModel model)
        {
            CustomJsonResult result = new CustomJsonResult();

            result = BizFactory.Order.SubmitEstimateList(this.CurrentUserId, model.OrderToCarClaim.UserId, model.OrderToCarClaim.Id, model.OrderToCarClaim.EstimateListImgUrl);

            return result;
        }



    }
}