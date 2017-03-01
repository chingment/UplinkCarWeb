﻿using Lumos.Entity;
using Lumos.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBack.Models.Biz.Merchant;
using Lumos.Common;
using Lumos.BLL;

namespace WebBack.Controllers.Biz
{
    public class MerchantController : WebBackController
    {
        // GET: Merchant
        public ViewResult OpenAccountList()
        {
            return View();
        }

        public ViewResult OpenAccount()
        {
            return View();
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

        public ViewResult EditList()
        {
            return View();
        }

        public ViewResult List()
        {
            return View();
        }

        public ViewResult PosMachineList()
        {
            return View();
        }

        public ViewResult Details(int id)
        {
            DetailsViewModel model = new DetailsViewModel(id);
            return View(model);
        }

        public ViewResult Edit(int id)
        {
            EditViewModel model = new EditViewModel(id);
            return View(model);
        }

        public ViewResult WithdrawDetails(int id)
        {
            Models.Biz.Withdraw.DetailsViewModel model = new Models.Biz.Withdraw.DetailsViewModel(id);
            return View(model);
        }

        public ViewResult TransactionsDetails(int id)
        {
            EditViewModel model = new EditViewModel(id);
            return View(model);
        }



        public JsonResult GetPosMachineList(Models.Biz.PosMachine.PosMachineSearchCondition condition)
        {


            string fuselageNumber = condition.FuselageNumber.ToSearchString();
            string terminalNumber = condition.TerminalNumber.ToSearchString();
            var list = (from p in CurrentDb.PosMachine
                        where (fuselageNumber.Length == 0 || p.FuselageNumber.Contains(fuselageNumber)) &&
                                (terminalNumber.Length == 0 || p.TerminalNumber.Contains(terminalNumber)) &&
                                p.IsUse == false
                        select new { p.Id, p.FuselageNumber, p.TerminalNumber, p.CreateTime, p.Version, p.DeviceId, p.Deposit, p.Rent });

            int total = list.Count();

            int pageIndex = condition.PageIndex;
            int pageSize = 10;
            list = list.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list };

            return Json(ResultType.Success, pageEntity, "");
        }

