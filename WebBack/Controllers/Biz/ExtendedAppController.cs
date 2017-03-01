﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBack.Models.Biz.ExtendedApp;
using Lumos.Common;
using Lumos.Entity;
using Lumos.Mvc;
using Lumos.BLL;

namespace WebBack.Controllers.Biz
{
    public class ExtendedAppController : WebBackController
    {
        // GET: ExtendedApp


        public ViewResult List()
        {
            return View();
        }

        public ViewResult Details()
        {
            return View();
        }

        public ViewResult ApplyList()
        {
            return View();
        }

        public ViewResult OnList()
        {
            return View();
        }

        public ViewResult OffList()
        {
            return View();
        }

        public ViewResult ApplyOn()
        {
            ApplyOnViewModel model = new ApplyOnViewModel();

            return View(model);
        }

        public ViewResult ApplyOff()
        {
            ApplyOffViewModel model = new ApplyOffViewModel();
            return View(model);
        }

        public ViewResult ApplyRecovery()
        {
            ApplyRecoveryViewModel model = new ApplyRecoveryViewModel();
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


        public JsonResult GetList(ExtendedAppSearchCondition condition)
        {
            string name = condition.Name.ToSearchString();
            var query = (from e in CurrentDb.ExtendedApp
                         where
                                 (name.Length == 0 || e.Name.Contains(name))

                         select new { e.Id, e.ImgUrl, e.LinkUrl, e.Name, e.Status, e.CreateTime, e.IsDisplay });

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
                    item.ImgUrl,
                    item.LinkUrl,
                    item.Name,
                    Status = item.Status.GetCnName(),
                    IsDisplay=GetIsDisplay(item.IsDisplay),
                    item.CreateTime

                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list };

            return Json(ResultType.Success, pageEntity, "");
        }


        public string GetIsDisplay(bool? isDisplay)
        {
            if (isDisplay == null)
            {
                return "正在审核";
            }

            if (isDisplay.Value)
            {
                return "是";
            }

            return "否";
        }


        public JsonResult GetOnList(ExtendedAppSearchCondition condition)
        {
            string name = condition.Name.ToSearchString();
            var query = (from e in CurrentDb.ExtendedApp
                         where
                          e.Type!= Enumeration.ExtendedAppType.CarService&&
                                 (name.Length == 0 || e.Name.Contains(name))
                                 && e.IsDisplay == true
                         select new { e.Id, e.ImgUrl, e.LinkUrl, e.Name, e.CreateTime, e.Description });

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
                    item.ImgUrl,
                    item.LinkUrl,
                    item.Name,
                    item.CreateTime,
                    item.Description
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list };

            return Json(ResultType.Success, pageEntity, "");
        }

        public JsonResult GetOffList(ExtendedAppSearchCondition condition)
        {
            string name = condition.Name.ToSearchString();
            var query = (from e in CurrentDb.ExtendedApp
                         where
                                 (name.Length == 0 || e.Name.Contains(name))
                                 && e.IsDisplay == false
                         select new { e.Id, e.ImgUrl, e.LinkUrl, e.Name, e.CreateTime, e.Description });

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
                    item.ImgUrl,
                    item.LinkUrl,
                    item.Name,
                    item.CreateTime,
                    item.Description

                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list };

