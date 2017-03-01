using Lumos.Entity;
using Lumos.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lumos.BLL
{
    public class CarInsureCommissionRateProvider : BaseProvider
    {
        public CustomJsonResult Apply(int operater, CarInsureCommissionRate afterCommissionRate, string reason)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var beforeCommissionRate = CurrentDb.CarInsureCommissionRate.Where(m => m.Id == afterCommissionRate.Id).FirstOrDefault();

                if (beforeCommissionRate == null)
                {
                    return new CustomJsonResult(ResultType.Failure, "不存在该对象");
                }

                if (beforeCommissionRate.Commercial >= 10)
                {
                    return new CustomJsonResult(ResultType.Failure, "交强险比例过大，不能超过10%");
                }

                if (beforeCommissionRate.Compulsory >= 10)
                {
                    return new CustomJsonResult(ResultType.Failure, "商业险比例过大，不能超过10%");
                }

                var bizProcessesAuditing = CurrentDb.BizProcessesAudit.Where(m => m.AduitReferenceId == beforeCommissionRate.Id && m.AduitType == Enumeration.BizProcessesAuditType.CommissionRateAudit && m.Status < 5).FirstOrDefault();
                if(bizProcessesAuditing!=null)
                {
                    return new CustomJsonResult(ResultType.Failure, "已经在有1个在申请中，请等候");
                }

                CommissionRateAdjustModel adjustModel = new CommissionRateAdjustModel();
                adjustModel.Before = beforeCommissionRate;
                adjustModel.After = afterCommissionRate;



                var bizProcessesAudit = BizFactory.BizProcessesAudit.Add(operater, Enumeration.BizProcessesAuditType.CommissionRateAudit, beforeCommissionRate.Id, Enumeration.CommissionRateAuditStatus.WaitPrimaryAudit, "佣金修改申请", reason, adjustModel);


                BizFactory.BizProcessesAudit.ChangeAuditDetails(Enumeration.OperateType.Submit, Enumeration.CommissionRateAuditStep.Apply, bizProcessesAudit.Id, operater, reason, "提交申请调整费率，正等待初审", this.DateTime);

                CurrentDb.SaveChanges();
                ts.Complete();


                result = new CustomJsonResult(ResultType.Success, "申请成功");
            }

            return result;
        }

        public CustomJsonResult PrimaryAudit(int operater, Enumeration.OperateType operate, CarInsureCommissionRate commissionRate, BizProcessesAudit bizProcessesAudit)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                switch (operate)
                {
                    case Enumeration.OperateType.Save:

                        BizFactory.BizProcessesAudit.ChangeAuditDetailsAuditComments(operater, bizProcessesAudit.CurrentDetails.Id, bizProcessesAudit.CurrentDetails.AuditComments, null);

                        result = new CustomJsonResult(ResultType.Success, "保存成功");
                        break;
                    case Enumeration.OperateType.Submit:

                        BizFactory.BizProcessesAudit.ChangeAuditDetailsAuditComments(operater, bizProcessesAudit.CurrentDetails.Id, bizProcessesAudit.CurrentDetails.AuditComments, "初审完成,等待复审", this.DateTime);

                        BizFactory.BizProcessesAudit.ChangeCommissionRateAuditStatus(operater, bizProcessesAudit.CurrentDetails.BizProcessesAuditId, Enumeration.CommissionRateAuditStatus.WaitSeniorAudit,null);

                        result = new CustomJsonResult(ResultType.Success, "提交成功");
                        break;
                }


                CurrentDb.SaveChanges();
                ts.Complete();

            }

            return result;

        }

        public CustomJsonResult SeniorAudit(int operater, Enumeration.OperateType operate, CarInsureCommissionRate commissionRate, BizProcessesAudit bizProcessesAudit)
        {
            CustomJsonResult result = new CustomJsonResult();

            var l_bizProcessesAudit = CurrentDb.BizProcessesAudit.Where(m => m.Id == bizProcessesAudit.CurrentDetails.BizProcessesAuditId).FirstOrDefault();

            var carInsureCommissionRate = CurrentDb.CarInsureCommissionRate.Where(m => m.Id == l_bizProcessesAudit.AduitReferenceId).FirstOrDefault();

            var carInsuranceCompany = CurrentDb.CarInsuranceCompany.Where(m => m.InsuranceCompanyId == carInsureCommissionRate.ReferenceId).FirstOrDefault();

            using (TransactionScope ts = new TransactionScope())
            {
                switch (operate)
                {
                    case Enumeration.OperateType.Save:

                        BizFactory.BizProcessesAudit.ChangeAuditDetailsAuditComments(operater, bizProcessesAudit.CurrentDetails.Id, bizProcessesAudit.CurrentDetails.AuditComments, null);

                        result = new CustomJsonResult(ResultType.Success, "保存成功");
                        break;
                    case Enumeration.OperateType.Refuse:


                        BizFactory.BizProcessesAudit.ChangeAuditDetailsAuditComments(operater, bizProcessesAudit.CurrentDetails.Id, bizProcessesAudit.CurrentDetails.AuditComments, "复审完成，审核拒绝", this.DateTime);

                        BizFactory.BizProcessesAudit.ChangeCommissionRateAuditStatus(operater, bizProcessesAudit.CurrentDetails.BizProcessesAuditId, Enumeration.CommissionRateAuditStatus.SeniorAuditRefuse, null, this.DateTime);

                        result = new CustomJsonResult(ResultType.Success, "拒绝成功");
                        break;
                    case Enumeration.OperateType.Pass:

                        var commissionRateAdjustModel = Newtonsoft.Json.JsonConvert.DeserializeObject<CommissionRateAdjustModel>(l_bizProcessesAudit.JsonData);

                        var l_commissionRate = CurrentDb.CarInsureCommissionRate.Where(m => m.Id == l_bizProcessesAudit.AduitReferenceId).FirstOrDefault();
                        l_commissionRate.Commercial = commissionRateAdjustModel.After.Commercial;
                        l_commissionRate.Compulsory = commissionRateAdjustModel.After.Compulsory;
                        l_commissionRate.LastUpdateTime = this.DateTime;
                        l_commissionRate.Mender = operater;

                        if(carInsuranceCompany.Status==Enumeration.CarInsuranceCompanyStatus.Audit)
                        {
                            carInsuranceCompany.Status = Enumeration.CarInsuranceCompanyStatus.Normal;
                            carInsuranceCompany.LastUpdateTime = this.DateTime;
                            carInsuranceCompany.Mender = operater;
                        }

                        BizFactory.BizProcessesAudit.ChangeAuditDetailsAuditComments(operater, bizProcessesAudit.CurrentDetails.Id, bizProcessesAudit.CurrentDetails.AuditComments, "复审完成，审核通过", this.DateTime);

                        BizFactory.BizProcessesAudit.ChangeCommissionRateAuditStatus(operater, bizProcessesAudit.CurrentDetails.BizProcessesAuditId, Enumeration.CommissionRateAuditStatus.SeniorAuditPass,null, this.DateTime);

                        result = new CustomJsonResult(ResultType.Success, "审核通过");
                        break;
                }


                CurrentDb.SaveChanges();
                ts.Complete();

            }

            return result;

        }


    }
}
