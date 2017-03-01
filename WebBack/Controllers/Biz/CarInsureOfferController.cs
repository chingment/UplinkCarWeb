using Lumos.BLL;
using Lumos.Entity;
using Lumos.Mvc;
using Lumos.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBack.Models.Biz.CarInsureOffer;

namespace WebBack.Controllers.Biz
{
    public class CarInsureOfferController : WebBackController
    {
        public ViewResult List()
        {
            return View();
        }

        public ViewResult Details(int id)
        {
            DetailsViewModel model = new DetailsViewModel(id);
            return View(model);
        }


        public ViewResult DealtList()
        {
            return View();
        }

        public ViewResult Dealt(int id)
        {
            DealtViewModel model = new DealtViewModel(id);

            return View(model);
        }

        public JsonResult GetList(CarInsureOfferSearchCondition condition)
        {

            string sn = condition.Sn.ToSearchString();
            string clientCode = condition.ClientCode.ToSearchString();
            string clientName = condition.ClientName.ToSearchString();


            var query = (from o in CurrentDb.OrderToCarInsure
                         join m in CurrentDb.Merchant on o.MerchantId equals m.Id
                         where o.PId == null &&
                               (o.Status == condition.Status || condition.Status == Enumeration.OrderStatus.Unknow) &&
                        (sn.Length == 0 || o.Sn.Contains(sn)) &&
                            (clientCode.Length == 0 || m.ClientCode.Contains(clientCode)) &&
                                (clientCode.Length == 0 || m.YYZZ_Name.Contains(clientCode))


                         select new { o.Id, m.ClientCode, o.Sn, m.YYZZ_Name, m.ContactPhoneNumber, o.ProductName,o.ProductType, o.SubmitTime, o.Status, o.CreateTime });



            int total = query.Count();

            int pageIndex = condition.PageIndex;
            int pageSize = 10;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

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
                    item.SubmitTime,
                    Status = item.Status.GetCnName()
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list };

            return Json(ResultType.Success, pageEntity, "");
        }


        public JsonResult GetDealtList(CarInsureOfferSearchCondition condition)
        {
            var waitOfferCount = (from h in CurrentDb.BizProcessesAudit where (h.AduitType == Enumeration.BizProcessesAuditType.CarInsureOffer) && h.Status == (int)Enumeration.CarInsureOfferDealtStatus.WaitOffer select h.Id).Count();
            var inOfferCount = (from h in CurrentDb.BizProcessesAudit where (h.AduitType == Enumeration.BizProcessesAuditType.CarInsureOffer) && h.Status == (int)Enumeration.CarInsureOfferDealtStatus.InOffer && h.Auditor == this.CurrentUserId select h.Id).Count();

            var query = (from b in CurrentDb.BizProcessesAudit
                         join o in CurrentDb.OrderToCarInsure on
                         b.AduitReferenceId equals o.Id
                         join m in CurrentDb.Merchant on o.MerchantId equals m.Id
                         where b.AduitType == Enumeration.BizProcessesAuditType.CarInsureOffer


                         select new { b.Id, m.ClientCode, o.Sn, m.YYZZ_Name, m.ContactPhoneNumber, o.ProductName, o.SubmitTime, b.Status, b.CreateTime, b.Auditor });

            if (condition.DealtStatus == Enumeration.CarInsureOfferDealtStatus.WaitOffer)
            {
                query = query.Where(m => m.Status == (int)Enumeration.CarInsureOfferDealtStatus.WaitOffer);
            }
            else if (condition.DealtStatus == Enumeration.CarInsureOfferDealtStatus.InOffer)
            {
                query = query.Where(m => m.Status == (int)Enumeration.CarInsureOfferDealtStatus.InOffer && m.Auditor == this.CurrentUserId);
            }



            int total = query.Count();

            int pageIndex = condition.PageIndex;
            int pageSize = 10;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

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
                    item.SubmitTime,
                    DealtStatus = item.Status
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list, Status = new { waitOfferCount = waitOfferCount, inOfferCount = inOfferCount } };




            //var waitCount = (from o in CurrentDb.OrderToCarInsure where o.DealtStatus == Enumeration.DealtStatus.Wait select o.Id).Count();
            //var handleCount = (from o in CurrentDb.OrderToCarInsure where o.DealtStatus == Enumeration.DealtStatus.Handle && o.Dealtor == this.CurrentUserId select o.Id).Count();
            //var completeCount = (from o in CurrentDb.OrderToCarInsure where o.DealtStatus == Enumeration.DealtStatus.Complete select o.Id).Count();



            //var query = (from o in CurrentDb.OrderToCarInsure
            //             join m in CurrentDb.Merchant on o.MerchantId equals m.Id
            //             select new { o.Id, m.ClientCode, o.Sn, m.YYZZ_Name, m.ContactPhoneNumber, o.ProductName, o.SubmitTime, o.DealtStatus, o.Dealtor });

            //if (condition.DealtStatus == Enumeration.DealtStatus.Wait)
            //{
            //    query = query.Where(m => m.DealtStatus == Enumeration.DealtStatus.Wait);
            //}
            //else if (condition.DealtStatus == Enumeration.DealtStatus.Handle)
            //{
            //    query = query.Where(m => m.DealtStatus == Enumeration.DealtStatus.Handle && m.Dealtor == this.CurrentUserId);
            //}
            //else if (condition.DealtStatus == Enumeration.DealtStatus.Complete)
            //{
            //    query = query.Where(m => m.DealtStatus == Enumeration.DealtStatus.Complete);
            //}


            //condition.Total = query.Count();
            //query = query.OrderBy(r => r.SubmitTime).Skip(condition.PageSize * (condition.PageIndex)).Take(condition.PageSize);

            //List<object> list = new List<object>();

            //foreach (var item in query)
            //{
            //    list.Add(new
            //    {
            //        item.Id,
            //        item.ClientCode,
            //        item.Sn,
            //        item.YYZZ_Name,
            //        item.ContactPhoneNumber,
            //        item.ProductName,
            //        item.SubmitTime,
            //        item.DealtStatus
            //    });
            //}

            //PageEntity pageEntity = new PageEntity { PageSize = condition.PageSize, TotalRecord = condition.Total, Rows = list, Status = new { waitCount = waitCount, handleCount = handleCount, completeCount = completeCount } };

            return Json(ResultType.Success, pageEntity, "");
        }

        [HttpPost]
        public JsonResult Dealt(DealtViewModel model)
        {
            CustomJsonResult reuslt = new CustomJsonResult();

            reuslt = BizFactory.Order.SubmitCarInsureOffer(this.CurrentUserId, model.Operate, model.OrderToCarInsure, model.OrderToCarInsureOfferCompany, model.BizProcessesAudit);

            return reuslt;
        }
    }
}