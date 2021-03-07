using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveStreamStore.Lib.Data.DBContext.Models
{
    public partial class StateProvince
    {
        public StateProvince()
        {
            District = new HashSet<District>();
        }

        [Key]
        [StringLength(50)]
        public string Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(100)]
        public string ShortName { get; set; }
        public bool? Published { get; set; }
        public int? DisplayOrder { get; set; }
        public int? AirPort { get; set; }
        [StringLength(20)]
        public string AirPortCode { get; set; }

        [InverseProperty("StateProvince")]
        public virtual ICollection<District> District { get; set; }
    }
}
