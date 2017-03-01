using AnXinSdk;
using log4net;
using Lumos.BLL;
using Lumos.Common;
using Lumos.Entity;
using Lumos.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppApi.Models.CarService;

namespace WebAppApi.Controllers
{
    public class CarServiceController : BaseApiController
    {

        //获取投保方案
        public APIResponse GetInsurePlan(int userId)
        {

            var carInsurePlans = CurrentDb.CarInsurePlan.OrderByDescending(m => m.Priority).ToList();

            List<InsurePlanModel> model = new List<InsurePlanModel>();

            foreach (var m in carInsurePlans)
            {
                InsurePlanModel insurePlanModel = new InsurePlanModel();
                insurePlanModel.Id = m.Id;
                insurePlanModel.Name = m.Name;
                insurePlanModel.ImgUrl = m.ImgUrl;
                model.Add(insurePlanModel);
            }


            APIResult result = new APIResult() { Result = ResultType.Success, Code = ResultCode.Success, Message = "", Data = model };

            return new APIResponse(result);
        }

        //获取投保方案明细
        public APIResponse GetInsurePlanKind(int userId, int insurePlanId)
        {
            var carInsurePlans = CurrentDb.CarInsurePlanKind.Where(m => m.CarInsurePlanId == insurePlanId).ToList();

            List<InsurePlanKindModel> model = new List<InsurePlanKindModel>();

            foreach (var m in carInsurePlans)
            {
                var carKind = CurrentDb.CarKind.Where(c => c.Id == m.CarKindId).FirstOrDefault();
                InsurePlanKindModel insurePlanKindModel = new InsurePlanKindModel();
                insurePlanKindModel.Id = carKind.Id;
                insurePlanKindModel.Name = carKind.Name;
                insurePlanKindModel.AliasName = carKind.AliasName;
                insurePlanKindModel.Type = carKind.Type;
                insurePlanKindModel.CanWaiverDeductible = carKind.CanWaiverDeductible;
                insurePlanKindModel.InputType = carKind.InputType;
                insurePlanKindModel.InputUnit = carKind.InputUnit;
                if (!string.IsNullOrEmpty(carKind.InputValue))
                {
                    insurePlanKindModel.InputValue = Newtonsoft.Json.JsonConvert.DeserializeObject(carKind.InputValue);
                }
                insurePlanKindModel.IsHasDetails = carKind.IsHasDetails;
                model.Add(insurePlanKindModel);
            }


            APIResult result = new APIResult() { Result = ResultType.Success, Code = ResultCode.Success, Message = "", Data = model };

            return new APIResponse(result);
        }

        //获取投保保险公司
        public APIResponse GetInsuranceCompany(int userId)
        {
            var carInsuranceCompanys = (from u in CurrentDb.CarInsuranceCompany
                                        join r in CurrentDb.InsuranceCompany on u.InsuranceCompanyId equals r.Id
                                        where u.Status == Enumeration.CarInsuranceCompanyStatus.Normal
                                        select new { r.Id, r.Name, u.InsuranceCompanyImgUrl, u.Priority }).Distinct().OrderByDescending(m => m.Priority);


            InsuranceCompanyResult model = new InsuranceCompanyResult();

            model.CanInsureCount = 2;

            List<InsuranceCompanyModel> insuranceCompanyModels = new List<InsuranceCompanyModel>();

            foreach (var m in carInsuranceCompanys)
            {
                InsuranceCompanyModel insuranceCompanyModel = new InsuranceCompanyModel();
                insuranceCompanyModel.Id = m.Id;
                insuranceCompanyModel.Name = m.Name;
                insuranceCompanyModel.ImgUrl = m.InsuranceCompanyImgUrl;
                insuranceCompanyModels.Add(insuranceCompanyModel);
            }

            model.InsuranceCompany = insuranceCompanyModels;

            APIResult result = new APIResult() { Result = ResultType.Success, Code = ResultCode.Success, Message = "", Data = model };

            return new APIResponse(result);
        }

