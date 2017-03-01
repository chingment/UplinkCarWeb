﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using System.Reflection;
using log4net;
using Lumos.Common;
using System.Globalization;
using Lumos.Mvc;

namespace WebBack.Controllers
{
    #region 授权过滤器
    // 摘要:
    //     继承Authorize属性
    //     扩展Permission权限代码,用来控制用户是否拥有该类或方法的权限
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class WebBackAuthorizeAttribute : AuthorizeAttribute
    {
        public WebBackAuthorizeAttribute(params string[] permissions)
        {
            if (permissions != null)
            {
                if (permissions.Length > 0)
                {
                    this.Permissions = permissions;
                }
            }

        }

        /// <summary>
        /// 权限代码
        /// </summary>
        public string[] Permissions { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);


            bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true) || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true);
            if (skipAuthorization)
            {
                return;
            }


            #region 判断是否有该权限
            if (Permissions != null)
            {

                MessageBoxModel messageBox = new MessageBoxModel();
                messageBox.No = Guid.NewGuid().ToString();
                messageBox.Type = MessageBoxTip.Exception;
                messageBox.Title = "您没有权限访问,可能链接超时";

                if (!filterContext.HttpContext.Request.IsAuthenticated)
                {
                    messageBox.Content = "请重新<a href=\"javascript:void(0)\" onclick=\"window.top.location.href='" + WebBackConfig.GetLoginPage() + "'\">登录</a>后打开";
                }

                bool IsHasPermission = HttpContext.Current.User.Identity.IsInPermission(Permissions);

                if (!IsHasPermission)
                {
                    bool isAjaxRequest = filterContext.RequestContext.HttpContext.Request.IsAjaxRequest();
                    if (isAjaxRequest)
                    {
                        CustomJsonResult jsonResult = new CustomJsonResult(ResultType.Exception,ResultCode.Exception, messageBox.Title, messageBox);
                        jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                        filterContext.Result = jsonResult;
                        filterContext.Result.ExecuteResult(filterContext);
                        filterContext.HttpContext.Response.End();
                        return;
                    }
                    else
                    {
                        string masterName = "_LayoutHome";
                        if (filterContext.HttpContext.Request.QueryString["dialogtitle"] != null)
                        {
                            masterName = "_Layout";
                        }

                        filterContext.Result = new ViewResult { ViewName = "MessageBox", MasterName = masterName, ViewData = new ViewDataDictionary { Model = messageBox } };
                        return;
                    }
                }
            }
            #endregion
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);

            filterContext.Result = new RedirectResult(WebBackConfig.GetLoginPage());

        }
    }
    #endregion
}