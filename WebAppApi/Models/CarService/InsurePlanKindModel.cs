﻿using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppApi.Models.CarService
{
    public class InsurePlanKindModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string AliasName { get; set; }

        public bool CanWaiverDeductible { get; set; }

        public Enumeration.CarKindType Type { get; set; }

        public Enumeration.CarKindInputType InputType { get; set; }

        public string InputUnit { get; set; }

        public object InputValue { get; set; }

        public bool IsHasDetails { get; set; }
    }
}