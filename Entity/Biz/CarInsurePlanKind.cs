﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.Entity
{
    [Table("CarInsurePlanKind")]
    public class CarInsurePlanKind
    {
        [Key]
        public int Id { get; set; }

        public int CarInsurePlanId { get; set; }

        public int CarKindId { get; set; }


    }
}
