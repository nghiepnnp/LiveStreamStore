using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveStreamStore.Lib.Data.DBContext.Models
{
    public partial class District
    {
        public District()
        {
            Ward = new HashSet<Ward>();
        }

        [Key]
        [StringLength(50)]
        public string Id { get; set; }
        [StringLength(50)]
        public string StateProvinceId { get; set; }
        [StringLength(100)]
        public string Name { get; set; }

        [ForeignKey(nameof(StateProvinceId))]
        [InverseProperty("District")]
        public virtual StateProvince StateProvince { get; set; }
        [InverseProperty("District")]
        public virtual ICollection<Ward> Ward { get; set; }
    }
}
