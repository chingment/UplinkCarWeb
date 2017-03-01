using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAppApi.Models.Banner;
using WebAppApi.Models.Product;

namespace WebAppApi.Models.Account
{
    public class HomeModel
    {
        public List<BannerImageModel> Banner { get; set; }

        public List<ExtendedAppModel> CarService { get; set; }

        public List<ExtendedAppModel> RecommendService { get; set; }

        public List<RecommendProductModel> RecommendProduct { get; set; }
    }
}