using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveStreamStore.Lib.Data.DBContext.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetail = new HashSet<OrderDetail>();
        }

        [Key]
        public long Id { get; set; }
        public int? CustomerId { get; set; }
        public int? LiveStreamId { get; set; }
        public int? UserId { get; set; }
        public int? AddressId { get; set; }
        public int? StoreId { get; set; }
        public int? PaymentId { get; set; }
        [StringLength(50)]
        public string Code { get; set; }
        public int? TotalItem { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Weight { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Height { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Width { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Depth { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ShippingDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? PickupDate { get; set; }
        [StringLength(200)]
        public string ShippingFree { get; set; }
        [StringLength(500)]
        public string Note { get; set; }
        public int? Discount { get; set; }
        public double? TotalPrice { get; set; }
        public int? Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDateUtc { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedDateUtc { get; set; }
        public int? ModifiedBy { get; set; }

        [ForeignKey(nameof(AddressId))]
        [InverseProperty("Order")]
        public virtual Address Address { get; set; }
        [ForeignKey(nameof(CustomerId))]
        [InverseProperty("Order")]
        public virtual Customer Customer { get; set; }
        [ForeignKey(nameof(LiveStreamId))]
        [InverseProperty("Order")]
        public virtual LiveStream LiveStream { get; set; }
        [ForeignKey(nameof(PaymentId))]
        [InverseProperty("Order")]
        public virtual Payment Payment { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty("Order")]
        public virtual User User { get; set; }
        [InverseProperty("Order")]
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
    }
}
