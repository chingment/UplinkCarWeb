using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBack.Models.Biz.CarInsureCommissionRate
{


    public class ApplyViewModel : BaseViewModel
    {
        private List<Lumos.Entity.CarInsureCommissionRate> _commissionObject = new List<Lumos.Entity.CarInsureCommissionRate>();

        private Lumos.Entity.CarInsureCommissionRate _commissionRate = new Lumos.Entity.CarInsureCommissionRate();

        private string _reason;

        public string Reason
        {
            get
            {
                return _reason;
            }
            set
            {
                _reason = value;
            }
        }

        public List<Lumos.Entity.CarInsureCommissionRate> CommissionObject
        {
            get
            {
                return _commissionObject;
            }
            set
            {
                _commissionObject = value;
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

        public ApplyViewModel()
        {
            var commissionObject = CurrentDb.CarInsureCommissionRate.ToList();
            if (commissionObject != null)
            {
                _commissionObject = commissionObject;
            }
        }
    }
}