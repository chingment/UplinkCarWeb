using Lumos.BLL;
using Lumos.Entity;
using Lumos.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBack.Models.Biz.CarInsureCommissionRate;

namespace WebBack.Controllers.Biz
{
    public class CarInsureCommissionRateController : WebBackController
    {
        // GET: Commission
        public ViewResult List()
        {
            return View();
        }

        public ViewResult Details(int id)
        {
            DetailsViewModel model = new DetailsViewModel(id);
            return View(model);
        }

        public ViewResult ApplyList()
        {
            return View();
        }

        public ViewResult Apply()
        {
            ApplyViewModel model = new ApplyViewModel();
            return View(model);
        }

        public ViewResult PrimaryAuditList()
        {
            return View();
        }
        public ViewResult PrimaryAudit(int id)
        {
            PrimaryAuditViewModel model = new PrimaryAuditViewModel(id);
            return View(model);
        }

        public ViewResult SeniorAuditList()
        {
            return View();
        }

        public ViewResult SeniorAudit(int id)
        {
            SeniorAuditViewModel model = new SeniorAuditViewModel(id);
            return View(model);
        }

        public JsonResult GetList(CarInsureCommissionRateCondition condition)
        {
            string name = condition.Name.ToSearchString();

            var query = (from m in CurrentDb.CarInsureCommissionRate
                         where
                                 (name.Length == 0 || m.ReferenceName.Contains(name))

                         select new { m.Id, m.ReferenceName, m.Commercial, m.Compulsory, m.CreateTime });

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
                    item.ReferenceName,
                    Commercial = item.Commercial.ToPrice(),
                    Compulsory = item.Compulsory.ToPrice()
                });


            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list };

            return Json(ResultType.Success, pageEntity, "");
        }

        [HttpPost]
        public JsonResult Apply(ApplyViewModel model)
        {
            CustomJsonResult reuslt = new CustomJsonResult();

            reuslt = BizFactory.CarInsureCommissionRate.Apply(this.CurrentUserId, model.CommissionRate, model.Reason);

            return reuslt;
        }

        public JsonResult GetApplyList(CarInsureCommissionRateCondition condition)
        {
            string name = condition.Name.ToSearchString();
            var query = (from b in CurrentDb.BizProcessesAudit
                         join e in CurrentDb.CarInsureCommissionRate on
                        b.AduitReferenceId equals e.Id
                         where b.AduitType == Enumeration.BizProcessesAuditType.CommissionRateAudit &&
                                 (name.Length == 0 || e.ReferenceName.Contains(name))

                         select new { e.Id, e.ReferenceName, e.Commercial, e.Compulsory, b.Status, b.CreateTime, b.JsonData });

            int total = query.Count();

            int pageIndex = condition.PageIndex;
            int pageSize = 10;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            List<object> list = new List<object>();

            foreach (var item in query)
            {
                CommissionRateAdjustModel commissionRateAdjustModel = Newtonsoft.Json.JsonConvert.DeserializeObject<CommissionRateAdjustModel>(item.JsonData);
                list.Add(new
                {
                    item.Id,
                    BeforeCommercial = commissionRateAdjustModel.Before.Commercial.ToPrice(),
                    AfterCommercial = commissionRateAdjustModel.After.Commercial.ToPrice(),
                    BeforeCompulsory = commissionRateAdjustModel.Before.Compulsory.ToPrice(),
                    AfterCompulsory = commissionRateAdjustModel.After.Compulsory.ToPrice(),
                    item.ReferenceName,
                    Status = ((Enumeration.CommissionRateAuditStatus)item.Status).GetCnName(),
                    item.CreateTime,

                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list };

            return Json(ResultType.Success, pageEntity, "");
        }

        public JsonResult GetPrimaryAuditList(CarInsureCommissionRateCondition condition)
        {
            var waitAuditCount = (from h in CurrentDb.BizProcessesAudit where (h.AduitType == Enumeration.BizProcessesAuditType.CommissionRateAudit) && h.Status == (int)Enumeration.CommissionRateAuditStatus.WaitPrimaryAudit select h.Id).Count();
            var inAuditCount = (from h in CurrentDb.BizProcessesAudit where (h.AduitType == Enumeration.BizProcessesAuditType.CommissionRateAudit) && h.Status == (int)Enumeration.CommissionRateAuditStatus.InPrimaryAudit && h.Auditor == this.CurrentUserId select h.Id).Count();


            var query = (from b in CurrentDb.BizProcessesAudit
                         join e in CurrentDb.CarInsureCommissionRate on
                        b.AduitReferenceId equals e.Id
                         where (b.AduitType == Enumeration.BizProcessesAuditType.CommissionRateAudit)
                         select new { b.Id, e.ReferenceName, e.Commercial, e.Compulsory, b.Status, b.Auditor, b.CreateTime, b.JsonData });

            if (condition.AuditStatus == Enumeration.CommissionRateAuditStatus.WaitPrimaryAudit)
            {
                query = query.Where(m => m.Status == (int)Enumeration.CommissionRateAuditStatus.WaitPrimaryAudit);
            }
            else if (condition.AuditStatus == Enumeration.CommissionRateAuditStatus.InPrimaryAudit)
            {
                query = query.Where(m => m.Status == (int)Enumeration.CommissionRateAuditStatus.InPrimaryAudit && m.Auditor == this.CurrentUserId);
            }

            int total = query.Count();

            int pageIndex = condition.PageIndex;
            int pageSize = 10;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            List<object> list = new List<object>();
            foreach (var item in query)
            {

                CommissionRateAdjustModel commissionRateAdjustModel = Newtonsoft.Json.JsonConvert.DeserializeObject<CommissionRateAdjustModel>(item.JsonData);
                list.Add(new
                {
                    item.Id,
                    BeforeCommercial = commissionRateAdjustModel.Before.Commercial.ToPrice(),
                    AfterCommercial = commissionRateAdjustModel.After.Commercial.ToPrice(),
                    BeforeCompulsory = commissionRateAdjustModel.Before.Compulsory.ToPrice(),
                    AfterCompulsory = commissionRateAdjustModel.After.Compulsory.ToPrice(),
                    AuditStatus =item.Status,
                    item.ReferenceName,
                    item.CreateTime,

                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list, Status = new { waitAuditCount = waitAuditCount, inAuditCount = inAuditCount } };

            return Json(ResultType.Success, pageEntity, "");
        }

        [HttpPost]
        public JsonResult PrimaryAudit(PrimaryAuditViewModel model)
        {
            return BizFactory.CarInsureCommissionRate.PrimaryAudit(this.CurrentUserId, model.Operate, model.CommissionRate, model.BizProcessesAudit);
        }

        public JsonResult GetSeniorAuditList(CarInsureCommissionRateCondition condition)
        {
            var waitAuditCount = (from h in CurrentDb.BizProcessesAudit where (h.AduitType == Enumeration.BizProcessesAuditType.CommissionRateAudit) && h.Status == (int)Enumeration.CommissionRateAuditStatus.WaitSeniorAudit select h.Id).Count();
            var inAuditCount = (from h in CurrentDb.BizProcessesAudit where (h.AduitType == Enumeration.BizProcessesAuditType.CommissionRateAudit) && h.Status == (int)Enumeration.CommissionRateAuditStatus.InSeniorAudit && h.Auditor == this.CurrentUserId select h.Id).Count();


            var query = (from b in CurrentDb.BizProcessesAudit
                         join e in CurrentDb.CarInsureCommissionRate on
                        b.AduitReferenceId equals e.Id
                         where (b.AduitType == Enumeration.BizProcessesAuditType.CommissionRateAudit)
                         select new { b.Id, e.ReferenceName, e.Commercial, e.Compulsory, b.Status, b.Auditor, b.CreateTime, b.JsonData });

            if (condition.AuditStatus == Enumeration.CommissionRateAuditStatus.WaitSeniorAudit)
            {
                query = query.Where(m => m.Status == (int)Enumeration.CommissionRateAuditStatus.WaitSeniorAudit);
            }
            else if (condition.AuditStatus == Enumeration.CommissionRateAuditStatus.InSeniorAudit)
            {
                query = query.Where(m => m.Status == (int)Enumeration.CommissionRateAuditStatus.InSeniorAudit && m.Auditor == this.CurrentUserId);
            }

            int total = query.Count();

            int pageIndex = condition.PageIndex;
            int pageSize = 10;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            List<object> list = new List<object>();
            foreach (var item in query)
            {

                CommissionRateAdjustModel commissionRateAdjustModel = Newtonsoft.Json.JsonConvert.DeserializeObject<CommissionRateAdjustModel>(item.JsonData);
                list.Add(new
                {
                    item.Id,
                    BeforeCommercial = commissionRateAdjustModel.Before.Commercial.ToPrice(),
                    AfterCommercial = commissionRateAdjustModel.After.Commercial.ToPrice(),
                    BeforeCompulsory = commissionRateAdjustModel.Before.Compulsory.ToPrice(),
                    AfterCompulsory = commissionRateAdjustModel.After.Compulsory.ToPrice(),
                    AuditStatus =item.Status,
                    item.ReferenceName,
                    item.CreateTime,

                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list, Status = new { waitAuditCount = waitAuditCount, inAuditCount = inAuditCount } };

            return Json(ResultType.Success, pageEntity, "");
        }

        [HttpPost]
        public JsonResult SeniorAudit(PrimaryAuditViewModel model)
        {
            return BizFactory.CarInsureCommissionRate.SeniorAudit(this.CurrentUserId, model.Operate, model.CommissionRate, model.BizProcessesAudit);
        }
    }
}