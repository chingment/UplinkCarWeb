﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBack.Models.Biz.PosMachine;
using Lumos.Common;
using Lumos.Entity;
using Lumos.Mvc;
using Lumos.BLL;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Data;
using System.Transactions;

namespace WebBack.Controllers.Biz
{
    public class PosMachineController : WebBackController
    {
        //
        // GET: /PosMachine/
        public ViewResult List()
        {
            return View();
        }

        public ViewResult ReturnList()
        {
            return View();
        }



        public ViewResult MerchantPosMachineList()
        {
            return View();
        }

        public ViewResult Return()
        {
            return View();
        }

        public ViewResult Add()
        {
            return View();
        }

        public ViewResult ExportAdd()
        {
            return View();
        }

        public ViewResult ExportReturn()
        {
            return View();
        }

        public ViewResult Edit(int id)
        {
            EditViewModel model = new EditViewModel(id);
            return View(model);
        }

        public JsonResult GetList(PosMachineSearchCondition condition)
        {


            string fuselageNumber = condition.FuselageNumber.ToSearchString();
            string deviceId = condition.DeviceId.ToSearchString();
            var list = (from p in CurrentDb.PosMachine
                        where (fuselageNumber.Length == 0 || p.FuselageNumber.Contains(fuselageNumber)) &&
                                (deviceId.Length == 0 || p.DeviceId.Contains(deviceId))
                        select new { p.Id, p.DeviceId, p.FuselageNumber, p.TerminalNumber, IsUse = (p.IsUse == true ? "是" : "否"), p.CreateTime, p.Version });

            int total = list.Count();

            int pageIndex = condition.PageIndex;
            int pageSize = 10;
            list = list.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list };

            return Json(ResultType.Success, pageEntity, "");
        }


        public JsonResult GetReturnList(PosMachineSearchCondition condition)
        {


            string fuselageNumber = condition.FuselageNumber.ToSearchString();
            string deviceId = condition.DeviceId.ToSearchString();
            var list = (from mp in CurrentDb.MerchantPosMachine
                        join p in CurrentDb.PosMachine on mp.PosMachineId equals p.Id
                        join m in CurrentDb.Merchant on mp.MerchantId equals m.Id
                        where (fuselageNumber.Length == 0 || p.FuselageNumber.Contains(fuselageNumber)) &&
                                (deviceId.Length == 0 || p.DeviceId.Contains(deviceId)) &&
                                mp.Status == Enumeration.MerchantPosMachineStatus.Return
                        select new { m.ClientCode, m.YYZZ_Name, p.Id, p.DeviceId, mp.Deposit, mp.ReturnDeposit, mp.DepositPayTime, mp.ReturnTime, });

            int total = list.Count();

            int pageIndex = condition.PageIndex;
            int pageSize = 10;
            list = list.OrderByDescending(r => r.ReturnTime).Skip(pageSize * (pageIndex)).Take(pageSize);


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list };

