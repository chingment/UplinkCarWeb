﻿using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL
{
    public class BizProcessesAuditProvider : BaseProvider
    {
        public BizProcessesAudit Add(int operater, Enumeration.BizProcessesAuditType type, int referenceid, object status, string remark,string reason=null, object jsonObject = null)
        {
            DateTime nowDate = DateTime.Now;

            BizProcessesAudit bizProcessesAudit = new BizProcessesAudit();
            bizProcessesAudit.AduitType = type;

            string aduitTypeEnumName = "" + type.GetType().FullName + ", Lumos.Entity";
            bizProcessesAudit.AduitTypeEnumName = aduitTypeEnumName;

            bizProcessesAudit.AduitReferenceId = referenceid;
            bizProcessesAudit.Status = Convert.ToInt32(status);
            bizProcessesAudit.StartTime = nowDate;
            bizProcessesAudit.Creator = operater;
            bizProcessesAudit.CreateTime = nowDate;
            bizProcessesAudit.Remark = remark;
            bizProcessesAudit.Reason = reason;
            if (jsonObject != null)
            {
                bizProcessesAudit.JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject);
            }
            CurrentDb.BizProcessesAudit.Add(bizProcessesAudit);
            CurrentDb.SaveChanges();

            return bizProcessesAudit;
        }


        public BizProcessesAudit ChangeExtendedAppAuditStatus(int operater, int bizProcessesAuditId, Enumeration.ExtendedAppAuditStatus changestatus, DateTime? endTime = null)
        {

            var bizProcessesAudit = CurrentDb.BizProcessesAudit.Where(m => m.Id == bizProcessesAuditId).FirstOrDefault();
            if (bizProcessesAudit != null)
            {
                if (bizProcessesAudit.EndTime == null)
                {
                    if (
                       bizProcessesAudit.Status != (int)Enumeration.ExtendedAppAuditStatus.ReviewPass
                        && bizProcessesAudit.Status != (int)Enumeration.ExtendedAppAuditStatus.ReviewRefuse
                        )
                    {
                        Enumeration.ExtendedAppAuditStatus old_Status = (Enumeration.ExtendedAppAuditStatus)bizProcessesAudit.Status;
                        bizProcessesAudit.Mender = operater;
                        bizProcessesAudit.LastUpdateTime = DateTime.Now;


                        if (endTime != null)
                        {
                            bizProcessesAudit.EndTime = endTime.Value;
                        }

                        if (changestatus == Enumeration.ExtendedAppAuditStatus.InAudit)
                        {
                            bizProcessesAudit.Status = (int)Enumeration.ExtendedAppAuditStatus.InAudit;
                            if (bizProcessesAudit.Auditor == null)
                            {
                                bizProcessesAudit.Auditor = operater;

                                ChangeAuditDetails(Enumeration.OperateType.Save, Enumeration.ExtendedAppAuditStep.PrimaryAudit, bizProcessesAudit.Id, operater, null, null, null);

                            }


                        }
                        else if (changestatus == Enumeration.ExtendedAppAuditStatus.WaitReview)
                        {

                            var bizProcessesAuditDetails = CurrentDb.BizProcessesAuditDetails.Where(m => m.BizProcessesAuditId == bizProcessesAudit.Id && m.AuditStep == (int)Enumeration.ExtendedAppAuditStep.SeniorAudit).OrderByDescending(m => m.CreateTime).Take(1).FirstOrDefault();
                            if (bizProcessesAuditDetails == null)
                            {
                                bizProcessesAudit.Status = (int)Enumeration.ExtendedAppAuditStatus.WaitReview;
                                bizProcessesAudit.Auditor = null;

                            }
                            else
                            {
                                bizProcessesAudit.Status = (int)Enumeration.ExtendedAppAuditStatus.InReview;
                                bizProcessesAudit.Auditor = bizProcessesAuditDetails.Auditor;
                            }

                        }
                        else if (changestatus == Enumeration.ExtendedAppAuditStatus.InReview)
                        {
                            bizProcessesAudit.Status = (int)Enumeration.ExtendedAppAuditStatus.InReview;
                            if (bizProcessesAudit.Auditor == null)
                            {
                                bizProcessesAudit.Auditor = operater;
                                ChangeAuditDetails(Enumeration.OperateType.Save, Enumeration.ExtendedAppAuditStep.SeniorAudit, bizProcessesAudit.Id, operater, null, null);
                            }
                        }
                        else if (changestatus == Enumeration.ExtendedAppAuditStatus.ReviewReject)
                        {
                            //var enterpriseAuditDetailsHistory = CurrentDb.EnterpriseAuditDetailsHistory.Where(m => m.EnterpriseAuditHistoryId == enterpriseAuditHistoryId && m.AuditStep == Enumeration.EnterpriseAuditDetailsHistoryStep.PrimaryAudit).OrderByDescending(m => m.CreateTime).Take(1).FirstOrDefault();
                            //if (enterpriseAuditDetailsHistory == null)
                            //{
                            //    enterpriseAuditHistory.Status = Enumeration.EnterpriseAuditHistoryAuditStatus.WaitAudit;
                            //    enterpriseAuditHistory.Auditor = null;
                            //}
                            //else
                            //{
                            //    enterpriseAuditHistory.Status = Enumeration.EnterpriseAuditHistoryAuditStatus.InAudit;
                            //    enterpriseAuditHistory.Auditor = enterpriseAuditDetailsHistory.AuditPerson;
                            //}
                        }
                        else if (changestatus == Enumeration.ExtendedAppAuditStatus.ReviewPass)
                        {
                            bizProcessesAudit.Status = (int)Enumeration.ExtendedAppAuditStatus.ReviewPass;
                            bizProcessesAudit.Auditor = operater;
                        }
                        else if (changestatus == Enumeration.ExtendedAppAuditStatus.ReviewRefuse)
                        {
                            bizProcessesAudit.Status = (int)Enumeration.ExtendedAppAuditStatus.ReviewRefuse;
                            bizProcessesAudit.Auditor = operater;

                        }
                    }

                    CurrentDb.SaveChanges();
                }
            }

            return bizProcessesAudit;
        }

        public BizProcessesAudit ChangeMerchantAuditStatus(int operater, int bizProcessesAuditId, Enumeration.MerchantAuditStatus changestatus, DateTime? endTime = null)
        {

            var bizProcessesAudit = CurrentDb.BizProcessesAudit.Where(m => m.Id == bizProcessesAuditId).FirstOrDefault();
            if (bizProcessesAudit != null)
            {
                if (bizProcessesAudit.EndTime == null)
                {

                    Enumeration.MerchantAuditStatus old_Status = (Enumeration.MerchantAuditStatus)bizProcessesAudit.Status;
                    bizProcessesAudit.Mender = operater;
                    bizProcessesAudit.LastUpdateTime = DateTime.Now;


                    if (endTime != null)
                    {
                        bizProcessesAudit.EndTime = endTime.Value;
                    }

                    if (changestatus == Enumeration.MerchantAuditStatus.InPrimaryAudit)
                    {
                        bizProcessesAudit.Status = (int)Enumeration.MerchantAuditStatus.InPrimaryAudit;

                        if (bizProcessesAudit.Auditor == null)
                        {
                            bizProcessesAudit.Auditor = operater;

                            ChangeAuditDetails(Enumeration.OperateType.Save, Enumeration.MerchantAuditStep.PrimaryAudit, bizProcessesAudit.Id, operater, null, null, null);
                        }
                    }
                    else if (changestatus == Enumeration.MerchantAuditStatus.WaitSeniorAudit)
                    {

                        var bizProcessesAuditDetails = CurrentDb.BizProcessesAuditDetails.Where(m => m.BizProcessesAuditId == bizProcessesAudit.Id && m.AuditStep == (int)Enumeration.ExtendedAppAuditStep.SeniorAudit).OrderByDescending(m => m.CreateTime).Take(1).FirstOrDefault();
                        if (bizProcessesAuditDetails == null)
                        {
                            bizProcessesAudit.Status = (int)Enumeration.MerchantAuditStatus.WaitSeniorAudit;
                            bizProcessesAudit.Auditor = null;

                        }
                        else
                        {
                            bizProcessesAudit.Status = (int)Enumeration.MerchantAuditStatus.InSeniorAudit;
                            bizProcessesAudit.Auditor = bizProcessesAuditDetails.Auditor;

                            ChangeAuditDetails(Enumeration.OperateType.Save, Enumeration.ExtendedAppAuditStep.SeniorAudit, bizProcessesAudit.Id, operater, null, null);
                        }
                    }
                    else if (changestatus == Enumeration.MerchantAuditStatus.InSeniorAudit)
                    {
                        bizProcessesAudit.Status = (int)Enumeration.MerchantAuditStatus.InSeniorAudit;
                        if (bizProcessesAudit.Auditor == null)
                        {
                            bizProcessesAudit.Auditor = operater;
                            ChangeAuditDetails(Enumeration.OperateType.Save, Enumeration.ExtendedAppAuditStep.SeniorAudit, bizProcessesAudit.Id, operater, null, null);
                        }
                    }
                    else if (changestatus == Enumeration.MerchantAuditStatus.SeniorAuditReject)
                    {
                        var bizProcessesAuditDetails = CurrentDb.BizProcessesAuditDetails.Where(m => m.BizProcessesAuditId == bizProcessesAudit.Id && m.AuditStep == (int)Enumeration.MerchantAuditStep.PrimaryAudit).OrderByDescending(m => m.CreateTime).Take(1).FirstOrDefault();
                        if (bizProcessesAuditDetails != null)
                        {
                            bizProcessesAudit.Status = (int)Enumeration.MerchantAuditStatus.InPrimaryAudit;
                            bizProcessesAudit.Auditor = bizProcessesAuditDetails.Auditor;

                            ChangeAuditDetails(Enumeration.OperateType.Save, Enumeration.ExtendedAppAuditStep.PrimaryAudit, bizProcessesAudit.Id, bizProcessesAuditDetails.Auditor.Value, null, null);
                        }
                    }
                    else if (changestatus == Enumeration.MerchantAuditStatus.SeniorAuditPass)
                    {
                        bizProcessesAudit.Status = (int)Enumeration.MerchantAuditStatus.SeniorAuditPass;
                        bizProcessesAudit.Auditor = operater;
                    }



                    CurrentDb.SaveChanges();
                }

                var historicalDetails = CurrentDb.BizProcessesAuditDetails.Where(m => m.BizProcessesAuditId == bizProcessesAudit.Id).OrderByDescending(m => m.AuditTime).ToList();

                bizProcessesAudit.HistoricalDetails = historicalDetails.Where(m => m.AuditTime != null).ToList();


                Enumeration.MerchantAuditStep merchantAuditStep = Enumeration.MerchantAuditStep.Unknow;
                if (changestatus == Enumeration.MerchantAuditStatus.WaitPrimaryAudit || changestatus == Enumeration.MerchantAuditStatus.InPrimaryAudit)
                {
                    merchantAuditStep = Enumeration.MerchantAuditStep.PrimaryAudit;
                }
                else if (changestatus == Enumeration.MerchantAuditStatus.WaitSeniorAudit || changestatus == Enumeration.MerchantAuditStatus.InSeniorAudit)
                {
                    merchantAuditStep = Enumeration.MerchantAuditStep.SeniorAudit;
                }

                var currentDetails = historicalDetails.Where(m => m.BizProcessesAuditId == bizProcessesAudit.Id && m.AuditStep == (int)merchantAuditStep).OrderByDescending(m => m.CreateTime).Take(1).FirstOrDefault();
                if (currentDetails != null)
                {
                    bizProcessesAudit.CurrentDetails = currentDetails;

                    var auditComments = historicalDetails.Where(m => m.BizProcessesAuditId == bizProcessesAudit.Id && m.AuditStep == (int)merchantAuditStep && m.AuditComments != null).OrderByDescending(m => m.CreateTime).Take(1).FirstOrDefault();
                    if (auditComments != null)
                    {
                        bizProcessesAudit.CurrentDetails.AuditComments = auditComments.AuditComments;
                    }
                }

            }

            return bizProcessesAudit;
        }

        public List<BizProcessesAuditDetails> GetDetails(Enumeration.BizProcessesAuditType type, int referenceid)
        {
            List<BizProcessesAuditDetails> auditDetails = new List<BizProcessesAuditDetails>();
            var bizProcessesAudits = CurrentDb.BizProcessesAudit.Where(m => m.AduitType == type && m.AduitReferenceId == referenceid).OrderByDescending(m=>m.CreateTime).ToList();
            if (bizProcessesAudits != null)
            {
                foreach (var bizProcessesAudit in bizProcessesAudits)
                {
                    var bizProcessesAuditDetails = CurrentDb.BizProcessesAuditDetails.Where(m => m.BizProcessesAuditId == bizProcessesAudit.Id).OrderByDescending(m => m.CreateTime).ToList();
                    if (bizProcessesAuditDetails != null)
                    {
                        foreach (var bizProcessesAuditDetail in bizProcessesAuditDetails)
                        {
                            auditDetails.Add(bizProcessesAuditDetail);
                        }
                    }
                }
            }

            return auditDetails;

        }

        public BizProcessesAudit ChangeCarInsureOfferDealtStatus(int operater, int bizProcessesAuditId, Enumeration.CarInsureOfferDealtStatus changestatus, string description = null, DateTime? endTime = null)
        {

            var bizProcessesAudit = CurrentDb.BizProcessesAudit.Where(m => m.Id == bizProcessesAuditId).FirstOrDefault();
            if (bizProcessesAudit != null)
            {
                if (bizProcessesAudit.EndTime == null)
                {
                    if (
                       bizProcessesAudit.Status != (int)Enumeration.CarInsureOfferDealtStatus.Complete
                        && bizProcessesAudit.Status != (int)Enumeration.CarInsureOfferDealtStatus.ClientCancle
                           && bizProcessesAudit.Status != (int)Enumeration.CarInsureOfferDealtStatus.StaffCancle
                        )
                    {
                        Enumeration.CarInsureOfferDealtStatus old_Status = (Enumeration.CarInsureOfferDealtStatus)bizProcessesAudit.Status;
                        bizProcessesAudit.Mender = operater;
                        bizProcessesAudit.LastUpdateTime = DateTime.Now;


                        if (endTime != null)
                        {
                            bizProcessesAudit.EndTime = endTime.Value;
                        }

                        if (changestatus == Enumeration.CarInsureOfferDealtStatus.WaitOffer)
                        {

                            var bizProcessesAuditDetails = CurrentDb.BizProcessesAuditDetails.Where(m => m.BizProcessesAuditId == bizProcessesAudit.Id && m.AuditStep == (int)Enumeration.CarInsureOfferDealtStep.Offer).OrderByDescending(m => m.CreateTime).Take(1).FirstOrDefault();
                            if (bizProcessesAuditDetails == null)
                            {
                                bizProcessesAudit.Status = (int)Enumeration.CarInsureOfferDealtStatus.WaitOffer;
                                bizProcessesAudit.Auditor = null;

                            }
                            else
                            {
                                bizProcessesAudit.Status = (int)Enumeration.CarInsureOfferDealtStatus.InOffer;
                                bizProcessesAudit.Auditor = bizProcessesAuditDetails.Auditor;

                                ChangeAuditDetails(Enumeration.OperateType.Save, Enumeration.CarInsureOfferDealtStep.Offer, bizProcessesAudit.Id, bizProcessesAuditDetails.Auditor.Value, null, "后台人员正在处理");
                            }

                        }
                        else if (changestatus == Enumeration.CarInsureOfferDealtStatus.InOffer)
                        {
                            bizProcessesAudit.Status = (int)Enumeration.CarInsureOfferDealtStatus.InOffer;
                            if (bizProcessesAudit.Auditor == null)
                            {
                                bizProcessesAudit.Auditor = operater;

                                ChangeAuditDetails(Enumeration.OperateType.Save, Enumeration.CarInsureOfferDealtStep.Offer, bizProcessesAudit.Id, operater, null, null);

                            }

                        }
                        else if (changestatus == Enumeration.CarInsureOfferDealtStatus.ClientFllow)
                        {

                            var bizProcessesAuditDetails = CurrentDb.BizProcessesAuditDetails.Where(m => m.BizProcessesAuditId == bizProcessesAudit.Id && (m.AuditStep == (int)Enumeration.CarInsureOfferDealtStep.Submit || m.AuditStep == (int)Enumeration.CarInsureOfferDealtStep.Fllow)).OrderByDescending(m => m.CreateTime).Take(1).FirstOrDefault();
                            if (bizProcessesAuditDetails != null)
                            {
                                bizProcessesAudit.Status = (int)Enumeration.CarInsureOfferDealtStatus.ClientFllow;
                                bizProcessesAudit.Auditor = bizProcessesAuditDetails.Auditor;

                                ChangeAuditDetails(Enumeration.OperateType.Save, Enumeration.CarInsureOfferDealtStep.Fllow, bizProcessesAudit.Id, bizProcessesAuditDetails.Auditor.Value, null, description);
                            }



                        }
                        else if (changestatus == Enumeration.CarInsureOfferDealtStatus.ClientCancle)
                        {
                            bizProcessesAudit.Status = (int)Enumeration.CarInsureOfferDealtStatus.ClientCancle;
                            bizProcessesAudit.Auditor = operater;

                            ChangeAuditDetails(Enumeration.OperateType.Cancle, Enumeration.CarInsureOfferDealtStep.Offer, bizProcessesAudit.Id, bizProcessesAudit.Auditor.Value, null, null);

                        }
                        else if (changestatus == Enumeration.CarInsureOfferDealtStatus.StaffCancle)
                        {
                            bizProcessesAudit.Status = (int)Enumeration.CarInsureOfferDealtStatus.StaffCancle;
                            bizProcessesAudit.Auditor = operater;
                            bizProcessesAudit.EndTime = this.DateTime;

                            ChangeAuditDetails(Enumeration.OperateType.Cancle, Enumeration.CarInsureOfferDealtStep.Offer, bizProcessesAudit.Id, bizProcessesAudit.Auditor.Value, null, null);
                        }
                        else if (changestatus == Enumeration.CarInsureOfferDealtStatus.Complete)
                        {
                            bizProcessesAudit.Status = (int)Enumeration.CarInsureOfferDealtStatus.Complete;
                            bizProcessesAudit.Auditor = operater;
                            bizProcessesAudit.EndTime = this.DateTime;

                            ChangeAuditDetails(Enumeration.OperateType.Submit, Enumeration.CarInsureOfferDealtStep.Offer, bizProcessesAudit.Id, bizProcessesAudit.Auditor.Value, null, null);

                        }
                    }

                    CurrentDb.SaveChanges();
                }

                var historicalDetails = CurrentDb.BizProcessesAuditDetails.Where(m => m.BizProcessesAuditId == bizProcessesAudit.Id).OrderByDescending(m => m.AuditTime).ToList();

                bizProcessesAudit.HistoricalDetails = historicalDetails.Where(m => m.AuditTime != null).ToList();


                Enumeration.CarInsureOfferDealtStep merchantAuditStep = Enumeration.CarInsureOfferDealtStep.Unknow;
                if (changestatus == Enumeration.CarInsureOfferDealtStatus.WaitOffer || changestatus == Enumeration.CarInsureOfferDealtStatus.InOffer)
                {
                    merchantAuditStep = Enumeration.CarInsureOfferDealtStep.Offer;
                }

                var currentDetails = historicalDetails.Where(m => m.BizProcessesAuditId == bizProcessesAudit.Id && m.AuditStep == (int)merchantAuditStep).OrderByDescending(m => m.CreateTime).Take(1).FirstOrDefault();
                if (currentDetails != null)
                {
                    bizProcessesAudit.CurrentDetails = currentDetails;

                    var auditComments = historicalDetails.Where(m => m.BizProcessesAuditId == bizProcessesAudit.Id && m.AuditStep == (int)merchantAuditStep && m.AuditComments != null).OrderByDescending(m => m.CreateTime).Take(1).FirstOrDefault();
                    if (auditComments != null)
                    {
                        bizProcessesAudit.CurrentDetails.AuditComments = auditComments.AuditComments;
                    }
                }


            }

            return bizProcessesAudit;
        }

        public BizProcessesAudit ChangeCarClaimDealtStatus(int operater, int bizProcessesAuditId, Enumeration.CarClaimDealtStatus changestatus, string description = null, DateTime? endTime = null)
        {

            var bizProcessesAudit = CurrentDb.BizProcessesAudit.Where(m => m.Id == bizProcessesAuditId).FirstOrDefault();
            if (bizProcessesAudit != null)
            {
                if (bizProcessesAudit.EndTime == null)
                {
                    if (
                       bizProcessesAudit.Status != (int)Enumeration.CarClaimDealtStatus.Complete
                        && bizProcessesAudit.Status != (int)Enumeration.CarClaimDealtStatus.ClientCancle
                           && bizProcessesAudit.Status != (int)Enumeration.CarClaimDealtStatus.StaffCancle
                        )
                    {
                        Enumeration.CarClaimDealtStatus old_Status = (Enumeration.CarClaimDealtStatus)bizProcessesAudit.Status;
                        bizProcessesAudit.Mender = operater;
                        bizProcessesAudit.LastUpdateTime = DateTime.Now;


                        if (endTime != null)
                        {
                            bizProcessesAudit.EndTime = endTime.Value;
                        }

                        if (changestatus == Enumeration.CarClaimDealtStatus.WaitVerifyOrder)
                        {

                            var bizProcessesAuditDetails = CurrentDb.BizProcessesAuditDetails.Where(m => m.BizProcessesAuditId == bizProcessesAudit.Id && m.AuditStep == (int)Enumeration.CarInsureOfferDealtStep.Offer).OrderByDescending(m => m.CreateTime).Take(1).FirstOrDefault();
                            if (bizProcessesAuditDetails == null)
                            {
                                bizProcessesAudit.Status = (int)Enumeration.CarClaimDealtStatus.WaitVerifyOrder;
                                bizProcessesAudit.Auditor = null;

                            }
                            else
                            {
                                bizProcessesAudit.Status = (int)Enumeration.CarClaimDealtStatus.InVerifyOrder;
                                bizProcessesAudit.Auditor = bizProcessesAuditDetails.Auditor;

                                ChangeAuditDetails(Enumeration.OperateType.Save, Enumeration.CarClaimDealtStep.VerifyOrder, bizProcessesAudit.Id, operater, null, null);
                            }

                        }
                        else if (changestatus == Enumeration.CarClaimDealtStatus.InVerifyOrder)
                        {
                            bizProcessesAudit.Status = (int)Enumeration.CarClaimDealtStatus.InVerifyOrder;
                            if (bizProcessesAudit.Auditor == null)
                            {
                                bizProcessesAudit.Auditor = operater;

                                ChangeAuditDetails(Enumeration.OperateType.Save, Enumeration.CarClaimDealtStep.VerifyOrder, bizProcessesAudit.Id, operater, null, null);

                            }

                        }
                        else if (changestatus == Enumeration.CarClaimDealtStatus.FllowUploadEstimateListImg)
                        {
                            var bizProcessesAuditDetails = CurrentDb.BizProcessesAuditDetails.Where(m => m.BizProcessesAuditId == bizProcessesAudit.Id && (m.AuditStep == (int)Enumeration.CarClaimDealtStep.Submit || m.AuditStep == (int)Enumeration.CarClaimDealtStep.UploadEstimateListImg)).OrderByDescending(m => m.CreateTime).Take(1).FirstOrDefault();
                            if (bizProcessesAuditDetails != null)
                            {
                                bizProcessesAudit.Status = (int)Enumeration.CarClaimDealtStatus.FllowUploadEstimateListImg;
                                bizProcessesAudit.Auditor = bizProcessesAuditDetails.Auditor;

                                ChangeAuditDetails(Enumeration.OperateType.Save, Enumeration.CarClaimDealtStep.UploadEstimateListImg, bizProcessesAudit.Id, bizProcessesAuditDetails.Auditor.Value, null, description);
                            }

                        }
                        else if (changestatus == Enumeration.CarClaimDealtStatus.WaitVerifyAmount)
                        {
                            bizProcessesAudit.Status = (int)Enumeration.CarClaimDealtStatus.WaitVerifyAmount;
                            bizProcessesAudit.Auditor = null;

                        }
                        else if (changestatus == Enumeration.CarClaimDealtStatus.InVerifyAmount)
                        {
                            bizProcessesAudit.Status = (int)Enumeration.CarClaimDealtStatus.InVerifyAmount;
                            if (bizProcessesAudit.Auditor == null)
                            {
                                bizProcessesAudit.Auditor = operater;

                                ChangeAuditDetails(Enumeration.OperateType.Save, Enumeration.CarClaimDealtStep.VerifyAmount, bizProcessesAudit.Id, operater, null, null);

                            }

                        }
                        else if (changestatus == Enumeration.CarClaimDealtStatus.ClientCancle)
                        {
                            bizProcessesAudit.Status = (int)Enumeration.CarClaimDealtStatus.ClientCancle;
                            bizProcessesAudit.Auditor = operater;

                            ChangeAuditDetails(Enumeration.OperateType.Cancle, Enumeration.CarClaimDealtStep.VerifyOrder, bizProcessesAudit.Id, bizProcessesAudit.Auditor.Value, null, null);

                        }
                        else if (changestatus == Enumeration.CarClaimDealtStatus.StaffCancle)
                        {
                            bizProcessesAudit.Status = (int)Enumeration.CarClaimDealtStatus.StaffCancle;
                            bizProcessesAudit.Auditor = operater;
                            bizProcessesAudit.EndTime = this.DateTime;

                            ChangeAuditDetails(Enumeration.OperateType.Cancle, Enumeration.CarClaimDealtStep.VerifyOrder, bizProcessesAudit.Id, bizProcessesAudit.Auditor.Value, null, null);
                        }
                        else if (changestatus == Enumeration.CarClaimDealtStatus.Complete)
                        {
                            bizProcessesAudit.Status = (int)Enumeration.CarClaimDealtStatus.Complete;
                            bizProcessesAudit.Auditor = operater;
                            bizProcessesAudit.EndTime = this.DateTime;

                            ChangeAuditDetails(Enumeration.OperateType.Submit, Enumeration.CarClaimDealtStep.VerifyOrder, bizProcessesAudit.Id, bizProcessesAudit.Auditor.Value, null, null);

                        }
                    }

                    CurrentDb.SaveChanges();
                }


                var historicalDetails = CurrentDb.BizProcessesAuditDetails.Where(m => m.BizProcessesAuditId == bizProcessesAudit.Id).OrderByDescending(m => m.AuditTime).ToList();

                bizProcessesAudit.HistoricalDetails = historicalDetails.Where(m => m.AuditTime != null).ToList();


                Enumeration.CarClaimDealtStep merchantAuditStep = Enumeration.CarClaimDealtStep.Unknow;
                if (changestatus == Enumeration.CarClaimDealtStatus.WaitVerifyOrder || changestatus == Enumeration.CarClaimDealtStatus.InVerifyOrder)
                {
                    merchantAuditStep = Enumeration.CarClaimDealtStep.VerifyOrder;
                }
                else if (changestatus == Enumeration.CarClaimDealtStatus.WaitVerifyAmount || changestatus == Enumeration.CarClaimDealtStatus.InVerifyAmount)
                {
                    merchantAuditStep = Enumeration.CarClaimDealtStep.VerifyAmount;
                }
                var currentDetails = historicalDetails.Where(m => m.BizProcessesAuditId == bizProcessesAudit.Id && m.AuditStep == (int)merchantAuditStep).OrderByDescending(m => m.CreateTime).Take(1).FirstOrDefault();
                if (currentDetails != null)
                {
                    bizProcessesAudit.CurrentDetails = currentDetails;

                    var auditComments = historicalDetails.Where(m => m.BizProcessesAuditId == bizProcessesAudit.Id && m.AuditStep == (int)merchantAuditStep && m.AuditComments != null).OrderByDescending(m => m.CreateTime).Take(1).FirstOrDefault();
                    if (auditComments != null)
                    {
                        bizProcessesAudit.CurrentDetails.AuditComments = auditComments.AuditComments;
                    }
                }


            }

            return bizProcessesAudit;
        }

        public void ChangeAuditDetails(Enumeration.OperateType operate, object auditStep, int bizProcessesAuditId, int operater, string auditComments, string description, DateTime? auditTime = null)
        {
            DateTime nowDate = DateTime.Now;
            var bizProcessesAudit = CurrentDb.BizProcessesAudit.Where(m => m.Id == bizProcessesAuditId).FirstOrDefault();
            if (bizProcessesAudit != null)
            {
                if (bizProcessesAudit.EndTime == null)
                {
                    var detailsHistory = CurrentDb.BizProcessesAuditDetails.Where(m => m.AuditStep == (int)auditStep && m.BizProcessesAuditId == bizProcessesAudit.Id).OrderByDescending(m => m.CreateTime).Take(1).FirstOrDefault();
                    if (detailsHistory == null)
                    {
                        detailsHistory = new BizProcessesAuditDetails();
                        detailsHistory.AuditStep = (int)auditStep;
                        string suditStepenumName = ""+ auditStep.GetType().FullName + ", Lumos.Entity";
                        detailsHistory.AuditStepEnumName = suditStepenumName;
                        detailsHistory.BizProcessesAuditId = bizProcessesAuditId;
                        detailsHistory.Auditor = operater;
                        detailsHistory.AuditComments = auditComments;
                        detailsHistory.Description = description;
                        if (operate != Enumeration.OperateType.Save)
                        {
                            detailsHistory.AuditTime = nowDate;
                            detailsHistory.EndTime = nowDate;
                        }
                        detailsHistory.StartTime = nowDate;
                        detailsHistory.CreateTime = nowDate;
                        detailsHistory.Creator = operater;
                        CurrentDb.BizProcessesAuditDetails.Add(detailsHistory);
                    }
                    else
                    {
                        if (detailsHistory.AuditTime == null)
                        {
                            detailsHistory.Auditor = operater;
                            detailsHistory.AuditComments = auditComments;
                            detailsHistory.Description = description;
                            if (operate != Enumeration.OperateType.Save)
                            {
                                detailsHistory.AuditTime = nowDate;
                                detailsHistory.EndTime = nowDate;
                            }
                            detailsHistory.LastUpdateTime = nowDate;
                            detailsHistory.Mender = operater;
                        }
                        else
                        {
                            detailsHistory = new BizProcessesAuditDetails();
                            detailsHistory.AuditStep = (int)auditStep;
                            string suditStepenumName = "" + auditStep.GetType().FullName + ", Lumos.Entity";
                            detailsHistory.AuditStepEnumName = suditStepenumName;
                            detailsHistory.BizProcessesAuditId = bizProcessesAuditId;
                            detailsHistory.Auditor = operater;
                            detailsHistory.AuditComments = auditComments;
                            detailsHistory.Description = description;
                            if (operate != Enumeration.OperateType.Save)
                            {
                                detailsHistory.AuditTime = nowDate;
                                detailsHistory.EndTime = nowDate;
                            }
                            detailsHistory.StartTime = nowDate;
                            detailsHistory.CreateTime = nowDate;
                            detailsHistory.Creator = operater;
                            CurrentDb.BizProcessesAuditDetails.Add(detailsHistory);
                        }
                    }


                    CurrentDb.SaveChanges();
                }
            }

        }

        public void ChangeAuditDetailsAuditComments(int operater, int bizProcessesAuditDetailsId, string auditComments, string description, DateTime? auditTime = null)
        {
            DateTime nowDate = DateTime.Now;
            var detailsHistory = CurrentDb.BizProcessesAuditDetails.Where(m => m.Id == bizProcessesAuditDetailsId).FirstOrDefault();
            if (detailsHistory != null)
            {
                if (detailsHistory.Auditor == null)
                {
                    detailsHistory.Auditor = operater;
                    detailsHistory.LastUpdateTime = nowDate;
                }

                if (auditTime != null)
                {
                    detailsHistory.AuditTime = auditTime;
                    detailsHistory.EndTime = auditTime;
                    detailsHistory.LastUpdateTime = auditTime;
                }

                detailsHistory.AuditComments = auditComments;
                detailsHistory.Description = description;
                detailsHistory.Mender = operater;

            }

            CurrentDb.SaveChanges();
        }


        public BizProcessesAudit ChangeCommissionRateAuditStatus(int operater, int bizProcessesAuditId, Enumeration.CommissionRateAuditStatus changestatus, string description = null, DateTime? endTime = null)
        {

            var bizProcessesAudit = CurrentDb.BizProcessesAudit.Where(m => m.Id == bizProcessesAuditId).FirstOrDefault();
            if (bizProcessesAudit != null)
            {
                if (bizProcessesAudit.EndTime == null)
                {

                    Enumeration.CommissionRateAuditStatus old_Status = (Enumeration.CommissionRateAuditStatus)bizProcessesAudit.Status;
                    bizProcessesAudit.Mender = operater;
                    bizProcessesAudit.LastUpdateTime = DateTime.Now;


                    if (endTime != null)
                    {
                        bizProcessesAudit.EndTime = endTime.Value;
                    }

                    if (changestatus == Enumeration.CommissionRateAuditStatus.InPrimaryAudit)
                    {
                        bizProcessesAudit.Status = (int)Enumeration.MerchantAuditStatus.InPrimaryAudit;

                        if (bizProcessesAudit.Auditor == null)
                        {
                            bizProcessesAudit.Auditor = operater;

                            ChangeAuditDetails(Enumeration.OperateType.Save, Enumeration.CommissionRateAuditStep.PrimaryAudit, bizProcessesAudit.Id, operater, null, description, null);
                        }
                    }
                    else if (changestatus == Enumeration.CommissionRateAuditStatus.WaitSeniorAudit)
                    {

                        var bizProcessesAuditDetails = CurrentDb.BizProcessesAuditDetails.Where(m => m.BizProcessesAuditId == bizProcessesAudit.Id && m.AuditStep == (int)Enumeration.CommissionRateAuditStep.SeniorAudit).OrderByDescending(m => m.CreateTime).Take(1).FirstOrDefault();
                        if (bizProcessesAuditDetails == null)
                        {
                            bizProcessesAudit.Status = (int)Enumeration.CommissionRateAuditStatus.WaitSeniorAudit;
                            bizProcessesAudit.Auditor = null;
                        }
                        else
                        {
                            bizProcessesAudit.Status = (int)Enumeration.CommissionRateAuditStatus.InSeniorAudit;
                            bizProcessesAudit.Auditor = bizProcessesAuditDetails.Auditor;

                            ChangeAuditDetails(Enumeration.OperateType.Save, Enumeration.CommissionRateAuditStep.SeniorAudit, bizProcessesAudit.Id, operater, null, null);
                        }
                    }
                    else if (changestatus == Enumeration.CommissionRateAuditStatus.InSeniorAudit)
                    {
                        bizProcessesAudit.Status = (int)Enumeration.CommissionRateAuditStatus.InSeniorAudit;
                        if (bizProcessesAudit.Auditor == null)
                        {
                            bizProcessesAudit.Auditor = operater;
                            ChangeAuditDetails(Enumeration.OperateType.Save, Enumeration.CommissionRateAuditStep.SeniorAudit, bizProcessesAudit.Id, operater, null, description);
                        }
                    }
                    else if (changestatus == Enumeration.CommissionRateAuditStatus.SeniorAuditReject)
                    {
                        var bizProcessesAuditDetails = CurrentDb.BizProcessesAuditDetails.Where(m => m.BizProcessesAuditId == bizProcessesAudit.Id && m.AuditStep == (int)Enumeration.MerchantAuditStep.PrimaryAudit).OrderByDescending(m => m.CreateTime).Take(1).FirstOrDefault();
                        if (bizProcessesAuditDetails != null)
                        {
                            bizProcessesAudit.Status = (int)Enumeration.CommissionRateAuditStatus.InPrimaryAudit;
                            bizProcessesAudit.Auditor = bizProcessesAuditDetails.Auditor;

                            ChangeAuditDetails(Enumeration.OperateType.Save, Enumeration.CommissionRateAuditStep.PrimaryAudit, bizProcessesAudit.Id, bizProcessesAuditDetails.Auditor.Value, null, description);
                        }
                    }
                    else if (changestatus == Enumeration.CommissionRateAuditStatus.SeniorAuditRefuse)
                    {
                        bizProcessesAudit.Status = (int)Enumeration.CommissionRateAuditStatus.SeniorAuditRefuse;
                        bizProcessesAudit.Auditor = operater;
                    }
                    else if (changestatus == Enumeration.CommissionRateAuditStatus.SeniorAuditPass)
                    {
                        bizProcessesAudit.Status = (int)Enumeration.CommissionRateAuditStatus.SeniorAuditPass;
                        bizProcessesAudit.Auditor = operater;
                    }



                    CurrentDb.SaveChanges();
                }

                var historicalDetails = CurrentDb.BizProcessesAuditDetails.Where(m => m.BizProcessesAuditId == bizProcessesAudit.Id).OrderByDescending(m => m.AuditTime).ToList();

                bizProcessesAudit.HistoricalDetails = historicalDetails.Where(m => m.AuditTime != null).ToList();


                Enumeration.CommissionRateAuditStep merchantAuditStep = Enumeration.CommissionRateAuditStep.Unknow;
                if (changestatus == Enumeration.CommissionRateAuditStatus.WaitPrimaryAudit || changestatus == Enumeration.CommissionRateAuditStatus.InPrimaryAudit)
                {
                    merchantAuditStep = Enumeration.CommissionRateAuditStep.PrimaryAudit;
                }
                else if (changestatus == Enumeration.CommissionRateAuditStatus.WaitSeniorAudit || changestatus == Enumeration.CommissionRateAuditStatus.InSeniorAudit)
                {
                    merchantAuditStep = Enumeration.CommissionRateAuditStep.SeniorAudit;
                }

                var currentDetails = historicalDetails.Where(m => m.BizProcessesAuditId == bizProcessesAudit.Id && m.AuditStep == (int)merchantAuditStep).OrderByDescending(m => m.CreateTime).Take(1).FirstOrDefault();
                if (currentDetails != null)
                {
                    bizProcessesAudit.CurrentDetails = currentDetails;

                    var auditComments = historicalDetails.Where(m => m.BizProcessesAuditId == bizProcessesAudit.Id && m.AuditStep == (int)merchantAuditStep && m.AuditComments != null).OrderByDescending(m => m.CreateTime).Take(1).FirstOrDefault();
                    if (auditComments != null)
                    {
                        bizProcessesAudit.CurrentDetails.AuditComments = auditComments.AuditComments;
                    }
                }

            }

            return bizProcessesAudit;
        }


    }
}
