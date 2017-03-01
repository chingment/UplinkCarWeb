﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Lumos.Entity
{
    [Table("BizProcessesAuditDetails")]
    public class BizProcessesAuditDetails
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int AuditStep { get; set; }

        [MaxLength(128)]
        public string AuditStepEnumName { get; set; }

        public int BizProcessesAuditId { get; set; }

        public int? Auditor { get; set; }

        [MaxLength(1024)]
        public string AuditComments { get; set; }

        [MaxLength(1024)]
        public string Description { get; set; }

        public DateTime? AuditTime { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int Creator { get; set; }

        public DateTime CreateTime { get; set; }

        public int? Mender { get; set; }

        public DateTime? LastUpdateTime { get; set; }

    }


}
