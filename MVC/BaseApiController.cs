﻿using Lumos.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace Lumos.Mvc
{
    public abstract class BaseApiController : ApiController
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
    }
}
