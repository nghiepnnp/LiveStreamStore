using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveStreamStore.Lib.Data.DBContext.Models
{
    public partial class ShoppingCart
    {
        [Key]
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int? LivestreamId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public int? QuantitySold { get; set; }
        public int? Status { get; set; }

        [ForeignKey(nameof(CustomerId))]
        [InverseProperty("ShoppingCart")]
        public virtual Customer Customer { get; set; }
        [ForeignKey(nameof(LivestreamId))]
        [InverseProperty(nameof(LiveStream.ShoppingCart))]
        public virtual LiveStream Livestream { get; set; }
        [ForeignKey(nameof(ProductId))]
        [InverseProperty("ShoppingCart")]
        public virtual Product Product { get; set; }
    }
}
