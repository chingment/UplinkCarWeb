﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppApi.Models.CarService
{
    public class SubmitEstimateListModel
    {
        public int UserId { get; set; }

        public int OrderId { get; set; }

        public Dictionary<string, ImageModel> ImgData { get; set; }

      //  public ImageModel EstimateListImg { get; set; }
    }
}