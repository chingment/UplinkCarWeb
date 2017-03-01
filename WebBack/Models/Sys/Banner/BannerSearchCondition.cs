﻿using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBack.Models.Sys.Banner
{
    public class BannerSearchCondition:SearchCondition
    {
        public string Title { get; set; }

        public Enumeration.BannerType Type{ get; set; }
    }
}