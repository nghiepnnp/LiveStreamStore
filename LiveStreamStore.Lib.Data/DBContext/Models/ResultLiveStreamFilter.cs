using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LiveStreamStore.Lib.Data.DBContext.Models
{
    public partial class ResultLiveStreamFilter
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Link { get; set; }
        public string Code { get; set; }
        public int? TotalOrder { get; set; }
        public int? TotalProduct { get; set; }
        public string Description { get; set; }
        public DateTime? LivestreamStartTime { get; set; }
        public DateTime? CreatedDateUtc { get; set; }
        public bool? IsStreaming { get; set; }
        public Nullable<int> TotalRow { get; set; }
        public Nullable<long> Row { get; set; }
        public bool? OpenPreSale { get; set; }


        [NotMapped]
        public virtual int? TotalOrderPreSale { get; set; }

        [NotMapped]
        public virtual int? TotalOrderSaleLive { get; set; }

        [NotMapped]
        public virtual int? TotalProductPreSale { get; set; }

        [NotMapped]
        public virtual int? TotalProductSaleLive { get; set; }
    }
}