        //提交投保单
        [HttpPost]
        public APIResponse SubmitInsure(SubmitInsureModel model)
        {
            //暂时不启用这段代码
            //var disabledInsuranceCompany = CurrentDb.CarInsuranceCompany.Where(m => model.InsuranceCompanyId.Contains(m.InsuranceCompanyId) && m.Status != Enumeration.CarInsuranceCompanyStatus.Normal).ToList();
            //if (disabledInsuranceCompany.Count > 0)
            //{
            //    string disabledInsuranceCompanyNames = string.Join(",", disabledInsuranceCompany.Select(m => m.InsuranceCompanyName).ToArray());
            //    return ResponseResult(ResultType.Failure, ResultCode.Failure, string.Format("{0}已经被停用", disabledInsuranceCompanyNames));
            //}


            OrderToCarInsure orderToCarInsure = new OrderToCarInsure();

            orderToCarInsure.MerchantId = model.MerchantId;
            orderToCarInsure.PosMachineId = model.PosMachineId;
            orderToCarInsure.InsurePlanId = model.InsurePlanId;
            orderToCarInsure.ProductType = model.Type;


            if (model.ImgData != null)
            {
                var key_CZ_CL_XSZ_Img = "CZ_CL_XSZ_Img";
                if (model.ImgData.ContainsKey(key_CZ_CL_XSZ_Img))
                {
                    orderToCarInsure.CZ_CL_XSZ_ImgUrl = GetUploadImageUrl(model.ImgData[key_CZ_CL_XSZ_Img], "CarInsure");

                    DrivingLicenceVO drivingLicence = AnXin.GetDrivingLicenceByImageBase64(model.ImgData[key_CZ_CL_XSZ_Img].Data);

                    if (drivingLicence != null)
                    {
                        orderToCarInsure.CarOwner = drivingLicence.owner;
                        orderToCarInsure.CarOwnerAddress = drivingLicence.address;
                        orderToCarInsure.CarModel = drivingLicence.model;
                        orderToCarInsure.CarPlateNo = drivingLicence.plateNum;
                        orderToCarInsure.CarEngineNo = drivingLicence.engineNo;
                        orderToCarInsure.CarVin = drivingLicence.vin;
                        orderToCarInsure.CarVechicheType = drivingLicence.vehicleType;
                        orderToCarInsure.CarIssueDate = drivingLicence.issueDate;
                        orderToCarInsure.CarUserCharacter = drivingLicence.userCharacter;
                        orderToCarInsure.CarRegisterDate = drivingLicence.registerDate;
                    }

                }

                var key_CZ_SFZ_Img = "CZ_SFZ_Img";
                if (model.ImgData.ContainsKey(key_CZ_SFZ_Img))
                {
                    orderToCarInsure.CZ_SFZ_ImgUrl = GetUploadImageUrl(model.ImgData[key_CZ_SFZ_Img], "CarInsure");

                    IdentityCardVO identityCardVO = AnXin.GetIdentityCardByImageBase64(model.ImgData[key_CZ_SFZ_Img].Data);

                    if (identityCardVO != null)
                    {
                        orderToCarInsure.CarOwnerIdNumber = identityCardVO.idNumber;
                    }

                }

                var key_CCSJM_WSZM_Img = "CCSJM_WSZM_Img";
                if (model.ImgData.ContainsKey(key_CCSJM_WSZM_Img))
                {
                    orderToCarInsure.CCSJM_WSZM_ImgUrl = GetUploadImageUrl(model.ImgData[key_CCSJM_WSZM_Img], "CarInsure");
                }

                var key_YCZ_CLDJZ_Img = "YCZ_CLDJZ_Img";
                if (model.ImgData.ContainsKey(key_YCZ_CLDJZ_Img))
                {
                    orderToCarInsure.YCZ_CLDJZ_ImgUrl = GetUploadImageUrl(model.ImgData[key_YCZ_CLDJZ_Img], "CarInsure");
                }
            }

            orderToCarInsure.ClientRequire = model.ClientRequire;


            if (model.RenewalId != 0)
            {
                var renewal = CurrentDb.OrderToCarInsure.Where(m => m.Id == model.RenewalId).FirstOrDefault();
                if (renewal != null)
                {
                    orderToCarInsure.CarOwner = renewal.CarOwner;
                    orderToCarInsure.CarOwnerIdNumber = renewal.CarOwnerIdNumber;
                    orderToCarInsure.CarOwnerAddress = renewal.CarOwnerAddress;
                    orderToCarInsure.CarModel = renewal.CarModel;
                    orderToCarInsure.CarPlateNo = renewal.CarPlateNo;
                    orderToCarInsure.CarEngineNo = renewal.CarEngineNo;
                    orderToCarInsure.CarVin = renewal.CarVin;
                    orderToCarInsure.CarVechicheType = renewal.CarVechicheType;
                    orderToCarInsure.CarIssueDate = renewal.CarIssueDate;
                    orderToCarInsure.CarUserCharacter = renewal.CarUserCharacter;
                    orderToCarInsure.CarRegisterDate = renewal.CarRegisterDate;
                    orderToCarInsure.CarIssueDate = renewal.CarIssueDate;
                    orderToCarInsure.CarSeat = renewal.CarSeat;

                }
            }



            List<OrderToCarInsureOfferCompany> orderToCarInsureOfferCompanys = new List<OrderToCarInsureOfferCompany>();

            var insureOfferCompanys = CurrentDb.InsuranceCompany.ToList();
            foreach (var m in model.InsuranceCompanyId)
            {
                var insureOfferCompany = insureOfferCompanys.Where(q => q.Id == m).FirstOrDefault();
                OrderToCarInsureOfferCompany orderToCarInsureOfferCompany = new OrderToCarInsureOfferCompany();
                orderToCarInsureOfferCompany.InsuranceCompanyId = insureOfferCompany.Id;
                orderToCarInsureOfferCompany.InsuranceCompanyName = insureOfferCompany.Name;
                orderToCarInsureOfferCompanys.Add(orderToCarInsureOfferCompany);
            }

            if (orderToCarInsure.ProductType == Enumeration.ProductType.InsureForCarForRenewal)
            {
                var insureOfferCompany = insureOfferCompanys.Where(q => q.Id == model.InsuranceCompanyId[0]).FirstOrDefault();
                orderToCarInsure.InsuranceCompanyId = insureOfferCompany.Id;
                orderToCarInsure.InsuranceCompanyName = insureOfferCompany.Name;
            }


            List<OrderToCarInsureOfferKind> orderToCarInsureOfferKinds = new List<OrderToCarInsureOfferKind>();

            foreach (var m in model.InsureKind)
            {
                OrderToCarInsureOfferKind orderToCarInsureOfferKind = new OrderToCarInsureOfferKind();
                orderToCarInsureOfferKind.KindId = m.Id;
                orderToCarInsureOfferKind.KindValue = m.Value;
                orderToCarInsureOfferKind.KindDetails = m.Details;
                orderToCarInsureOfferKind.IsWaiverDeductible = m.IsWaiverDeductible;
                orderToCarInsureOfferKinds.Add(orderToCarInsureOfferKind);
            }

            IResult result = BizFactory.Order.SubmitCarInsure(model.UserId, model.UserId, orderToCarInsure, orderToCarInsureOfferCompanys, orderToCarInsureOfferKinds);



            return new APIResponse(result);

        }

