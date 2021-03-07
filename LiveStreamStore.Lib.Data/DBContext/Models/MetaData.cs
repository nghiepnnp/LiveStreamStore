using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LiveStreamStore.Lib.Data.DBContext.Models
{
    public partial class Comment
    {
        [NotMapped]
        public virtual string Phone { get; set; }
        [NotMapped]
        public virtual bool HasAddress { get; set; }
        [NotMapped]
        public virtual int TotalPurchased { get; set; }
        [NotMapped]
        public virtual int TotalCancellations { get; set; }

    }
    public partial class OrderDetail
    {
        [NotMapped]
        public virtual int? SoLuongProductConLai { get; set; }
    }
    public partial class OrderTemp
    {
        [NotMapped]
        public virtual int? Count { get; set; }
        [NotMapped]
        public virtual Comment Comment { get; set; }
    }
}
