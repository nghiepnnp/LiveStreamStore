using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveStreamStore.Lib.Data.DBContext.Models
{
    public partial class OrderDetail
    {
        public OrderDetail()
        {
            OrderTemp = new HashSet<OrderTemp>();
        }

        [Key]
        public long Id { get; set; }
        public long? OrderId { get; set; }
        public int? ProductId { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
        public double? TotalPrice { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDateUtc { get; set; }

        [ForeignKey(nameof(OrderId))]
        [InverseProperty("OrderDetail")]
        public virtual Order Order { get; set; }
        [ForeignKey(nameof(ProductId))]
        [InverseProperty("OrderDetail")]
        public virtual Product Product { get; set; }
        [InverseProperty("OrderDetail")]
        public virtual ICollection<OrderTemp> OrderTemp { get; set; }
    }
}
