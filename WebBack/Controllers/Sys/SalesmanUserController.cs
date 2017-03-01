﻿using Lumos.DAL.AuthorizeRelay;
using Lumos.Entity;
using Lumos.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBack.Models.Sys.SalesmanUser;

namespace WebBack.Controllers.Sys
{
    public class SalesmanUserController : WebBackController
    {
        #region 视图

        public ViewResult List()
        {
            return View();
        }

        public ViewResult SelectList()
        {
            return View();
        }

        public ViewResult Add()
        {
            AddViewModel model = new AddViewModel();
            return View();
        }

        public ViewResult Edit(int id)
        {
            EditViewModel model = new EditViewModel(id);
            return View(model);
        }


        #endregion

        public JsonResult GetList(SalesmanUserSearchCondition condition)
        {
            var list = (from u in CurrentDb.SysSalesmanUser
                        where (condition.UserName == null || u.UserName.Contains(condition.UserName)) &&
                        (condition.FullName == null || u.FullName.Contains(condition.FullName)) &&
                        u.IsDelete == false
                        select new { u.Id, u.UserName, u.FullName, u.Email, u.PhoneNumber, u.CreateTime, u.IsDelete });

            int total = list.Count();

            int pageIndex = condition.PageIndex;
            int pageSize = 10;
            list = list.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list };

            return Json(ResultType.Success, pageEntity, "");
        }





        public JsonResult GetSelectList(SalesmanUserSearchCondition condition)
        {
            var list = (from u in CurrentDb.SysSalesmanUser
                        where (condition.UserName == null || u.UserName.Contains(condition.UserName)) &&
                        (condition.FullName == null || u.FullName.Contains(condition.FullName)) &&
                        u.IsDelete == false
                        select new { u.Id, u.UserName, u.FullName, u.Email, u.PhoneNumber, u.CreateTime, u.IsDelete });

            int total = list.Count();

            int pageIndex = condition.PageIndex;
            int pageSize = 10;
            list = list.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list };

            return Json(ResultType.Success, pageEntity, "");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Add(AddViewModel model)
        {
            SysSalesmanUser user = new SysSalesmanUser();
            user.UserName = string.Format("YW{0}", model.SysSalesmanUser.UserName);
            user.FullName = model.SysSalesmanUser.FullName;
            user.PasswordHash = "888888";
            user.Email = model.SysSalesmanUser.Email;
            user.PhoneNumber = model.SysSalesmanUser.PhoneNumber;
            user.IsModifyDefaultPwd = false;
            user.IsDelete = false;
            user.Status = Enumeration.UserStatus.Normal;
            user.Creator = this.CurrentUserId;
            user.CreateTime = DateTime.Now;

            var identiy = new AspNetIdentiyAuthorizeRelay<SysSalesmanUser>();


            if (identiy.UserExists(user.UserName.Trim()))
                return Json(ResultType.Failure, WebBackOperateTipUtils.USER_EXISTS);


            bool r = identiy.CreateUser(this.CurrentUserId, user, null);
            if (!r)
                return Json(ResultType.Failure, WebBackOperateTipUtils.ADD_FAILURE);



            return Json(ResultType.Success, WebBackOperateTipUtils.ADD_SUCCESS);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(EditViewModel model)
        {

            var identiy = new AspNetIdentiyAuthorizeRelay<SysSalesmanUser>();
            SysSalesmanUser user = identiy.GetUser(model.SysSalesmanUser.Id);

            user.FullName = model.SysSalesmanUser.FullName;
            user.Email = model.SysSalesmanUser.Email;
            user.PhoneNumber = model.SysSalesmanUser.PhoneNumber;
            user.Mender = this.CurrentUserId;
            user.LastUpdateTime = DateTime.Now;


            bool r = identiy.UpdateUser(this.CurrentUserId, user, model.SysSalesmanUser.PasswordHash);
            if (!r)
            {
                return Json(ResultType.Failure, WebBackOperateTipUtils.UPDATE_FAILURE);
            }
            return Json(ResultType.Success, WebBackOperateTipUtils.UPDATE_SUCCESS);
        }

    }
}