            return Json(ResultType.Success, pageEntity, "");
        }

        public JsonResult GetMerchantPosMachineList(PosMachineSearchCondition condition)
        {


            string fuselageNumber = condition.FuselageNumber.ToSearchString();
            string deviceId = condition.DeviceId.ToSearchString();
            var query = (from mp in CurrentDb.MerchantPosMachine
                         join p in CurrentDb.PosMachine on mp.PosMachineId equals p.Id
                         join m in CurrentDb.Merchant on mp.MerchantId equals m.Id
                         where (fuselageNumber.Length == 0 || p.FuselageNumber.Contains(fuselageNumber)) &&
                                 (deviceId.Length == 0 || p.DeviceId.Contains(deviceId)) &&
                                (mp.Status == Enumeration.MerchantPosMachineStatus.Normal ||
                                  mp.Status == Enumeration.MerchantPosMachineStatus.NoActive ||
                                  mp.Status == Enumeration.MerchantPosMachineStatus.Rentdue
                                  )

                         select new { m.ClientCode, m.YYZZ_Name, m.ContactName, m.ContactPhoneNumber, mp.Deposit, mp.DepositPayTime, p.DeviceId, p.FuselageNumber, p.TerminalNumber, p.Version, m.CreateTime, mp.Status });

            int total = query.Count();

            int pageIndex = condition.PageIndex;
            int pageSize = 10;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);


            List<object> list = new List<object>();

            foreach (var item in query)
            {

                list.Add(new
                {
                    item.ClientCode,
                    item.YYZZ_Name,
                    item.ContactName,
                    item.ContactPhoneNumber,
                    Deposit = item.Deposit.ToPrice(),
                    item.DepositPayTime,
                    item.DeviceId,
                    item.FuselageNumber,
                    item.TerminalNumber,
                    item.Version,
                    item.CreateTime,
                    StatusName = item.Status.GetCnName(),
                });


            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list };

            return Json(ResultType.Success, pageEntity, "");
        }


        [HttpPost]
        public JsonResult Add(AddViewModel model)
        {
            return BizFactory.PosMachine.Add(this.CurrentUserId, model.PosMachine);
        }

        [HttpPost]
        public JsonResult Edit(AddViewModel model)
        {
            return BizFactory.PosMachine.Edit(this.CurrentUserId, model.PosMachine);
        }

        [HttpPost]
        public JsonResult ExportAdd(EditViewModel model)
        {
            CustomJsonResult r = new CustomJsonResult();
            r.ContentType = "text/html";

            try
            {
                HttpPostedFileBase file_upload = Request.Files[0];

                if (file_upload == null)
                    return Json("text/html", ResultType.Failure, "找不到上传的对象");

                if (file_upload.ContentLength == 0)
                    return Json("text/html", ResultType.Failure, "文件内容为空,请重新选择");

                System.IO.FileInfo file = new System.IO.FileInfo(file_upload.FileName);
                string ext = file.Extension.ToLower();

                if (ext != ".xls")
                {
                    return Json("text/html", ResultType.Failure, "上传的文件不是excel格式(xls)");
                }

                HSSFWorkbook workbook = new HSSFWorkbook(file_upload.InputStream);

                ISheet sheet = workbook.GetSheetAt(0);

                ExportCheckErrorPoint exportCheckErrorPoint = new ExportCheckErrorPoint();

                IRow rowTitle = sheet.GetRow(0);

                exportCheckErrorPoint.CheckCellTitle(rowTitle.GetCell(0), "设备ID");
                exportCheckErrorPoint.CheckCellTitle(rowTitle.GetCell(1), "机身号");
                exportCheckErrorPoint.CheckCellTitle(rowTitle.GetCell(2), "终端号");
                exportCheckErrorPoint.CheckCellTitle(rowTitle.GetCell(3), "版本号");
                exportCheckErrorPoint.CheckCellTitle(rowTitle.GetCell(4), "押金");
                exportCheckErrorPoint.CheckCellTitle(rowTitle.GetCell(5), "租金");


                if (exportCheckErrorPoint.TitleHasError)
                {
                    return Json("text/html", ResultType.Failure, "上传的文件模板错误，请点击下载的文件模板");
                }


                int rowCount = sheet.LastRowNum + 1;

                if (rowCount == 1)
                {
                    return Json("text/html", ResultType.Failure, "上传的文件数据为空，请检查后再次上传");
                }


                List<PosMachine> posMachines = new List<PosMachine>();
                for (int i = 1; i < rowCount; i++)
                {
                    IRow row = sheet.GetRow(i);

                    string deviceId = row.GetCell(0).ToString();
                    string fuselageNumber = row.GetCell(1) == null ? "" : row.GetCell(1).ToString();
                    string terminalNumber = row.GetCell(2) == null ? "" : row.GetCell(2).ToString();
                    string version = row.GetCell(3) == null ? "" : row.GetCell(3).ToString();
                    string deposit = row.GetCell(4) == null ? "" : row.GetCell(4).ToString();
                    string rent = row.GetCell(5) == null ? "" : row.GetCell(5).ToString();

                    if (string.IsNullOrEmpty(deviceId))
                    {
                        return Json("text/html", ResultType.Failure, "检查到有为空的设备ID，请完善后再次上传");
                    }

                    if (string.IsNullOrEmpty(deviceId) || deviceId.Length > 100)
                    {
                        exportCheckErrorPoint.AddPoint(deviceId, "设备ID不能为空，且不能超过100个字符");
                    }

                    if (string.IsNullOrEmpty(fuselageNumber) || fuselageNumber.Length > 100)
                    {
                        exportCheckErrorPoint.AddPoint(deviceId, "机身号不能为空，且不能超过100个字符");
                    }

                    if (string.IsNullOrEmpty(terminalNumber) || terminalNumber.Length > 100)
                    {
                        exportCheckErrorPoint.AddPoint(deviceId, "终端号不能为空，且不能超过100个字符");
                    }

                    if (string.IsNullOrEmpty(version) || version.Length > 100)
                    {
                        exportCheckErrorPoint.AddPoint(deviceId, "版本号不能为空，且不能超过100个字符");
                    }

                    if (CommonUtils.IsDecimal(deposit))
                    {
                        exportCheckErrorPoint.AddPoint(deviceId, "押金必须数字格式,且整数位最多16位,小数位最多2位");
                    }

                    if (CommonUtils.IsDecimal(rent))
                    {
                        exportCheckErrorPoint.AddPoint(deviceId, "租金必须数字格式,且整数位最多16位,小数位最多2位");
                    }

                    PosMachine posMachine = new PosMachine();
                    posMachine.DeviceId = deviceId;
                    posMachine.FuselageNumber = fuselageNumber;
                    posMachine.TerminalNumber = terminalNumber;
                    posMachine.Version = version;
                    posMachines.Add(posMachine);
                }


                if (exportCheckErrorPoint.ErrorPoint.Count > 0)
                {
                    return Json("text/html", ResultType.Failure, exportCheckErrorPoint.ErrorPoint, "更新数据失败，检查到无效的数据");
                }


                using (TransactionScope ts = new TransactionScope())
                {
                    foreach (var posMachine in posMachines)
                    {
                        var old = CurrentDb.PosMachine.Where(m => m.DeviceId == posMachine.DeviceId).FirstOrDefault();
                        if (old != null)
                        {
                            exportCheckErrorPoint.AddPoint(old.DeviceId, "设备ID号:" + posMachine.DeviceId + "，已经存在");

                        }
                    }


                    if (exportCheckErrorPoint.ErrorPoint.Count > 0)
                    {
                        return Json("text/html", ResultType.Failure, exportCheckErrorPoint.ErrorPoint, "更新数据失败，检查到无效的数据");
                    }

                    DateTime dateNow = DateTime.Now;
                    foreach (var posMachine in posMachines)
                    {
                        posMachine.CreateTime = dateNow;
                        posMachine.Creator = this.CurrentUserId;
                        CurrentDb.PosMachine.Add(posMachine);
                    }

                    CurrentDb.SaveChanges();
                    ts.Complete();
                }


                return Json("text/html", ResultType.Success, "上传成功");
            }
            catch (Exception ex)
            {
                Log.Error("导入POS机信息", ex);
                return Json("text/html", ResultType.Exception, "上传失败，系统出现异常");
            }
        }


    }
}