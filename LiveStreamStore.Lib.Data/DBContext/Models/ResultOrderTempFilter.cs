using System;
using System.Collections.Generic;
using System.Text;

namespace LiveStreamStore.Lib.Data.DBContext.Models
{
    public class ResultOrderTempFilter
    {
        public long Id { get; set; }
        public int? CustomerId { get; set; }
        public int? LiveStreamId { get; set; }
        public short? Status { get; set; }
        public string CommentFacebookId { get; set; }
        public long? OrderDetailId { get; set; }
        public bool? IsPreSale { get; set; }
        public DateTime? CreatedDateUtc { get; set; }
        public Nullable<int> TotalRow { get; set; }
        public Nullable<long> Row { get; set; }
        public int? Quantity { get; set; }
        public double? TotalPrice { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string ProductCode { get; set; }
        public double? ProductPrice { get; set; }
        public bool? MappingToUsTransport { get; set; }
    }
}
