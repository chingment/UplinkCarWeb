using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBack.Models.Biz.CarInsureCommissionRate
{
    public class CarInsureCommissionRateCondition : SearchCondition
    {
        public Enumeration.CommissionRateAuditStatus AuditStatus { get; set; }
    }
}