        //提交跟进的投保单
        [HttpPost]
        public APIResponse SubmitFollowInsure(SubmitFollowInsureModel model)
        {
            OrderToCarInsure orderToCarInsure = new OrderToCarInsure();

            orderToCarInsure.Id = model.OrderId;

            if (model.ImgData != null)
            {
                var key_ZJ1_Img = "ZJ1_Img";
                if (model.ImgData.ContainsKey(key_ZJ1_Img))
                {
                    orderToCarInsure.ZJ1_ImgUrl = GetUploadImageUrl(model.ImgData[key_ZJ1_Img], "CarInsure");
                }

                var key_ZJ2_Img = "ZJ2_Img";
                if (model.ImgData.ContainsKey(key_ZJ2_Img))
                {
                    orderToCarInsure.ZJ2_ImgUrl = GetUploadImageUrl(model.ImgData[key_ZJ2_Img], "CarInsure");
                }

                var key_ZJ3_Img = "ZJ3_Img";
                if (model.ImgData.ContainsKey(key_ZJ3_Img))
                {
                    orderToCarInsure.ZJ3_ImgUrl = GetUploadImageUrl(model.ImgData[key_ZJ3_Img], "CarInsure");
                }


                var key_ZJ4_Img = "ZJ4_Img";
                if (model.ImgData.ContainsKey(key_ZJ4_Img))
                {
                    orderToCarInsure.ZJ4_ImgUrl = GetUploadImageUrl(model.ImgData[key_ZJ4_Img], "CarInsure");
                }

            }



            IResult result = BizFactory.Order.SubmitFollowInsure(model.UserId, orderToCarInsure);
            return new APIResponse(result);
        }

