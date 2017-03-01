﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lumos.Common;
using Lumos.Entity;
using Lumos.Mvc;
using Lumos.BLL;
using WebBack.Models;
using System.Collections.Specialized;
using WebBack.Models.Biz.CarInsuranceCompany;

namespace WebBack.Controllers.Biz
{
    public class CarInsuranceCompanyController : WebBackController
    {


        public ViewResult List()
        {
            return View();
        }

        public ViewResult InsuranceCompanyList()
        {
            return View();
        }

        public ViewResult Add()
        {
            return View();
        }


        [HttpPost]
        public JsonResult Add(AddViewModel model)
        {
            CustomJsonResult reuslt = new CustomJsonResult();


            reuslt = BizFactory.CarInsuranceCompany.Add(this.CurrentUserId, model.InsuranceCompanyId, model.InsuranceCompanyName, model.InsuranceCompanyImgUrl, model.CommercialRate, model.CompulsoryRate);

            return reuslt;
        }

        [HttpPost]
        public JsonResult Disable(int id)
        {
            CustomJsonResult reuslt = new CustomJsonResult();
            reuslt = BizFactory.CarInsuranceCompany.Disable(this.CurrentUserId, id);

            return reuslt;
        }


        public JsonResult GetList(SearchCondition condition)
        {
            string name = condition.Name.ToSearchString();
            var query = (from c in CurrentDb.CarInsuranceCompany
                         join i in CurrentDb.InsuranceCompany on c.InsuranceCompanyId equals i.Id into tmp0
                         from tt0 in tmp0.DefaultIfEmpty()
                         where
                                 (name.Length == 0 || tt0.Name.Contains(name))

                         select new { c.Id, c.InsuranceCompanyId, c.Status, c.InsuranceCompanyImgUrl, tt0.Name, c.CreateTime });

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
                    item.InsuranceCompanyId,
                    item.Name,
                    item.InsuranceCompanyImgUrl,
                    item.CreateTime,
                    Status= item.Status,
                    StatusName = item.Status.GetCnName()
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list };

            return Json(ResultType.Success, pageEntity, "");
        }


        /// <summary>
        /// 可选择的保险公司列表
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public JsonResult GetInsuranceCompanyList(SearchCondition condition)
        {
            string name = condition.Name.ToSearchString();
            var query = (from i in CurrentDb.InsuranceCompany
                         where
                                 (name.Length == 0 || i.Name.Contains(name)) && !(CurrentDb.CarInsuranceCompany.Any(m => m.InsuranceCompanyId == i.Id))

                         select new { i.Id, i.ImgUrl, i.Name, i.LastUpdateTime, i.CreateTime });

            int total = query.Count();

            int pageIndex = condition.PageIndex;
            int pageSize = 5;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            List<object> list = new List<object>();

            foreach (var item in query)
            {
                list.Add(new
                {
                    item.Id,
                    item.ImgUrl,
                    item.Name,
                    item.LastUpdateTime,
                    item.CreateTime

                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list };

            return Json(ResultType.Success, pageEntity, "");
        }

    }
}