﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBack.Models.Biz.ApplyPos
{
    public class ApplyPosSearchCondition : SearchCondition
    {
        public string DeviceId { get; set; }

        public string UserName { get; set; }
    }
}