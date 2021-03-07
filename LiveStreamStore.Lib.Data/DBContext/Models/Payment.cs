using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveStreamStore.Lib.Data.DBContext.Models
{
    public partial class Payment
    {
        public Payment()
        {
            Order = new HashSet<Order>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(200)]
        public string PaymentType { get; set; }

        [InverseProperty("Payment")]
        public virtual ICollection<Order> Order { get; set; }
    }
}
