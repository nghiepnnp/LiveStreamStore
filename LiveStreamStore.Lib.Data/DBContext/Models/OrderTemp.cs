using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveStreamStore.Lib.Data.DBContext.Models
{
    public partial class OrderTemp
    {
        [Key]
        public long Id { get; set; }
        public int? CustomerId { get; set; }
        public int? LiveStreamId { get; set; }
        [StringLength(100)]
        public string CommentFaceBookId { get; set; }
        public long? OrderDetailId { get; set; }
        public bool? IsPreSale { get; set; }
        public short? Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDateUtc { get; set; }

        [ForeignKey(nameof(CustomerId))]
        [InverseProperty("OrderTemp")]
        public virtual Customer Customer { get; set; }
        [ForeignKey(nameof(LiveStreamId))]
        [InverseProperty("OrderTemp")]
        public virtual LiveStream LiveStream { get; set; }
        [ForeignKey(nameof(OrderDetailId))]
        [InverseProperty("OrderTemp")]
        public virtual OrderDetail OrderDetail { get; set; }
    }
}
