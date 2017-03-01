using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.Entity
{
    [Table("CarInsureCommissionRate")]
    public class CarInsureCommissionRate
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Enumeration.CommissionRateType Type { get; set; }

        public int ReferenceId { get; set; }

        [MaxLength(128)]
        public string ReferenceName { get; set; }

        public decimal Commercial { get; set; }

        public decimal Compulsory { get; set; }

        public int Creator { get; set; }

        public DateTime CreateTime { get; set; }

        public int? Mender { get; set; }

        public DateTime? LastUpdateTime { get; set; }
    }
}
