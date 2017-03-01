using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAppApi.Models.Banner;
using Lumos.Entity;
using Lumos.Mvc;

namespace WebAppApi.Controllers
{
    public class BannerController:BaseApiController
    {
        [HttpGet]
        public APIResponse GetList(int userId,Enumeration.BannerType type)
        {
            var banner = CurrentDb.SysBanner.Where(m => m.Type == type).ToList();
            if (banner == null)
            {
                return ResponseResult(ResultType.Failure, ResultCode.FailureNoData, "没有数据");
            }

            List<BannerImageModel> model = new List<BannerImageModel>();

            foreach (var m in banner)
            {

                BannerImageModel imageModel = new BannerImageModel();
                imageModel.Id = m.Id;
                imageModel.Title = m.Title;
                imageModel.LinkUrl = "www.baidu.com";
                imageModel.ImgUrl = m.ImgUrl;
                model.Add(imageModel);
            }

            APIResult result = new APIResult() { Result = ResultType.Success, Code = ResultCode.Success, Message = "成功", Data = model };
            return new APIResponse(result);
        }

        //[HttpGet]
        //public APIResponse GetDetails(int id)
        //{
        //    var bannerImage = CurrentDb.SysBannerImage.Where(m => m.Id == id).FirstOrDefault();
        //    if (bannerImage == null)
        //    {
        //        return ResponseResult(ResultType.Failure, ResultCode.FailureNoData, "没有数据");
        //    }

        //    BannerImageModel model = new BannerImageModel();
        //    model.Id = bannerImage.Id;
        //    model.Title = bannerImage.Title;
        //    model.ReadCount = bannerImage.ReadCount;
        //    model.ImgUrl = bannerImage.ImgUrl;

        //    APIResult result = new APIResult() { Result = ResultType.Success, Code = ResultCode.Success, Message = "成功", Data= model };
        //    return new APIResponse(result);
        //}
    }
}