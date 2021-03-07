using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveStreamStore.Lib.Data.DBContext.Models
{
    public partial class ProductInfo
    {
        public ProductInfo()
        {
            Product = new HashSet<Product>();
        }

        [Key]
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public int? ImageId { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(100)]
        public string Barcode { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDateUtc { get; set; }
        public int? StoreId { get; set; }
        public short? IsDeleted { get; set; }

        [ForeignKey(nameof(CategoryId))]
        [InverseProperty("ProductInfo")]
        public virtual Category Category { get; set; }
        [ForeignKey(nameof(ImageId))]
        [InverseProperty(nameof(File.ProductInfo))]
        public virtual File Image { get; set; }
        [InverseProperty("ProductInfo")]
        public virtual ICollection<Product> Product { get; set; }
    }
}
