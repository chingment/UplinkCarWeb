using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBack.Models.Biz.Report
{
    public class ReportModel
    {
        public Enumeration.OperateType Operate { get; set; }

        public string ClientCode { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }


        public string TableHtml { get; set; }
    }
}