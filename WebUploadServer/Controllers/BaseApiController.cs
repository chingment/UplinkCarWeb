﻿using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Lumos.Mvc;

namespace WebUploadImageServer.Controllers
{

    public class BaseApiController : ApiController
    {
        #region JsonResult 扩展

        protected JsonResult Json(string contenttype, ResultType type, object content, string message, params JsonConverter[] converters)
        {
            return new CustomJsonResult(contenttype, type, message, content);
        }

        protected JsonResult Json(ResultType type, object content, string message, params JsonConverter[] converters)
        {
            return new CustomJsonResult(null, type, message, content, converters);
        }

        protected internal JsonResult Json(ResultType type)
        {
            return Json(type, null, null);
        }

        protected internal JsonResult Json(ResultType type, string message)
        {
            return Json(type, null, message);
        }

        protected internal JsonResult Json(string contenttype, ResultType type, string message)
        {
            return Json(contenttype, type, null, message);
        }

        protected internal JsonResult Json(ResultType type, object content)
        {
            return Json(type, content, null);
        }
        #endregion



        protected ILog Log
        {
            get
            {
                return LogManager.GetLogger(this.GetType());
            }
        }

        protected static ILog GetLog(Type t)
        {
            return LogManager.GetLogger(t);
        }

        protected static ILog GetLog()
        {
            //return LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType); //有问题，子类调用，返回的还是父类的logger

            var trace = new System.Diagnostics.StackTrace();
            Type baseType = typeof(BaseController);
            for (int i = 0; i < trace.FrameCount; i++)
            {
                var frame = trace.GetFrame(i);
                var method = frame.GetMethod();
                var type = method.DeclaringType;
                if (type.IsSubclassOf(baseType)) return GetLog(type);
            }
            return LogManager.GetLogger(baseType);
        }

        protected void SetTrackID()
        {
            if (ThreadContext.Properties["trackid"] == null)
                ThreadContext.Properties["trackid"] = DateTime.Now.TimeOfDay.TotalMilliseconds.ToString("00000000"); //Guid.NewGuid().ToString("N");
        }

        public BaseApiController()
        {

        }

    }

}