        public JsonResult GetOpenAccountList(MerchantSearchCondition condition)
        {
            string clientCode = condition.ClientCode.ToSearchString();
            string yYZZ_Name = condition.YYZZ_Name.ToSearchString();
            string yYZZ_RegisterNo = condition.YYZZ_RegisterNo.ToSearchString();
            var query = (from m in CurrentDb.Merchant
                         join u in CurrentDb.SysClientUser on m.UserId equals u.Id
                         where (clientCode.Length == 0 || u.ClientCode.Contains(clientCode)) &&
                                 (yYZZ_Name.Length == 0 || m.YYZZ_Name.Contains(yYZZ_Name)) &&
                                 (yYZZ_RegisterNo.Length == 0 || m.YYZZ_RegisterNo.Contains(yYZZ_RegisterNo))
                         select new { m.Id, u.ClientCode, m.YYZZ_Name, m.FR_Name, m.ContactName, m.ContactPhoneNumber, m.CreateTime });

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
                    item.YYZZ_Name,
                    item.ContactName,
                    item.ContactPhoneNumber,
                    DeviceId = GetDeviceId(item.Id),
                    item.CreateTime
                });


            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list };

            return Json(ResultType.Success, pageEntity, "");
        }

        public string GetDeviceId(int merchantId)
        {
            Lumos.DAL.LumosDbContext db = new Lumos.DAL.LumosDbContext();
            string deviceId = "";
            var merchantPosMachine = db.MerchantPosMachine.Where(m => m.MerchantId == merchantId).FirstOrDefault();
            if (merchantPosMachine != null)
            {
                var posMachine = db.PosMachine.Where(m => m.Id == merchantPosMachine.PosMachineId).FirstOrDefault();
                if (posMachine != null)
                {
                    deviceId = posMachine.DeviceId;
                }
            }

            return deviceId;
        }

        [HttpPost]
        public JsonResult OpenAccount(OpenAccountViewModel model)
        {
            return BizFactory.Merchant.OpenAccount(this.CurrentUserId, model.Merchant, model.MerchantPosMachine, model.BankCard);
        }

        public JsonResult GetList(MerchantSearchCondition condition)
        {
            string clientCode = condition.ClientCode.ToSearchString();
            string yYZZ_Name = condition.YYZZ_Name.ToSearchString();
            string yYZZ_RegisterNo = condition.YYZZ_RegisterNo.ToSearchString();
            var query = (from m in CurrentDb.Merchant
                         join u in CurrentDb.SysClientUser on m.UserId equals u.Id
                         where (clientCode.Length == 0 || u.ClientCode.Contains(clientCode)) &&
                                 (yYZZ_Name.Length == 0 || m.YYZZ_Name.Contains(yYZZ_Name)) &&
                                 (yYZZ_RegisterNo.Length == 0 || m.YYZZ_RegisterNo.Contains(yYZZ_RegisterNo))
                         select new { m.Id, u.ClientCode, m.Type, m.RepairCapacity, m.Area, m.YYZZ_Name, m.FR_Name, m.ContactName, m.CreateTime });

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
                    item.YYZZ_Name,
                    item.ContactName,
                    Type = item.Type.GetCnName(),
                    RepairCapacity = item.RepairCapacity.GetCnName(),
                    item.Area,
                    item.CreateTime
                });


            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list };

            return Json(ResultType.Success, pageEntity, "");
        }

        public JsonResult GetEditList(MerchantSearchCondition condition)
        {
            string clientCode = condition.ClientCode.ToSearchString();
            string yYZZ_Name = condition.YYZZ_Name.ToSearchString();
            string yYZZ_RegisterNo = condition.YYZZ_RegisterNo.ToSearchString();
            var query = (from m in CurrentDb.Merchant
                         join u in CurrentDb.SysClientUser on m.UserId equals u.Id
                         where (clientCode.Length == 0 || u.ClientCode.Contains(clientCode)) &&
                                 (yYZZ_Name.Length == 0 || m.YYZZ_Name.Contains(yYZZ_Name)) &&
                                 (yYZZ_RegisterNo.Length == 0 || m.YYZZ_RegisterNo.Contains(yYZZ_RegisterNo))
                         select new { m.Id, u.ClientCode, m.Type, m.RepairCapacity, m.Area, m.YYZZ_Name, m.FR_Name, m.ContactName, m.CreateTime });

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
                    item.YYZZ_Name,
                    item.ContactName,
                    Type = item.Type.GetCnName(),
                    RepairCapacity = item.RepairCapacity.GetCnName(),
                    item.Area,
                    item.CreateTime
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list };

            return Json(ResultType.Success, pageEntity, "");
        }

        [HttpPost]
        public JsonResult Edit(EditViewModel model)
        {
            return BizFactory.Merchant.Edit(this.CurrentUserId, model.Merchant, model.EstimateInsuranceCompanyIds, model.MerchantPosMachine, model.BankCard);
        }

        public JsonResult GetPrimaryAuditList(MerchantSearchCondition condition)
        {
            var waitAuditCount = (from h in CurrentDb.BizProcessesAudit where (h.AduitType == Enumeration.BizProcessesAuditType.MerchantAudit) && h.Status == (int)Enumeration.MerchantAuditStatus.WaitPrimaryAudit select h.Id).Count();
            var inAuditCount = (from h in CurrentDb.BizProcessesAudit where (h.AduitType == Enumeration.BizProcessesAuditType.MerchantAudit) && h.Status == (int)Enumeration.MerchantAuditStatus.InPrimaryAudit && h.Auditor == this.CurrentUserId select h.Id).Count();


            var query = (from b in CurrentDb.BizProcessesAudit
                         join m in CurrentDb.Merchant on
                        b.AduitReferenceId equals m.Id
                         where (b.AduitType == Enumeration.BizProcessesAuditType.MerchantAudit)
                         select new { b.Id, b.Status, b.Auditor, m.ClientCode, m.Type, m.RepairCapacity, m.Area, m.YYZZ_Name, m.FR_Name, m.ContactName, m.CreateTime });

            if (condition.AuditStatus == Enumeration.MerchantAuditStatus.WaitPrimaryAudit)
            {
                query = query.Where(m => m.Status == (int)Enumeration.MerchantAuditStatus.WaitPrimaryAudit);
            }
            else if (condition.AuditStatus == Enumeration.MerchantAuditStatus.InPrimaryAudit)
            {
                query = query.Where(m => m.Status == (int)Enumeration.MerchantAuditStatus.InPrimaryAudit && m.Auditor == this.CurrentUserId);
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
                    item.YYZZ_Name,
                    item.ContactName,
                    Type = item.Type.GetCnName(),
                    RepairCapacity = item.RepairCapacity.GetCnName(),
                    item.Area,
                    item.CreateTime,
                    AuditStatus = item.Status
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list, Status = new { waitAuditCount = waitAuditCount, inAuditCount = inAuditCount } };

            return Json(ResultType.Success, pageEntity, "");
        }

        [HttpPost]
        public JsonResult PrimaryAudit(PrimaryAuditViewModel model)
        {
            return BizFactory.Merchant.PrimaryAudit(this.CurrentUserId, model.Operate, model.Merchant, model.EstimateInsuranceCompanyIds, model.MerchantPosMachine, model.BankCard, model.BizProcessesAudit);
        }


        public JsonResult GetSeniorAuditList(MerchantSearchCondition condition)
        {
            var waitAuditCount = (from h in CurrentDb.BizProcessesAudit where (h.AduitType == Enumeration.BizProcessesAuditType.MerchantAudit) && h.Status == (int)Enumeration.MerchantAuditStatus.WaitSeniorAudit select h.Id).Count();
            var inAuditCount = (from h in CurrentDb.BizProcessesAudit where (h.AduitType == Enumeration.BizProcessesAuditType.MerchantAudit) && h.Status == (int)Enumeration.MerchantAuditStatus.InSeniorAudit && h.Auditor == this.CurrentUserId select h.Id).Count();


            var query = (from b in CurrentDb.BizProcessesAudit
                         join m in CurrentDb.Merchant on
                        b.AduitReferenceId equals m.Id
                         where (b.AduitType == Enumeration.BizProcessesAuditType.MerchantAudit)
                         select new { b.Id, b.Status, b.Auditor, m.ClientCode, m.Type, m.RepairCapacity, m.Area, m.YYZZ_Name, m.FR_Name, m.ContactName, m.CreateTime });

            if (condition.AuditStatus == Enumeration.MerchantAuditStatus.WaitSeniorAudit)
            {
                query = query.Where(m => m.Status == (int)Enumeration.MerchantAuditStatus.WaitSeniorAudit);
            }
            else if (condition.AuditStatus == Enumeration.MerchantAuditStatus.InSeniorAudit)
            {
                query = query.Where(m => m.Status == (int)Enumeration.MerchantAuditStatus.InSeniorAudit && m.Auditor == this.CurrentUserId);
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
                    item.YYZZ_Name,
                    item.ContactName,
                    Type = item.Type.GetCnName(),
                    RepairCapacity = item.RepairCapacity.GetCnName(),
                    item.Area,
                    item.CreateTime,
                    AuditStatus = item.Status
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list, Status = new { waitAuditCount = waitAuditCount, inAuditCount = inAuditCount } };

            return Json(ResultType.Success, pageEntity, "");
        }

        [HttpPost]
        public JsonResult SeniorAudit(SeniorAuditViewModel model)
        {
            return BizFactory.Merchant.SeniorAudit(this.CurrentUserId, model.Operate, model.BizProcessesAudit);
        }

        public JsonResult GetWithdrawList(Models.Biz.Withdraw.WithdrawSearchCondition condition)
        {

            var query = (from m in CurrentDb.Withdraw
                         join u in CurrentDb.Merchant on m.MerchantId equals u.Id
                         where
                           m.UserId == condition.UserId
                         select new { m.Id, m.Sn, u.ClientCode, m.Amount, m.AmountByAfterFee, m.Fee, m.SettlementStartTime, m.Status, m.CreateTime });


            int pageIndex = condition.PageIndex;
            int pageSize = 10;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            int total = query.Count();


            List<object> olist = new List<object>();

            foreach (var item in query)
            {
                olist.Add(new
                {
                    item.Id,
                    item.Sn,
                    item.ClientCode,
                    Amount = item.Amount.ToPrice(),
                    AmountByAfterFee = item.AmountByAfterFee.ToPrice(),
                    Fee = item.Fee.ToPrice(),
                    Status = item.Status.GetCnName(),
                    item.SettlementStartTime
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = olist };

            return Json(ResultType.Success, pageEntity, "");
        }

        public JsonResult GetTransactionsList(Models.Biz.Withdraw.WithdrawSearchCondition condition)
        {

            var q = (from u in CurrentDb.Transactions
                     where u.UserId == condition.UserId

                     select new
                     {
                         u.Id,
                         u.Sn,
                         u.CreateTime,
                         u.ChangeAmount,
                         u.Balance,
                         u.Type
                     });

            int total = q.Count();

            int pageIndex = condition.PageIndex;
            int pageSize = condition.PageSize;
            q = q.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = q.ToList();

            List<object> rList = new List<object>();

            foreach (var item in list)
            {
                rList.Add(new
                {
                    Id = item.Id,
                    Sn = item.Sn,
                    CreateTime = item.CreateTime,
                    ChangeAmount = item.ChangeAmount.ToPrice(),
                    Balance = item.Balance.ToPrice(),
                    Type = item.Type.GetCnName()
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = rList };

            return Json(ResultType.Success, pageEntity, "");
        }

    }
}