            return Json(ResultType.Success, pageEntity, "");
        }

        public JsonResult GetApplyList(ExtendedAppSearchCondition condition)
        {
            string name = condition.Name.ToSearchString();
            var query = (from b in CurrentDb.BizProcessesAudit
                         join e in CurrentDb.ExtendedApp on
                        b.AduitReferenceId equals e.Id
                         where (b.AduitType == Enumeration.BizProcessesAuditType.ExtendedAppOff
                         || b.AduitType == Enumeration.BizProcessesAuditType.ExtendedAppOn || b.AduitType == Enumeration.BizProcessesAuditType.ExtendedAppRecovery) &&
                         e.Type == Enumeration.ExtendedAppType.ThirdPartyApp &&
                                 (name.Length == 0 || e.Name.Contains(name))

                         select new { e.Id, b.AduitType, e.ImgUrl, e.LinkUrl, e.Name, b.Status, b.CreateTime });

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
                    item.ImgUrl,
                    item.LinkUrl,
                    AduitType = item.AduitType.GetCnName(),
                    item.Name,
                    Status = ((Enumeration.ExtendedAppAuditStatus)item.Status).GetCnName(),
                    item.CreateTime,

                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list };

            return Json(ResultType.Success, pageEntity, "");
        }

        [HttpPost]
        public JsonResult ApplyOn(ApplyOnViewModel model)
        {
            CustomJsonResult reuslt = new CustomJsonResult();

            reuslt = BizFactory.ExtendedApp.ApplyOn(this.CurrentUserId, model.ExtendedApp);

            return reuslt;
        }

        [HttpPost]
        public JsonResult ApplyOff(ApplyOffViewModel model)
        {
            CustomJsonResult reuslt = new CustomJsonResult();

            reuslt = BizFactory.ExtendedApp.ApplyOff(this.CurrentUserId, model.ExtendedApp.Id, model.Remarks);

            return reuslt;
        }

        [HttpPost]
        public JsonResult ApplyRecovery(ApplyOffViewModel model)
        {
            CustomJsonResult reuslt = new CustomJsonResult();

            reuslt = BizFactory.ExtendedApp.ApplyRecovery(this.CurrentUserId, model.ExtendedApp.Id, model.Remarks);

            return reuslt;
        }

        public JsonResult GetPrimaryAuditList(ExtendedAppSearchCondition condition)
        {
            var waitCount = (from h in CurrentDb.BizProcessesAudit where (h.AduitType == Enumeration.BizProcessesAuditType.ExtendedAppOn || h.AduitType == Enumeration.BizProcessesAuditType.ExtendedAppOff || h.AduitType == Enumeration.BizProcessesAuditType.ExtendedAppRecovery) && h.Status == (int)Enumeration.ExtendedAppAuditStatus.WaitAudit select h.Id).Count();
            var handleCount = (from h in CurrentDb.BizProcessesAudit where (h.AduitType == Enumeration.BizProcessesAuditType.ExtendedAppOn || h.AduitType == Enumeration.BizProcessesAuditType.ExtendedAppOff || h.AduitType == Enumeration.BizProcessesAuditType.ExtendedAppRecovery) && h.Status == (int)Enumeration.ExtendedAppAuditStatus.InAudit && h.Auditor == this.CurrentUserId select h.Id).Count();


            var query = (from b in CurrentDb.BizProcessesAudit
                         join e in CurrentDb.ExtendedApp on
                        b.AduitReferenceId equals e.Id
                         where (b.AduitType == Enumeration.BizProcessesAuditType.ExtendedAppOff
                         || b.AduitType == Enumeration.BizProcessesAuditType.ExtendedAppOn
                         || b.AduitType == Enumeration.BizProcessesAuditType.ExtendedAppRecovery
                         ) &&
                         e.Type == Enumeration.ExtendedAppType.ThirdPartyApp
                         select new { b.Id, b.AduitType, e.ImgUrl, e.LinkUrl, e.Name, b.Status, b.CreateTime, b.Auditor, b.Remark });

            if (condition.AuditStatus == Enumeration.ExtendedAppAuditStatus.WaitAudit)
            {
                query = query.Where(m => m.Status == (int)Enumeration.ExtendedAppAuditStatus.WaitAudit);
            }
            else if (condition.AuditStatus == Enumeration.ExtendedAppAuditStatus.InAudit)
            {
                query = query.Where(m => m.Status == (int)Enumeration.ExtendedAppAuditStatus.InAudit && m.Auditor == this.CurrentUserId);
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
                    item.ImgUrl,
                    item.LinkUrl,
                    item.Name,
                    AuditStatus = item.Status,
                    AduitType = item.AduitType.GetCnName(),
                    item.CreateTime,
                    item.Remark,
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list, Status = new { waitCount = waitCount, handleCount = handleCount } };

            return Json(ResultType.Success, pageEntity, "");
        }

        [HttpPost]
        public JsonResult PrimaryAudit(PrimaryAuditViewModel model)
        {
            CustomJsonResult reuslt = new CustomJsonResult();
            reuslt = BizFactory.ExtendedApp.PrimaryAudit(this.CurrentUserId, model.Operate, model.ExtendedApp, model.BizProcessesAuditDetails);

            return reuslt;
        }

        public JsonResult GetSeniorAuditList(ExtendedAppSearchCondition condition)
        {
            var waitCount = (from h in CurrentDb.BizProcessesAudit where (h.AduitType == Enumeration.BizProcessesAuditType.ExtendedAppOn || h.AduitType == Enumeration.BizProcessesAuditType.ExtendedAppOff || h.AduitType == Enumeration.BizProcessesAuditType.ExtendedAppRecovery) && h.Status == (int)Enumeration.ExtendedAppAuditStatus.WaitReview select h.Id).Count();
            var handleCount = (from h in CurrentDb.BizProcessesAudit where (h.AduitType == Enumeration.BizProcessesAuditType.ExtendedAppOn || h.AduitType == Enumeration.BizProcessesAuditType.ExtendedAppOff || h.AduitType == Enumeration.BizProcessesAuditType.ExtendedAppRecovery) && h.Status == (int)Enumeration.ExtendedAppAuditStatus.InReview && h.Auditor == this.CurrentUserId select h.Id).Count();


            var query = (from b in CurrentDb.BizProcessesAudit
                         join e in CurrentDb.ExtendedApp on
                        b.AduitReferenceId equals e.Id
                         where (b.AduitType == Enumeration.BizProcessesAuditType.ExtendedAppOff
                         || b.AduitType == Enumeration.BizProcessesAuditType.ExtendedAppOn
                             || b.AduitType == Enumeration.BizProcessesAuditType.ExtendedAppRecovery
                         ) &&
                         e.Type == Enumeration.ExtendedAppType.ThirdPartyApp
                         select new { b.Id, b.AduitType, e.ImgUrl, e.LinkUrl, e.Name, b.Status, b.CreateTime, b.Auditor, b.Remark });

            if (condition.AuditStatus == Enumeration.ExtendedAppAuditStatus.WaitReview)
            {
                query = query.Where(m => m.Status == (int)Enumeration.ExtendedAppAuditStatus.WaitReview);
            }
            else if (condition.AuditStatus == Enumeration.ExtendedAppAuditStatus.InReview)
            {
                query = query.Where(m => m.Status == (int)Enumeration.ExtendedAppAuditStatus.InReview && m.Auditor == this.CurrentUserId);
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
                    item.ImgUrl,
                    item.LinkUrl,
                    item.Name,
                    AuditStatus = item.Status,
                    AduitType = item.AduitType.GetCnName(),
                    item.CreateTime,
                    item.Remark,
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list, Status = new { waitCount = waitCount, handleCount = handleCount } };

            return Json(ResultType.Success, pageEntity, "");
        }

        [HttpPost]
        public JsonResult SeniorAudit(SeniorAuditViewModel model)
        {
            CustomJsonResult reuslt = new CustomJsonResult();
            reuslt = BizFactory.ExtendedApp.SeniorAudit(this.CurrentUserId, model.Operate, model.AuditCommentsCurrent);

            return reuslt;
        }
    }
}