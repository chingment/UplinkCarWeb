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
using Lumos;

namespace WebBack.Controllers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class WebBackStatisticsTrackerAttribute : ActionFilterAttribute, IExceptionFilter
    {
        private readonly string Key = "_thisOnActionMonitorLog_";
        ILog log = log4net.LogManager.GetLogger(CommonSetting.LoggerStatisticsTracker);

        public int GetUserId()
        {
            if(HttpContext.Current==null)
            {
                return 0;
            }

            if(HttpContext.Current.User==null)
            {
                return 0;
            }


            if(HttpContext.Current.User.Identity==null)
            {
                return 0;
            }

            int userId=HttpContext.Current.User.Identity.GetUserId<int>();
            return userId;
        }

        #region Action时间监控
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            MonitorLog MonLog = new MonitorLog();
            MonLog.RequestTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffff", DateTimeFormatInfo.InvariantInfo));
            MonLog.ControllerName = filterContext.RouteData.Values["controller"] as string;
            MonLog.ActionName = filterContext.RouteData.Values["action"] as string;
            MonLog.UserId = GetUserId();
            filterContext.Controller.ViewData[Key] = MonLog;
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            MonitorLog MonLog = filterContext.Controller.ViewData[Key] as MonitorLog;
            MonLog.ResponseTime = DateTime.Now;
            MonLog.FormCollections = filterContext.HttpContext.Request.Form;//form表单提交的数据
            MonLog.QueryCollections = filterContext.HttpContext.Request.QueryString;//Url 参数
            MonLog.UserId = GetUserId();
            log.Info(MonLog.GetRequestInfo());
        }
        #endregion

        #region View 视图生成时间监控
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            MonitorLog MonLog = filterContext.Controller.ViewData[Key] as MonitorLog;
            MonLog.ResponseTime = DateTime.Now;
        }
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            MonitorLog MonLog = filterContext.Controller.ViewData[Key] as MonitorLog;
            MonLog.ResponseTime = DateTime.Now;
            log.Info(MonLog.GetResponseInfo());
            filterContext.Controller.ViewData.Remove(Key);
        }
        #endregion


        #region 错误日志
        public void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                string ControllerName = string.Format("{0}Controller", filterContext.RouteData.Values["controller"] as string);
                string ActionName = filterContext.RouteData.Values["action"] as string;
                string ErrorMsg = string.Format("在执行 controller[{0}] 的 action[{1}] 时产生异常", ControllerName, ActionName);
                log.Error(ErrorMsg, filterContext.Exception);
            }
        }
        #endregion
    }

}