using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveStreamStore.Lib.Data.DBContext.Models
{
    public partial class Category
    {
        public Category()
        {
            ProductInfo = new HashSet<ProductInfo>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(250)]
        public string Name { get; set; }
        [StringLength(50)]
        public string Code { get; set; }

        [InverseProperty("Category")]
        public virtual ICollection<ProductInfo> ProductInfo { get; set; }
    }
}
