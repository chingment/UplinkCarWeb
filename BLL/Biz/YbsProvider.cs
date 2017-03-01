using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL
{
    public class YbsProvider
    {
        //todo 正式环境要配置
        public YbsMerchantModel GetCarClaimMerchantInfo()
        {
            YbsMerchantModel model = new YbsMerchantModel();
            model.merchant_id = "861440360120020";
            model.ybs_mer_code = "000574";
            return model;
        }

        //todo 正式环境要配置
        public YbsMerchantModel GetDepositRentMerchantInfo()
        {
            YbsMerchantModel model = new YbsMerchantModel();
            model.merchant_id = "861440360120020";
            model.ybs_mer_code = "000573";
            return model;
        }

        public YbsMerchantModel GetCarInsureMerchantInfo(int insuranceCompanyId, string merchant_id = null, string ybs_mer_code = null)
        {
            YbsMerchantModel model = new YbsMerchantModel();
            model.merchant_id = merchant_id;
            model.ybs_mer_code = ybs_mer_code;
            return model;
        }
    }
}
