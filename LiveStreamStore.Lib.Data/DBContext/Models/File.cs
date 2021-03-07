using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveStreamStore.Lib.Data.DBContext.Models
{
    public partial class File
    {
        public File()
        {
            Customer = new HashSet<Customer>();
            ProductInfo = new HashSet<ProductInfo>();
            User = new HashSet<User>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string FileExtension { get; set; }
        [StringLength(500)]
        public string FileName { get; set; }
        [StringLength(2000)]
        public string FilePath { get; set; }
        [StringLength(500)]
        public string Domain { get; set; }
        [StringLength(2000)]
        public string FileUrl { get; set; }
        public int? FileSize { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDateUtc { get; set; }

        [InverseProperty("Avatar")]
        public virtual ICollection<Customer> Customer { get; set; }
        [InverseProperty("Image")]
        public virtual ICollection<ProductInfo> ProductInfo { get; set; }
        [InverseProperty("Avatar")]
        public virtual ICollection<User> User { get; set; }
    }
}
