using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveStreamStore.Lib.Data.DBContext.Models
{
    public partial class Ward
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; }
        [StringLength(50)]
        public string DistrictId { get; set; }
        [StringLength(250)]
        public string WardName { get; set; }

        [ForeignKey(nameof(DistrictId))]
        [InverseProperty("Ward")]
        public virtual District District { get; set; }
    }
}
