using Lumos.BLL;
using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBack.Models.Biz.CarInsureCommissionRate
{
    public class PrimaryAuditViewModel : BaseViewModel
    {
        public CommissionRateAdjustModel _commissionRateAdjustModel = new CommissionRateAdjustModel();

        public Lumos.Entity.CarInsureCommissionRate _commissionRate = new Lumos.Entity.CarInsureCommissionRate();

        private Lumos.Entity.BizProcessesAudit _bizProcessesAudit = new Lumos.Entity.BizProcessesAudit();

        public Lumos.Entity.CommissionRateAdjustModel CommissionRateAdjustModel
        {
            get
            {
                return _commissionRateAdjustModel;
            }
            set
            {
                _commissionRateAdjustModel = value;
            }
        }

        public Lumos.Entity.CarInsureCommissionRate CommissionRate
        {
            get
            {
                return _commissionRate;
            }
            set
            {
                _commissionRate = value;
            }
        }


        public Lumos.Entity.BizProcessesAudit BizProcessesAudit
        {
            get
            {
                return _bizProcessesAudit;
            }
            set
            {
                _bizProcessesAudit = value;
            }
        }

        public PrimaryAuditViewModel()
        {

        }

        public PrimaryAuditViewModel(int id)
        {
            var bizProcessesAudit = BizFactory.BizProcessesAudit.ChangeCommissionRateAuditStatus(this.Operater, id, Enumeration.CommissionRateAuditStatus.InPrimaryAudit,"正在初审中");

            if (bizProcessesAudit != null)
            {
                _bizProcessesAudit = bizProcessesAudit;

                var commissionRate = CurrentDb.CarInsureCommissionRate.Where(m => m.Id == bizProcessesAudit.AduitReferenceId).FirstOrDefault();
                if (commissionRate != null)
                {
                    _commissionRate = commissionRate;
                }

                _commissionRateAdjustModel = Newtonsoft.Json.JsonConvert.DeserializeObject<CommissionRateAdjustModel>(bizProcessesAudit.JsonData);
            }

        }
    }
}