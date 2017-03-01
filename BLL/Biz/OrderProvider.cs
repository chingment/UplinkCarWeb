﻿using Lumos.Entity;
using Lumos.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lumos.BLL
{
    public class OrderProvider : BaseProvider
    {
        public CustomJsonResult SubmitCarInsure(int operater, int userId, OrderToCarInsure orderToCarInsure, List<OrderToCarInsureOfferCompany> orderToCarInsureOfferCompany, List<OrderToCarInsureOfferKind> orderToCarInsureOfferKind)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                OrderToCarInsure order = new OrderToCarInsure();

                //用户信息
                var clientUser = CurrentDb.SysClientUser.Where(m => m.Id == userId).FirstOrDefault();
                //商户信息
                var merchant = CurrentDb.Merchant.Where(m => m.Id == clientUser.MerchantId).FirstOrDefault();
                //2011为车险投保产品,2012为车险续保产品
                var product = CurrentDb.Product.Where(m => m.Type == orderToCarInsure.ProductType).FirstOrDefault();


                order.MerchantId = merchant.Id;
                order.PosMachineId = orderToCarInsure.PosMachineId;
                order.UserId = merchant.UserId;
                order.ProductId = product.Id;
                order.ProductName = product.Name;
                order.ProductType = product.Type;
                order.ClientRequire = orderToCarInsure.ClientRequire;


                order.InsuranceCompanyId = orderToCarInsure.InsuranceCompanyId;
                order.InsuranceCompanyName = orderToCarInsure.InsuranceCompanyName;

                order.CarOwner = orderToCarInsure.CarOwner;
                order.CarOwnerIdNumber = orderToCarInsure.CarOwnerIdNumber;
                order.CarOwnerAddress = orderToCarInsure.CarOwnerAddress;
                order.CarModel = orderToCarInsure.CarModel;
                order.CarOwner = orderToCarInsure.CarOwner;
                order.CarPlateNo = orderToCarInsure.CarPlateNo;
                order.CarEngineNo = orderToCarInsure.CarEngineNo;
                order.CarVin = orderToCarInsure.CarVin;
                order.CarVechicheType = orderToCarInsure.CarVechicheType;
                order.CarRegisterDate = orderToCarInsure.CarRegisterDate;
                order.CarIssueDate = orderToCarInsure.CarIssueDate;


                order.InsurePlanId = orderToCarInsure.InsurePlanId;
                order.CZ_CL_XSZ_ImgUrl = orderToCarInsure.CZ_CL_XSZ_ImgUrl;
                order.CZ_SFZ_ImgUrl = orderToCarInsure.CZ_SFZ_ImgUrl;
                order.YCZ_CLDJZ_ImgUrl = orderToCarInsure.YCZ_CLDJZ_ImgUrl;
                order.CCSJM_WSZM_ImgUrl = orderToCarInsure.CCSJM_WSZM_ImgUrl;
                order.Status = Enumeration.OrderStatus.Submitted;
                order.StartOfferTime = this.DateTime;
                order.SubmitTime = this.DateTime;
                order.CreateTime = this.DateTime;
                order.Creator = operater;
                order.IsLastYearNewCar = orderToCarInsure.IsLastYearNewCar;
                order.IsSameLastYear = orderToCarInsure.IsSameLastYear;
                CurrentDb.OrderToCarInsure.Add(order);
                CurrentDb.SaveChanges();
                order.Sn = Sn.Build(SnType.CarInsure, order.Id);



                if (orderToCarInsureOfferCompany != null)
                {
                    foreach (var m in orderToCarInsureOfferCompany)
                    {
                        m.OrderId = order.Id;
                        m.CreateTime = this.DateTime;
                        m.Creator = operater;
                        CurrentDb.OrderToCarInsureOfferCompany.Add(m);
                    }
                }

                if (orderToCarInsureOfferKind != null)
                {
                    foreach (var m in orderToCarInsureOfferKind)
                    {
                        m.OrderId = order.Id;
                        m.CreateTime = this.DateTime;
                        m.Creator = operater;
                        CurrentDb.OrderToCarInsureOfferKind.Add(m);
                    }
                }


                BizProcessesAudit bizProcessesAudit = BizFactory.BizProcessesAudit.Add(operater, Enumeration.BizProcessesAuditType.CarInsureOffer, order.Id, Enumeration.CarInsureOfferDealtStatus.WaitOffer, "");

                BizFactory.BizProcessesAudit.ChangeAuditDetails(Enumeration.OperateType.Submit, Enumeration.CarInsureOfferDealtStep.Submit, bizProcessesAudit.Id, operater, orderToCarInsure.ClientRequire, "商户提交投保订单", this.DateTime);

                CurrentDb.SaveChanges();
                ts.Complete();


                result = new CustomJsonResult(ResultType.Success, "提交成功");
            }

            return result;
        }

        public CustomJsonResult SubmitFollowInsure(int operater, OrderToCarInsure orderToCarInsure)
        {
            CustomJsonResult result = new CustomJsonResult();

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    var l_orderToCarInsure = CurrentDb.OrderToCarInsure.Where(m => m.Id == orderToCarInsure.Id).FirstOrDefault();
                    l_orderToCarInsure.ZJ1_ImgUrl = orderToCarInsure.ZJ1_ImgUrl;
                    l_orderToCarInsure.ZJ2_ImgUrl = orderToCarInsure.ZJ2_ImgUrl;
                    l_orderToCarInsure.ZJ3_ImgUrl = orderToCarInsure.ZJ3_ImgUrl;
                    l_orderToCarInsure.ZJ4_ImgUrl = orderToCarInsure.ZJ4_ImgUrl;
                    l_orderToCarInsure.FollowStatus = (int)Enumeration.OrderToCarInsureFollowStatus.Submitted;
                    l_orderToCarInsure.LastUpdateTime = this.DateTime;
                    l_orderToCarInsure.Mender = operater;


                    var bizProcessesAudit = CurrentDb.BizProcessesAudit.Where(m => m.AduitReferenceId == l_orderToCarInsure.Id && m.AduitType == Enumeration.BizProcessesAuditType.CarInsureOffer).FirstOrDefault();

                    BizFactory.BizProcessesAudit.ChangeAuditDetails(Enumeration.OperateType.Submit, Enumeration.CarInsureOfferDealtStep.Fllow, bizProcessesAudit.Id, operater, orderToCarInsure.ClientRequire, "商户再次提交投保订单", this.DateTime);

                    BizFactory.BizProcessesAudit.ChangeCarInsureOfferDealtStatus(operater, bizProcessesAudit.Id, Enumeration.CarInsureOfferDealtStatus.WaitOffer);

                    CurrentDb.SaveChanges();
                    ts.Complete();
                    result = new CustomJsonResult(ResultType.Success, "提交成功");
                }

                return result;
            }
            catch (Exception ex)
            {
                Log.Error("出错" + orderToCarInsure.Id, ex);

                return result;
            }



        }

        public CustomJsonResult SubmitCarInsureOffer(int operater, Enumeration.OperateType operate, OrderToCarInsure orderToCarInsure, List<OrderToCarInsureOfferCompany> orderToCarInsureOfferCompany, BizProcessesAudit bizProcessesAudit)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {


                var l_bizProcessesAudit = CurrentDb.BizProcessesAudit.Where(m => m.Id == bizProcessesAudit.CurrentDetails.BizProcessesAuditId && (m.Status == (int)Enumeration.CarInsureOfferDealtStatus.WaitOffer || m.Status == (int)Enumeration.CarInsureOfferDealtStatus.InOffer)).FirstOrDefault();

                if (l_bizProcessesAudit == null)
                {
                    return new CustomJsonResult(ResultType.Success, "该订单已经处理完成");
                }

                if (l_bizProcessesAudit.Auditor != null)
                {
                    if (l_bizProcessesAudit.Auditor.Value != operater)
                    {
                        return new CustomJsonResult(ResultType.Failure, "该订单其他用户正在处理");
                    }
                }

                var l_orderToCarInsure = CurrentDb.OrderToCarInsure.Where(m => m.Id == orderToCarInsure.Id).FirstOrDefault();

                if (l_orderToCarInsure.Status == Enumeration.OrderStatus.Cancled)
                {
                    return new CustomJsonResult(ResultType.Failure, "该订单已经被取消");
                }


                l_orderToCarInsure.CarOwner = orderToCarInsure.CarOwner;
                l_orderToCarInsure.CarOwnerIdNumber = orderToCarInsure.CarOwnerIdNumber;
                l_orderToCarInsure.CarOwnerAddress = orderToCarInsure.CarOwnerAddress;
                l_orderToCarInsure.CarPlateNo = orderToCarInsure.CarPlateNo;
                l_orderToCarInsure.CarRegisterDate = orderToCarInsure.CarRegisterDate;
                l_orderToCarInsure.CarIssueDate = orderToCarInsure.CarIssueDate;
                l_orderToCarInsure.CarSeat = orderToCarInsure.CarSeat;
                l_orderToCarInsure.CarUserCharacter = orderToCarInsure.CarUserCharacter;
                l_orderToCarInsure.CarVin = orderToCarInsure.CarVin;
                l_orderToCarInsure.CarVechicheType = orderToCarInsure.CarVechicheType;
                l_orderToCarInsure.CarModel = orderToCarInsure.CarModel;
                l_orderToCarInsure.CarEngineNo = orderToCarInsure.CarEngineNo;


                l_orderToCarInsure.PeriodStart = orderToCarInsure.PeriodStart;
                if (orderToCarInsure.PeriodStart != null)
                {
                    l_orderToCarInsure.PeriodEnd = orderToCarInsure.PeriodStart.Value.AddYears(1);
                }
                l_orderToCarInsure.Remarks = orderToCarInsure.Remarks;
                l_orderToCarInsure.LastUpdateTime = this.DateTime;
                l_orderToCarInsure.Mender = operater;



                bizProcessesAudit.CurrentDetails.AuditComments = orderToCarInsure.Remarks;




                foreach (var m in orderToCarInsureOfferCompany)
                {
                    var l_orderToCarInsureOfferCompany = CurrentDb.OrderToCarInsureOfferCompany.Where(q => q.Id == m.Id).FirstOrDefault();
                    if (l_orderToCarInsureOfferCompany != null)
                    {
                        l_orderToCarInsureOfferCompany.InsuranceOrderId = m.InsuranceOrderId;
                        l_orderToCarInsureOfferCompany.InsureImgUrl = m.InsureImgUrl;
                        l_orderToCarInsureOfferCompany.CommercialPrice = m.CommercialPrice;
                        l_orderToCarInsureOfferCompany.CompulsoryPrice = m.CompulsoryPrice;
                        l_orderToCarInsureOfferCompany.TravelTaxPrice = m.TravelTaxPrice;

                        decimal insureTotalPrice = 0;
                        if (l_orderToCarInsureOfferCompany.CommercialPrice != null)
                        {
                            insureTotalPrice += l_orderToCarInsureOfferCompany.CommercialPrice.Value;
                        }

                        if (l_orderToCarInsureOfferCompany.CompulsoryPrice != null)
                        {
                            insureTotalPrice += l_orderToCarInsureOfferCompany.CompulsoryPrice.Value;
                        }

                        if (l_orderToCarInsureOfferCompany.TravelTaxPrice != null)
                        {
                            insureTotalPrice += l_orderToCarInsureOfferCompany.TravelTaxPrice.Value;
                        }

                        l_orderToCarInsureOfferCompany.InsureTotalPrice = insureTotalPrice;

                        l_orderToCarInsureOfferCompany.LastUpdateTime = this.DateTime;
                        l_orderToCarInsureOfferCompany.Mender = operater;
                    }

                }

                if (l_orderToCarInsure.ProductType == Enumeration.ProductType.InsureForCarForRenewal)
                {
                    if (orderToCarInsureOfferCompany.Count == 1)
                    {

                        var insuranceOrderId = orderToCarInsureOfferCompany[0].InsuranceOrderId;
                        var insureImgUrl = orderToCarInsureOfferCompany[0].InsureImgUrl;
                        var commercialPrice = orderToCarInsureOfferCompany[0].CommercialPrice;
                        var compulsoryPrice = orderToCarInsureOfferCompany[0].CompulsoryPrice;
                        var travelTaxPrice = orderToCarInsureOfferCompany[0].TravelTaxPrice;
                        decimal insureTotalPrice = 0;
                        if (orderToCarInsureOfferCompany[0].CommercialPrice != null)
                        {
                            insureTotalPrice += orderToCarInsureOfferCompany[0].CommercialPrice.Value;
                        }

                        if (orderToCarInsureOfferCompany[0].CompulsoryPrice != null)
                        {
                            insureTotalPrice += orderToCarInsureOfferCompany[0].CompulsoryPrice.Value;
                        }

                        if (orderToCarInsureOfferCompany[0].TravelTaxPrice != null)
                        {
                            insureTotalPrice += orderToCarInsureOfferCompany[0].TravelTaxPrice.Value;
                        }


                        l_orderToCarInsure.InsuranceOrderId = insuranceOrderId;
                        l_orderToCarInsure.InsureImgUrl = insureImgUrl;
                        l_orderToCarInsure.CommercialPrice = commercialPrice == null ? 0 : commercialPrice.Value;
                        l_orderToCarInsure.TravelTaxPrice = travelTaxPrice == null ? 0 : travelTaxPrice.Value;
                        l_orderToCarInsure.CompulsoryPrice = compulsoryPrice == null ? 0 : compulsoryPrice.Value;
                        l_orderToCarInsure.Price = insureTotalPrice;

                    }
                }

                var creator = CurrentDb.Users.Where(m => m.Id == l_orderToCarInsure.Creator).FirstOrDefault();

                switch (operate)
                {
                    case Enumeration.OperateType.Save:

                        result = new CustomJsonResult(ResultType.Success, "保存成功");

                        BizFactory.BizProcessesAudit.ChangeAuditDetailsAuditComments(operater, bizProcessesAudit.CurrentDetails.Id, bizProcessesAudit.CurrentDetails.AuditComments, null);

                        break;
                    case Enumeration.OperateType.Reject:

                        l_orderToCarInsure.Status = Enumeration.OrderStatus.Follow;
                        l_orderToCarInsure.FollowStatus = (int)Enumeration.OrderToCarInsureFollowStatus.WaitSubmit;

                        BizFactory.BizProcessesAudit.ChangeAuditDetailsAuditComments(operater, bizProcessesAudit.CurrentDetails.Id, bizProcessesAudit.CurrentDetails.AuditComments, "后台人员转给商户跟进", this.DateTime);

                        BizFactory.BizProcessesAudit.ChangeCarInsureOfferDealtStatus(operater, bizProcessesAudit.CurrentDetails.BizProcessesAuditId, Enumeration.CarInsureOfferDealtStatus.ClientFllow, "商户正在跟进");

                        BizFactory.Sms.SendCarInsureOfferFollow(creator.Id, creator.PhoneNumber, l_orderToCarInsure.Sn);

                        result = new CustomJsonResult(ResultType.Success, "转给客户跟进成功");

                        break;
                    case Enumeration.OperateType.Cancle:

                        l_orderToCarInsure.CancleTime = this.DateTime;
                        l_orderToCarInsure.EndOfferTime = this.DateTime;
                        l_orderToCarInsure.Status = Enumeration.OrderStatus.Cancled;


                        BizFactory.BizProcessesAudit.ChangeAuditDetails(operate, Enumeration.CarInsureOfferDealtStep.Offer, bizProcessesAudit.CurrentDetails.Id, operater, bizProcessesAudit.CurrentDetails.AuditComments, "后台人员撤销订单", this.DateTime);


                        BizFactory.BizProcessesAudit.ChangeCarInsureOfferDealtStatus(operater, bizProcessesAudit.CurrentDetails.BizProcessesAuditId, Enumeration.CarInsureOfferDealtStatus.StaffCancle);

                        result = new CustomJsonResult(ResultType.Success, "撤销成功");

                        break;
                    case Enumeration.OperateType.Submit:

                        l_orderToCarInsure.EndOfferTime = this.DateTime;
                        l_orderToCarInsure.Status = Enumeration.OrderStatus.WaitPay;


                        BizFactory.BizProcessesAudit.ChangeAuditDetails(operate, Enumeration.CarInsureOfferDealtStep.Offer, bizProcessesAudit.CurrentDetails.Id, operater, bizProcessesAudit.CurrentDetails.AuditComments, "后台人员完成报价", this.DateTime);
                        BizFactory.BizProcessesAudit.ChangeCarInsureOfferDealtStatus(operater, bizProcessesAudit.CurrentDetails.BizProcessesAuditId, Enumeration.CarInsureOfferDealtStatus.Complete);

                        BizFactory.Sms.SendCarInsureOfferComplete(creator.Id, creator.PhoneNumber, l_orderToCarInsure.Sn, l_orderToCarInsure.CarOwner, l_orderToCarInsure.CarPlateNo);

                        result = new CustomJsonResult(ResultType.Success, "提交成功");
                        break;

                }


                CurrentDb.SaveChanges();
                ts.Complete();
            }

            return result;

        }

        public CustomJsonResult SubmitClaim(int operater, int userId, OrderToCarClaim orderToCarClaim)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                //用户信息
                var clientUser = CurrentDb.SysClientUser.Where(m => m.Id == userId).FirstOrDefault();
                //商户信息
                var merchant = CurrentDb.Merchant.Where(m => m.Id == clientUser.MerchantId).FirstOrDefault();

                var insuranceCompany = CurrentDb.InsuranceCompany.Where(m => m.Id == orderToCarClaim.InsuranceCompanyId).FirstOrDefault();

                //2011为车险理赔
                var product = CurrentDb.Product.Where(m => m.Id == 2013).FirstOrDefault();

                orderToCarClaim.ProductId = product.Id;
                orderToCarClaim.ProductType = product.Type;
                orderToCarClaim.ProductName = product.Name;
                orderToCarClaim.MerchantId = merchant.Id;
                orderToCarClaim.PosMachineId = orderToCarClaim.PosMachineId;
                orderToCarClaim.UserId = merchant.UserId;
                orderToCarClaim.InsuranceCompanyName = insuranceCompany.Name;
                orderToCarClaim.Status = Enumeration.OrderStatus.Submitted;
                orderToCarClaim.SubmitTime = this.DateTime;
                orderToCarClaim.CreateTime = this.DateTime;
                orderToCarClaim.Creator = operater;
                CurrentDb.OrderToCarClaim.Add(orderToCarClaim);
                CurrentDb.SaveChanges();
                orderToCarClaim.Sn = Sn.Build(SnType.CarClaim, orderToCarClaim.Id);

                //状态改为待核实
                BizProcessesAudit bizProcessesAudit = BizFactory.BizProcessesAudit.Add(operater, Enumeration.BizProcessesAuditType.CarClaim, orderToCarClaim.Id, Enumeration.CarClaimDealtStatus.WaitVerifyOrder, "");
                BizFactory.BizProcessesAudit.ChangeAuditDetails(Enumeration.OperateType.Submit, Enumeration.CarClaimDealtStep.Submit, bizProcessesAudit.Id, operater, orderToCarClaim.ClientRequire, "商户提交理赔需求", this.DateTime);


                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, "提交成功");
            }


            return result;
        }

        public CustomJsonResult SubmitEstimateList(int operater, int userId, int orderId, string estimateListImgUrl)
        {
            CustomJsonResult result = new CustomJsonResult();

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    var estimateOrderToCarClaim = CurrentDb.OrderToCarClaim.Where(m => m.Id == orderId).FirstOrDefault();

                    estimateOrderToCarClaim.EstimateListImgUrl = estimateListImgUrl;
                    estimateOrderToCarClaim.FollowStatus = (int)Enumeration.OrderToCarClaimFollowStatus.VerifyEstimateAmount;
                    estimateOrderToCarClaim.Remarks = "定损单已经上传，正在核实";
                    estimateOrderToCarClaim.LastUpdateTime = this.DateTime;
                    estimateOrderToCarClaim.Mender = operater;


                    var orderToCarClaim = CurrentDb.OrderToCarClaim.Where(m => m.Id == estimateOrderToCarClaim.PId).FirstOrDefault();
                    orderToCarClaim.EstimateListImgUrl = estimateListImgUrl;
                    orderToCarClaim.FollowStatus = (int)Enumeration.OrderToCarClaimFollowStatus.VerifyEstimateAmount;
                    orderToCarClaim.Remarks = "定损单已经上传，正在核实";
                    orderToCarClaim.LastUpdateTime = this.DateTime;
                    orderToCarClaim.Mender = operater;

                    var bizProcessesAudit = CurrentDb.BizProcessesAudit.Where(m => m.AduitReferenceId == orderToCarClaim.Id && m.AduitType == Enumeration.BizProcessesAuditType.CarClaim).FirstOrDefault();

                    BizFactory.BizProcessesAudit.ChangeAuditDetails(Enumeration.OperateType.Submit, Enumeration.CarClaimDealtStep.UploadEstimateListImg, bizProcessesAudit.Id, operater, "定损单已经上传，正在核实", "商户提交定损单", this.DateTime);

                    BizFactory.BizProcessesAudit.ChangeCarClaimDealtStatus(operater, bizProcessesAudit.Id, Enumeration.CarClaimDealtStatus.WaitVerifyAmount);


                    CurrentDb.SaveChanges();
                    ts.Complete();

                    result = new CustomJsonResult(ResultType.Success, "提交成功");
                }

                return result;
            }
            catch (Exception ex)
            {
                Log.Error("出错" + orderId, ex);

                return result;
            }

        }

        public CustomJsonResult VerifyClaimOrder(int operater, Enumeration.OperateType operate, OrderToCarClaim orderToCarClaim, int estimateMerchantId, string estimateMerchantRemarks, BizProcessesAudit bizProcessesAudit)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {

                var l_bizProcessesAudit = CurrentDb.BizProcessesAudit.Where(m => m.Id == bizProcessesAudit.CurrentDetails.BizProcessesAuditId && (m.Status == (int)Enumeration.CarInsureOfferDealtStatus.WaitOffer || m.Status == (int)Enumeration.CarInsureOfferDealtStatus.InOffer)).FirstOrDefault();

                if (bizProcessesAudit == null)
                {
                    return new CustomJsonResult(ResultType.Success, "该订单已经处理完成");
                }

                if (bizProcessesAudit.Auditor != null)
                {
                    if (bizProcessesAudit.Auditor.Value != operater)
                    {
                        return new CustomJsonResult(ResultType.Failure, "该订单其他用户正在处理");
                    }
                }


                var l_orderToCarClaim = CurrentDb.OrderToCarClaim.Where(m => m.Id == orderToCarClaim.Id).FirstOrDefault();
                l_orderToCarClaim.HandMerchantId = estimateMerchantId;
                l_orderToCarClaim.HandMerchantType = Enumeration.MerchantType.CarRepair;
                l_orderToCarClaim.Remarks = orderToCarClaim.Remarks;

                bizProcessesAudit.CurrentDetails.AuditComments = orderToCarClaim.Remarks;

                switch (operate)
                {
                    case Enumeration.OperateType.Save:

                        result = new CustomJsonResult(ResultType.Success, "保存成功");

                        BizFactory.BizProcessesAudit.ChangeAuditDetails(operate, Enumeration.CarClaimDealtStep.VerifyOrder, bizProcessesAudit.CurrentDetails.BizProcessesAuditId, operater, bizProcessesAudit.CurrentDetails.AuditComments, null);

                        break;
                    case Enumeration.OperateType.Cancle:

                        l_orderToCarClaim.Status = Enumeration.OrderStatus.Cancled;
                        l_orderToCarClaim.CancleTime = this.DateTime;

                        BizFactory.BizProcessesAudit.ChangeAuditDetails(operate, Enumeration.CarClaimDealtStep.VerifyOrder, bizProcessesAudit.CurrentDetails.BizProcessesAuditId, operater, bizProcessesAudit.CurrentDetails.AuditComments, "后台人员撤销订单", this.DateTime);

                        BizFactory.BizProcessesAudit.ChangeCarInsureOfferDealtStatus(operater, bizProcessesAudit.CurrentDetails.BizProcessesAuditId, Enumeration.CarInsureOfferDealtStatus.StaffCancle);

                        result = new CustomJsonResult(ResultType.Success, "撤销成功");

                        break;
                    case Enumeration.OperateType.Submit:

                        l_orderToCarClaim.Status = Enumeration.OrderStatus.Follow;

                        l_orderToCarClaim.FollowStatus = (int)Enumeration.OrderToCarClaimFollowStatus.WaitEstimate;


                        var merchant = CurrentDb.Merchant.Where(m => m.Id == l_orderToCarClaim.HandMerchantId).FirstOrDefault();

                        var estimateOrderToCarClaim = new OrderToCarClaim();
                        estimateOrderToCarClaim.RepairsType = l_orderToCarClaim.RepairsType;
                        estimateOrderToCarClaim.MerchantId = merchant.Id;
                        estimateOrderToCarClaim.PosMachineId = l_orderToCarClaim.PosMachineId;
                        estimateOrderToCarClaim.UserId = merchant.UserId;
                        estimateOrderToCarClaim.HandPerson = l_orderToCarClaim.HandPerson;
                        estimateOrderToCarClaim.HandPersonPhone = l_orderToCarClaim.HandPersonPhone;
                        estimateOrderToCarClaim.InsuranceCompanyId = l_orderToCarClaim.InsuranceCompanyId;
                        estimateOrderToCarClaim.InsuranceCompanyName = l_orderToCarClaim.InsuranceCompanyName;
                        estimateOrderToCarClaim.CarPlateNo = l_orderToCarClaim.CarPlateNo;
                        estimateOrderToCarClaim.Status = Enumeration.OrderStatus.Follow;
                        estimateOrderToCarClaim.FollowStatus = (int)Enumeration.OrderToCarClaimFollowStatus.WaitUploadEstimateList;
                        estimateOrderToCarClaim.SubmitTime = this.DateTime;
                        estimateOrderToCarClaim.Creator = operater;
                        estimateOrderToCarClaim.CreateTime = this.DateTime;

                        estimateOrderToCarClaim.HandMerchantId = l_orderToCarClaim.MerchantId;
                        estimateOrderToCarClaim.HandMerchantType = Enumeration.MerchantType.CarSales;
                        estimateOrderToCarClaim.Remarks = estimateMerchantRemarks;//告知维修厂备注


                        estimateOrderToCarClaim.ProductId = l_orderToCarClaim.ProductId;
                        estimateOrderToCarClaim.ProductName = l_orderToCarClaim.ProductName;
                        estimateOrderToCarClaim.ProductType = l_orderToCarClaim.ProductType;
                        estimateOrderToCarClaim.PId = l_orderToCarClaim.Id;
                        estimateOrderToCarClaim.ClientRequire = l_orderToCarClaim.ClientRequire;
                        CurrentDb.OrderToCarClaim.Add(estimateOrderToCarClaim);
                        estimateOrderToCarClaim.Sn = Sn.Build(SnType.CarClaim, estimateOrderToCarClaim.Id);


                        BizFactory.BizProcessesAudit.ChangeAuditDetails(operate, Enumeration.CarClaimDealtStep.VerifyOrder, bizProcessesAudit.CurrentDetails.BizProcessesAuditId, operater, bizProcessesAudit.CurrentDetails.AuditComments, "后台人员派单完成", this.DateTime);

                        BizFactory.BizProcessesAudit.ChangeCarClaimDealtStatus(operater, bizProcessesAudit.CurrentDetails.BizProcessesAuditId, Enumeration.CarClaimDealtStatus.FllowUploadEstimateListImg, "等待商户上传定损单");

                        result = new CustomJsonResult(ResultType.Success, "提交成功");
                        break;

                }



                CurrentDb.SaveChanges();
                ts.Complete();
            }

            return result;

        }

        public CustomJsonResult VerifyClaimAmount(int operater, Enumeration.OperateType operate, OrderToCarClaim orderToCarClaim, BizProcessesAudit bizProcessesAudit)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {

                //var l_bizProcessesAudit = CurrentDb.BizProcessesAudit.Where(m => m.Id == bizProcessesAudit.CurrentDetails.BizProcessesAuditId && (m.Status == (int)Enumeration.CarInsureOfferDealtStatus.WaitOffer || m.Status == (int)Enumeration.CarInsureOfferDealtStatus.InOffer)).FirstOrDefault();

                //if (bizProcessesAudit == null)
                //{
                //    return new CustomJsonResult(ResultType.Success, "该订单已经处理完成");
                //}

                //if (bizProcessesAudit.Auditor != null)
                //{
                //    if (bizProcessesAudit.Auditor.Value != operater)
                //    {
                //        return new CustomJsonResult(ResultType.Failure, "该订单其他用户正在处理");
                //    }
                //}


                var l_orderToCarClaim = CurrentDb.OrderToCarClaim.Where(m => m.Id == orderToCarClaim.Id).FirstOrDefault();
                l_orderToCarClaim.WorkingHoursPrice = orderToCarClaim.WorkingHoursPrice;
                l_orderToCarClaim.AccessoriesPrice = orderToCarClaim.AccessoriesPrice;
                l_orderToCarClaim.EstimatePrice = orderToCarClaim.WorkingHoursPrice + orderToCarClaim.AccessoriesPrice;
                l_orderToCarClaim.Remarks = orderToCarClaim.Remarks;


                //bizProcessesAudit.CurrentDetails.AuditComments = orderToCarClaim.Remarks;
                var estimateOrderToCarClaim = CurrentDb.OrderToCarClaim.Where(m => m.PId == orderToCarClaim.Id).FirstOrDefault();
                switch (operate)
                {
                    case Enumeration.OperateType.Save:

                        result = new CustomJsonResult(ResultType.Success, "保存成功");

                        BizFactory.BizProcessesAudit.ChangeAuditDetailsAuditComments(operater, bizProcessesAudit.CurrentDetails.BizProcessesAuditId, bizProcessesAudit.CurrentDetails.AuditComments, null);

                        break;
                    case Enumeration.OperateType.Cancle:

                        l_orderToCarClaim.Status = Enumeration.OrderStatus.Cancled;
                        l_orderToCarClaim.CancleTime = this.DateTime;

                        estimateOrderToCarClaim.Status = Enumeration.OrderStatus.Cancled;
                        estimateOrderToCarClaim.CancleTime = this.DateTime;

                        BizFactory.BizProcessesAudit.ChangeAuditDetails(operate, Enumeration.CarClaimDealtStep.VerifyAmount, bizProcessesAudit.CurrentDetails.BizProcessesAuditId, operater, bizProcessesAudit.CurrentDetails.AuditComments, "后台人员撤销订单", this.DateTime);

                        BizFactory.BizProcessesAudit.ChangeCarClaimDealtStatus(operater, bizProcessesAudit.CurrentDetails.BizProcessesAuditId, Enumeration.CarClaimDealtStatus.StaffCancle);

                        result = new CustomJsonResult(ResultType.Success, "撤销成功");

                        break;
                    case Enumeration.OperateType.Submit:


                        estimateOrderToCarClaim.WorkingHoursPrice = l_orderToCarClaim.WorkingHoursPrice;
                        estimateOrderToCarClaim.AccessoriesPrice = l_orderToCarClaim.AccessoriesPrice;


                        CalculateCarClaimPayPrice calculateCarClaimPayPrice = new CalculateCarClaimPayPrice(l_orderToCarClaim.WorkingHoursPrice, l_orderToCarClaim.AccessoriesPrice);

                        estimateOrderToCarClaim.EstimatePrice = l_orderToCarClaim.EstimatePrice;
                        estimateOrderToCarClaim.Remarks = orderToCarClaim.Remarks;


                        if (l_orderToCarClaim.RepairsType == Enumeration.RepairsType.EstimateRepair)
                        {
                            l_orderToCarClaim.Status = Enumeration.OrderStatus.Follow;
                            l_orderToCarClaim.FollowStatus = (int)Enumeration.OrderToCarClaimFollowStatus.WaitPayCommission;
                            l_orderToCarClaim.Price = 0;


                            estimateOrderToCarClaim.Status = Enumeration.OrderStatus.WaitPay;
                            estimateOrderToCarClaim.FollowStatus = (int)Enumeration.OrderToCarClaimFollowStatus.WaitPayCommission;
                            estimateOrderToCarClaim.Price = calculateCarClaimPayPrice.PayPrice;//应付金额

                        }
                        else if (l_orderToCarClaim.RepairsType == Enumeration.RepairsType.Estimate)
                        {
                            l_orderToCarClaim.Status = Enumeration.OrderStatus.WaitPay;
                            l_orderToCarClaim.FollowStatus = (int)Enumeration.OrderToCarClaimFollowStatus.WaitPayCommission;
                            l_orderToCarClaim.Price = calculateCarClaimPayPrice.PayPrice;//应付金额

                            estimateOrderToCarClaim.Status = Enumeration.OrderStatus.Follow;
                            estimateOrderToCarClaim.FollowStatus = (int)Enumeration.OrderToCarClaimFollowStatus.WaitPayCommission;
                            estimateOrderToCarClaim.Price = 0;
                        }

                        BizFactory.BizProcessesAudit.ChangeAuditDetails(operate, Enumeration.CarClaimDealtStep.VerifyAmount, bizProcessesAudit.CurrentDetails.BizProcessesAuditId, operater, bizProcessesAudit.CurrentDetails.AuditComments, "复核金额完成，提交给商户支付", this.DateTime);

                        BizFactory.BizProcessesAudit.ChangeCarClaimDealtStatus(operater, bizProcessesAudit.CurrentDetails.BizProcessesAuditId, Enumeration.CarClaimDealtStatus.Complete, "复核金额完成，提交给商户支付");


                        result = new CustomJsonResult(ResultType.Success, "提交成功");
                        break;

                }



                CurrentDb.SaveChanges();
                ts.Complete();
            }

            return result;

        }

        public CustomJsonResult Pay(int operater, string orderSn)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var order = CurrentDb.Order.Where(m => m.Sn == "").FirstOrDefault();
                if (order == null)
                {
                    return new CustomJsonResult(ResultType.Failure, "找不到该订单");
                }

                switch (order.Status)
                {
                    case Enumeration.OrderStatus.Submitted:
                    case Enumeration.OrderStatus.Follow:
                        result = new CustomJsonResult(ResultType.Failure, "该订单在跟进中");
                        break;
                    case Enumeration.OrderStatus.Completed:
                        result = new CustomJsonResult(ResultType.Failure, "该订单已经完成");
                        break;
                    case Enumeration.OrderStatus.Cancled:
                        result = new CustomJsonResult(ResultType.Failure, "该订单已经取消");
                        break;
                    case Enumeration.OrderStatus.WaitPay:
                        result = Pay(operater, order);
                        break;
                }
            }


            return result;

        }

        private CustomJsonResult Pay(int operater, Order order)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                switch (order.ProductType)
                {
                    case Enumeration.ProductType.InsureForCarForInsure:
                    case Enumeration.ProductType.InsureForCarForRenewal:
                        // result = BizFactory.OrderToCarInsure.Pay(operater, order.Sn);
                        break;
                    case Enumeration.ProductType.InsureForCarForClaim:

                        break;
                }
            }

            return result;
        }


        public CustomJsonResult Cancle(int operater, string orderSn)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var order = CurrentDb.Order.Where(m => m.Sn == orderSn).FirstOrDefault();
                if (order == null)
                {
                    return new CustomJsonResult(ResultType.Failure, "找不到该订单");
                }

                if (order.Status == Enumeration.OrderStatus.Completed)
                {
                    return new CustomJsonResult(ResultType.Failure, "该已经完成，不能取消");
                }

                if (order.Status == Enumeration.OrderStatus.Cancled)
                {
                    return new CustomJsonResult(ResultType.Failure, "该已经被取消");
                }

                order.CancleTime = this.DateTime;
                order.Status = Enumeration.OrderStatus.Cancled;

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, "该订单取消成功");
            }


            return result;

        }



    }
}
