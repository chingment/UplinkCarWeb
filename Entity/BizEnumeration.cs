﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.Entity
{
    /// <summary>
    /// 业务的枚举
    /// </summary>
    public partial class Enumeration
    {

        public enum MerchantPosMachineStatus
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("正常")]
            Normal = 1,
            [Remark("未激活")]
            NoActive = 2,
            [Remark("租金到期")]
            Rentdue = 3,
            [Remark("已注销")]
            Return = 4,
            [Remark("账号与设备好不匹配")]
            NotMatch = 5
        }

        public enum ExtendedAppType
        {
            Unknow = 0,
            CarService = 1,
            ThirdPartyApp = 2
        }

        public enum ExtendedAppAuditStatus
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("等待初审")]
            WaitAudit = 1,
            [Remark("初审中")]
            InAudit = 2,
            [Remark("等待复审")]
            WaitReview = 3,
            [Remark("复审中")]
            InReview = 4,
            [Remark("复审通过")]
            ReviewPass = 5,
            [Remark("复审驳回")]
            ReviewReject = 6,
            [Remark("复审拒绝")]
            ReviewRefuse = 7
        }

        public enum CarInsureOfferDealtStatus
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("等待报价")]
            WaitOffer = 1,
            [Remark("报价中")]
            InOffer = 2,
            [Remark("客户跟进")]
            ClientFllow = 3,
            [Remark("后台取消订单")]
            StaffCancle = 4,
            [Remark("客户取消订单")]
            ClientCancle = 5,
            [Remark("完成报价")]
            Complete = 6
        }

        public enum CarInsureOfferDealtStep
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("提交订单")]
            Submit = 1,
            [Remark("报价")]
            Offer = 2,
            [Remark("跟进订单")]
            Fllow = 3
        }

        public enum CarClaimDealtStatus
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("等待核实")]
            WaitVerifyOrder = 1,
            [Remark("核实需求")]
            InVerifyOrder = 2,
            [Remark("跟进待上传定损单")]
            FllowUploadEstimateListImg = 3,
            [Remark("核实金额")]
            WaitVerifyAmount = 4,
            [Remark("核实金额")]
            InVerifyAmount = 5,
            [Remark("后台取消订单")]
            StaffCancle = 6,
            [Remark("客户取消订单")]
            ClientCancle = 7,
            [Remark("完成")]
            Complete = 8
        }

        public enum CarClaimDealtStep
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("提交订单")]
            Submit = 1,
            [Remark("核实需求")]
            VerifyOrder = 2,
            [Remark("上传定损单")]
            UploadEstimateListImg = 3,
            [Remark("核实金额")]
            VerifyAmount = 4

        }


        public enum MerchantAuditStatus
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("等待初审")]
            WaitPrimaryAudit = 1,
            [Remark("初审中")]
            InPrimaryAudit = 2,
            [Remark("等待复审")]
            WaitSeniorAudit = 3,
            [Remark("复审中")]
            InSeniorAudit = 4,
            [Remark("复审通过")]
            SeniorAuditPass = 5,
            [Remark("复审驳回")]
            SeniorAuditReject = 6
        }

        public enum MerchantAuditStep
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("初审")]
            PrimaryAudit = 1,
            [Remark("复审")]
            SeniorAudit = 2
        }


        public enum ExtendedAppAuditStep
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("初审")]
            PrimaryAudit = 1,
            [Remark("复审")]
            SeniorAudit = 2,
        }

        public enum ExtendedAppApplyType
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("上架")]
            On = 1,
            [Remark("下架")]
            Off = 2,
            [Remark("恢复")]
            Recovery = 3
        }

        public enum ExtendedAppStatus
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("上架审核中")]
            AuditOn = 1,
            [Remark("上架审核通过")]
            AuditOnPass = 2,
            [Remark("上架审核拒绝")]
            AuditOnRefuse = 3,
            [Remark("下架审核中")]
            AuditOff = 4,
            [Remark("下架审核通过")]
            AuditOffPass = 5,
            [Remark("下架审核拒绝")]
            AuditOffRefuse = 6,
            [Remark("恢复审核中")]
            AuditRecovery = 7,
            [Remark("恢复审核通过")]
            AuditRecoveryPass = 8,
            [Remark("恢复审核拒绝")]
            AuditRecoveryRefuse = 9
        }

        public enum CarInsurePlanStatus
        {
            Unknow = 0,
            CarService = 1
        }

        public enum CarKindInputType
        {
            Unknow = 0,
            None = 1,
            Text = 2,
            DropDownList = 3
        }
        public enum CarKindType
        {
            Unknow = 0,
            Compulsory = 1,
            Commercial = 2,
            AdditionalRisk = 3
        }

        public enum ProductType
        {
            Unknow = 0,
            [Remark("商品")]
            Goods = 1,
            [Remark("汽车用品")]
            GoodsForCar = 101,
            [Remark("机油")]
            GoodsForCarForMachineOil = 1011,
            [Remark("轮胎")]
            GoodsForCarForTyre = 1012,
            [Remark("座垫")]
            GoodsForCarForCushion = 1013,
            [Remark("香水")]
            GoodsForCarForPerfume = 1014,
            [Remark("空气净化")]
            GoodsForCarForAirPurge = 1015,
            [Remark("方向盘套")]
            GoodsForCarForSteeringWheelCover = 1016,
            [Remark("座套")]
            GoodsForCarForSeatCover = 1017,
            [Remark("头枕")]
            GoodsForCarForHeadPillow = 1018,
            [Remark("保险")]
            Insure = 2,
            [Remark("车险")]
            InsureForCar = 201,
            [Remark("车险投保")]
            InsureForCarForInsure = 2011,
            [Remark("车险续保")]
            InsureForCarForRenewal = 2012,
            [Remark("车险理赔")]
            InsureForCarForClaim = 2013,
            [Remark("保险通")]
            InsureForPopular = 202,
            [Remark("POS机押金租金")]
            PosMachineDepositRent = 301,
            [Remark("POS机租金")]
            PosMachineRent = 302,

        }

        public enum ProductStatus
        {
            Unknow = 0,
            [Remark("上架")]
            OnLine = 1,
            [Remark("下架")]
            OffLine = 2
        }

        public enum DealtStatus
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("待处理")]
            Wait = 1,
            [Remark("处理中")]
            Handle = 2,
            [Remark("已处理")]
            Complete = 3
        }


        public enum OrderToCarClaimDealtStatus
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("定损中")]
            Wait = 1,
            [Remark("复核中")]
            Handle = 2,
            [Remark("待支付")]
            Complete = 3
        }

        public enum CarInsuranceClaimResult
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("派单")]
            Dispatch = 1,
            [Remark("撤销")]
            Cancel = 2,
        }

        public enum OrderStatus
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("已提交")]
            Submitted = 1,
            [Remark("跟进中")]
            Follow = 2,
            [Remark("待支付")]
            WaitPay = 3,
            [Remark("已完成")]
            Completed = 4,
            [Remark("已取消")]
            Cancled = 5
        }

        public enum OrderToCarInsureFollowStatus
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("待提交")]
            WaitSubmit = 1,
            [Remark("已提交")]
            Submitted = 2
        }

        public enum OrderToCarClaimFollowStatus
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("定损中")]
            WaitEstimate = 1,
            [Remark("待上传定损单")]
            WaitUploadEstimateList = 2,
            [Remark("等待核实定损金额")]
            VerifyEstimateAmount = 3,
            [Remark("等待支付佣金")]
            WaitPayCommission = 6,
            [Remark("支付完成")]
            PayedCommission = 7
        }


        public enum PayWay
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("POS")]
            POS = 1,
            [Remark("微信")]
            Wechat = 2,
            [Remark("支付宝")]
            Alipay = 3,
            [Remark("现金")]
            Cash = 3,
        }

        public enum BizProcessesAuditType
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("扩展应用上架申请")]
            ExtendedAppOn = 1,
            [Remark("扩展应用下架申请")]
            ExtendedAppOff = 2,
            [Remark("扩展应用恢复申请")]
            ExtendedAppRecovery = 3,
            [Remark("车险订单报价")]
            CarInsureOffer = 4,
            [Remark("车险理赔")]
            CarClaim = 5,
            [Remark("商户资料审核")]
            MerchantAudit = 6,
            [Remark("佣金审核")]
            CommissionRateAudit = 7
        }

        public enum MerchantStatus
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("待完善资料")]
            WaitFill = 1,
            [Remark("完善资料中")]
            InFill = 2,
            [Remark("已完善")]
            Filled = 3
        }

        public enum MerchantType
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("车行")]
            CarSales = 1,
            [Remark("维修店")]
            CarRepair = 2,
            [Remark("美容店")]
            CarBeauty = 3
        }


        public enum RepairCapacity
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("无维修能力")]
            NoRepair = 1,
            [Remark("无定损权但有维修能力")]
            NoEstimateButRepair = 2,
            [Remark("有定损权也有维修能力")]
            EstimateAndRepair = 3,
        }

        public enum RepairsType
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("维修和定损")]
            EstimateRepair = 1,
            [Remark("只定损")]
            Estimate = 2
        }

        public enum WithdrawStatus
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("发起请求")]
            SendRequest = 1,
            [Remark("处理中")]
            Handing = 2,
            [Remark("成功")]
            Success = 3,
            [Remark("失败")]
            Failure = 4
        }

        public enum TransactionsType
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("提现")]
            Withdraw = 1,
            [Remark("佣金")]
            CarClaimCommission = 2,
            [Remark("佣金")]
            CarInsureCommission = 3,
            [Remark("收取手续费")]
            ChargeFee = 4,
            [Remark("退回手续费")]
            ReturnFee = 5,
            [Remark("转入提现资金")]
            TurnsInWithdrawFund = 6,
            [Remark("转出提现资金")]
            TurnsOutWithdrawFund = 7,
            [Remark("退回提现资金")]
            ReturnWithdrawFund = 8,
            [Remark("提现失败冲回")]
            WithdrawRefund = 9,
            [Remark("垫付资金")]
            Advance = 10,
            [Remark("押金租金")]
            DepositRent = 11,
            [Remark("租金")]
            Rent = 12
        }

        public enum CommissionRateType
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("全线通")]
            Uplink = 1,
            [Remark("易办事")]
            YiBanShi = 2,
            [Remark("保险公司")]
            InsuranceCompany = 3
        }

        public enum CommissionRateAuditStatus
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("等待初审")]
            WaitPrimaryAudit = 1,
            [Remark("初审中")]
            InPrimaryAudit = 2,
            [Remark("等待复审")]
            WaitSeniorAudit = 3,
            [Remark("复审中")]
            InSeniorAudit = 4,
            [Remark("复审通过")]
            SeniorAuditPass = 5,
            [Remark("复审驳回")]
            SeniorAuditReject = 6,
            [Remark("复审拒绝")]
            SeniorAuditRefuse = 7
        }

        public enum CommissionRateAuditStep
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("申请")]
            Apply = 1,
            [Remark("初审")]
            PrimaryAudit = 2,
            [Remark("复审")]
            SeniorAudit = 3
        }


        public enum CarUserCharacter
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("家庭自用汽车")]
            HU = 1,
            [Remark("企业非营运客车")]
            ENonOB = 2,
            [Remark("非营运货车")]
            NonOV = 3,
            [Remark("营运货车")]
            OV = 5,
            [Remark("营运特种车")]
            OSV = 6,
            [Remark("非营运特种车")]
            NoOSV = 7
        }

        public enum CarInsuranceCompanyStatus
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("佣金比例审核中")]
            Audit = 1,
            [Remark("正常")]
            Normal = 2,
            [Remark("停用")]
            Disable = 3
        }
    }
}
