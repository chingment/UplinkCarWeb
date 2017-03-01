using Lumos.BLL;
using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBack.Models.Biz.CarInsureCommissionRate
{
    public class DetailsViewModel:BaseViewModel
    {
        private Lumos.Entity.CarInsureCommissionRate _commissionRate = new Lumos.Entity.CarInsureCommissionRate();

        private List<Lumos.Entity.BizProcessesAuditDetails> _bizProcessesAuditDetails = new List<Lumos.Entity.BizProcessesAuditDetails>();

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

        public List<Lumos.Entity.BizProcessesAuditDetails> BizProcessesAuditDetails
        {
            get
            {
                return _bizProcessesAuditDetails;
            }
            set
            {
                _bizProcessesAuditDetails = value;
            }
        }

        public DetailsViewModel()
        {

        }

        public DetailsViewModel(int id)
        {
            var commissionRate = CurrentDb.CarInsureCommissionRate.Where(m => m.Id == id).FirstOrDefault();
            if (commissionRate != null)
            {
                _commissionRate = commissionRate;

                _bizProcessesAuditDetails = BizFactory.BizProcessesAudit.GetDetails(Enumeration.BizProcessesAuditType.CommissionRateAudit, id);
            }
        }
    }
}