using Lumos.BLL;
using Lumos.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAppApi.Models.Product;

namespace WebAppApi.Controllers
{
    public class ProductController : BaseApiController
    {

        public APIResponse GetMoreRecommend(int userId, int merchantId, int pageIndex)
        {

            var query = (from m in CurrentDb.Product
                         where ((int)m.Type).ToString().StartsWith("101")
                         select new { m.Id, m.Name, m.Price, m.Type, m.CreateTime, m.MainImgUrl });

            int total = query.Count();

            int pageSize = 6;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);


            List<object> list = new List<object>();

            foreach (var item in query)
            {
                list.Add(new
                {
                    Id = item.Id,
                    Name = item.Name,
                    Type = item.Type.GetCnName(),
                    Price = item.Price.ToF2Price(),
                    ImgUrl = item.MainImgUrl,
                    LinkUrl = BizFactory.Product.GetLinkUrl(item.Type, item.Id)
                });
            }


            APIResult result = new APIResult() { Result = ResultType.Success, Code = ResultCode.Success, Message = "", Data = list };

            return new APIResponse(result);
        }


        public enum PriceArea
        {
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4
        }

        public APIResponse GetList(int userId, int merchantId, int type, PriceArea priceArea, int pageIndex)
        {

            var query = (from m in CurrentDb.Product
                         where ((int)m.Type).ToString().StartsWith(type.ToString())
                         select new { m.Id, m.Name, m.Price, m.Type, m.CreateTime, m.MainImgUrl });

            if (priceArea == PriceArea.One)
            {
                query = query.Where(m => m.Price <= 100);
            }
            else if (priceArea == PriceArea.Two)
            {
                query = query.Where(m => m.Price >= 100 && m.Price <= 500);
            }
            else if (priceArea == PriceArea.Three)
            {
                query = query.Where(m => m.Price >= 500 && m.Price <= 1000);
            }
            else if (priceArea == PriceArea.Four)
            {
                query = query.Where(m => m.Price >= 1000);
            }

            int total = query.Count();

            int pageSize = 10;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);



            List<object> list = new List<object>();

            foreach (var item in query)
            {
                list.Add(new
                {
                    Id = item.Id,
                    Name = item.Name,
                    Type = item.Type.GetCnName(),
                    Price = item.Price.ToF2Price(),
                    ImgUrl = item.MainImgUrl,
                    LinkUrl = BizFactory.Product.GetLinkUrl(item.Type, item.Id)
                });
            }


            APIResult result = new APIResult() { Result = ResultType.Success, Code = ResultCode.Success, Message = "", Data = list };

            return new APIResponse(result);
        }

    }
}