using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LiveStreamStore.Lib.Data.DBContext.Models
{
    public partial class ResultOrderFilter
    {
        public ResultOrderFilter()
        {
            OrderDetail = new HashSet<OrderDetail>();
        }
        public long Id { get; set; }
        public int? CustomerId { get; set; }
        public int? LiveStreamId { get; set; }
        public int? UserId { get; set; }
        public int? AddressId { get; set; }
        public int? StoreId { get; set; }
        public int? PaymentId { get; set; }
        public string Code { get; set; }
        public int? TotalItem { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Height { get; set; }
        public decimal? Width { get; set; }
        public decimal? Depth { get; set; }
        public DateTime? ShippingDate { get; set; }
        public DateTime? PickupDate { get; set; }
        public string ShippingFree { get; set; }
        public string Note { get; set; }
        public int? Discount { get; set; }
        public double? TotalPrice { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDateUtc { get; set; }
        public DateTime? ModifiedDateUtc { get; set; }
        public int? ModifiedBy { get; set; }
        public Nullable<int> TotalRow { get; set; }
        public Nullable<long> Row { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string UserEmail { get; set; }
        public bool? MappingToUsTransport { get; set; }
        public DateTime? LivestreamStartTime { get; set; }
        [NotMapped]
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
    }
}
