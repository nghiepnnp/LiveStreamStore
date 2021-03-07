using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveStreamStore.Lib.Data.DBContext.Models
{
    public partial class LiveStream
    {
        public LiveStream()
        {
            Comment = new HashSet<Comment>();
            Order = new HashSet<Order>();
            OrderTemp = new HashSet<OrderTemp>();
            Product = new HashSet<Product>();
            ShoppingCart = new HashSet<ShoppingCart>();
        }

        [Key]
        public int Id { get; set; }
        public int? UserId { get; set; }
        [StringLength(500)]
        public string Link { get; set; }
        [StringLength(50)]
        public string Code { get; set; }
        public int? TotalOrder { get; set; }
        public int? TotalProduct { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public short? Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LivestreamStartTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDateUtc { get; set; }
        public bool? OpenPreSale { get; set; }
        public bool? IsStreaming { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("LiveStream")]
        public virtual User User { get; set; }
        [InverseProperty("LiveStream")]
        public virtual ICollection<Comment> Comment { get; set; }
        [InverseProperty("LiveStream")]
        public virtual ICollection<Order> Order { get; set; }
        [InverseProperty("LiveStream")]
        public virtual ICollection<OrderTemp> OrderTemp { get; set; }
        [InverseProperty("LiveStream")]
        public virtual ICollection<Product> Product { get; set; }
        [InverseProperty("Livestream")]
        public virtual ICollection<ShoppingCart> ShoppingCart { get; set; }
    }
}
