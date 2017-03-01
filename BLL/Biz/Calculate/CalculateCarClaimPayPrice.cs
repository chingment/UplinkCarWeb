using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL
{
    public class CalculateCarClaimPayPrice
    {
        private string _version = "2017.02.20";

        private decimal _rate = 0.15m;

        private decimal _merchantCommission = 0;

        private decimal _uplinkCommission = 0;

        private decimal _payPrice = 0;


        public decimal PayPrice
        {
            get { return _payPrice; }
        }


        public decimal MerchantCommission
        {
            get
            {
                return _merchantCommission;
            }
        }


        public decimal UplinkCommission
        {

            get
            {
                return _uplinkCommission;
            }
        }



        public CalculateCarClaimPayPrice(decimal workingHoursPrice, decimal accessoriesPrice)
        {

            _payPrice = (workingHoursPrice + accessoriesPrice) * _rate;


            _merchantCommission = _payPrice * 0.85m;

            _uplinkCommission = _payPrice * 0.15m;
        }

        public string Version
        {
            get
            {
                return _version;
            }

        }
    }
}
