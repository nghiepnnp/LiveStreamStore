using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveStreamStore.Lib.Data.DBContext.Models
{
    public partial class Address
    {
        public Address()
        {
            Order = new HashSet<Order>();
        }

        [Key]
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        [StringLength(50)]
        public string StateProvinceId { get; set; }
        [StringLength(50)]
        public string DistrictId { get; set; }
        [StringLength(50)]
        public string WardId { get; set; }
        [StringLength(100)]
        public string StateProvinceName { get; set; }
        [StringLength(100)]
        public string DistrictName { get; set; }
        [StringLength(250)]
        public string WardName { get; set; }
        [Column("Address")]
        [StringLength(500)]
        public string Address1 { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDateUtc { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedDateUtc { get; set; }

        [ForeignKey(nameof(CustomerId))]
        [InverseProperty("Address")]
        public virtual Customer Customer { get; set; }
        [InverseProperty("Address")]
        public virtual ICollection<Order> Order { get; set; }
    }
}