        //提交理赔需求
        [HttpPost]
        public APIResponse SubmitClaim(SubmitClaimModel model)
        {

            OrderToCarClaim orderToCarClaim = new OrderToCarClaim();
            orderToCarClaim.PosMachineId = model.PosMachineId;
            orderToCarClaim.RepairsType = model.RepairsType;
            orderToCarClaim.CarPlateNo = model.CarLicenseNumber;
            orderToCarClaim.HandPerson = model.HandPerson;
            orderToCarClaim.HandPersonPhone = model.HandPersonPhone;
            orderToCarClaim.InsuranceCompanyId = model.InsuranceCompanyId;
            orderToCarClaim.ClientRequire = model.ClientRequire;
            IResult result = BizFactory.Order.SubmitClaim(model.UserId, model.UserId, orderToCarClaim);

            return new APIResponse(result);
        }

        //提交定损单
        [HttpPost]
        public APIResponse SubmitEstimateList(SubmitEstimateListModel model)
        {

            string estimateListImg = "";
            if (model.ImgData != null)
            {
                var key_EstimateListImg = "estimateListImg";
                if (model.ImgData.ContainsKey(key_EstimateListImg))
                {
                    estimateListImg = GetUploadImageUrl(model.ImgData[key_EstimateListImg], "CarInsure");
                }
            }

            IResult result = BizFactory.Order.SubmitEstimateList(model.UserId, model.UserId, model.OrderId, estimateListImg);

            return new APIResponse(result);
        }


        public APIResponse GetRenewal(int userId, int merchantId, int pageIndex)
        {

            //DateTime periodEndMax = DateTime.Now.AddDays(-7);
            var query = (from m in CurrentDb.OrderToCarInsure
                         join n in CurrentDb.CarInsuranceCompany on m.InsuranceCompanyId equals n.InsuranceCompanyId
                         where m.MerchantId == merchantId && m.UserId == userId && m.Status == Enumeration.OrderStatus.Completed
                         select new { m.Id, m.CarOwner, m.InsuranceCompanyId, n.InsuranceCompanyName, n.InsuranceCompanyImgUrl, m.CarPlateNo, m.Price, m.PeriodEnd });

            int total = query.Count();

            int pageSize = 10;

            query = query.OrderByDescending(r => r.PeriodEnd).Skip(pageSize * (pageIndex)).Take(pageSize);


            List<object> list = new List<object>();

            foreach (var item in query)
            {
                list.Add(new
                {
                    Id = item.Id,
                    InsuranceCompanyId = item.InsuranceCompanyId,
                    InsuranceCompanyName = item.InsuranceCompanyName,
                    InsuranceCompanyImgUrl = item.InsuranceCompanyImgUrl,
                    CarOwner = item.CarOwner,
                    CarPlateNo = item.CarPlateNo,
                    Price = item.Price,
                    PeriodEnd = item.PeriodEnd,
                    PeriodTip = "5天后到期"
                });
            }

            APIResult result = new APIResult() { Result = ResultType.Success, Code = ResultCode.Success, Message = "获取成功", Data = list };
            return new APIResponse(result);

        }

    }
}