using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveStreamStore.Lib.Data.DBContext.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderDetail = new HashSet<OrderDetail>();
            ShoppingCart = new HashSet<ShoppingCart>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string Code { get; set; }
        public double? Price { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Weight { get; set; }
        public int? Quantity { get; set; }
        public int? NumberProductSold { get; set; }
        public int? Limited { get; set; }
        public short? Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDateUtc { get; set; }
        public int? ProductInfoId { get; set; }
        public int? LiveStreamId { get; set; }
        public int? StoreId { get; set; }

        [ForeignKey(nameof(LiveStreamId))]
        [InverseProperty("Product")]
        public virtual LiveStream LiveStream { get; set; }
        [ForeignKey(nameof(ProductInfoId))]
        [InverseProperty("Product")]
        public virtual ProductInfo ProductInfo { get; set; }
        [InverseProperty("Product")]
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
        [InverseProperty("Product")]
        public virtual ICollection<ShoppingCart> ShoppingCart { get; set; }
    }
}
