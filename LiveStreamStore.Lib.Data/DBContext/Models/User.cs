using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveStreamStore.Lib.Data.DBContext.Models
{
    public partial class User
    {
        public User()
        {
            Customer = new HashSet<Customer>();
            LiveStream = new HashSet<LiveStream>();
            Order = new HashSet<Order>();
        }

        [Key]
        public int Id { get; set; }
        public int? StoreId { get; set; }
        public int? AvatarId { get; set; }
        public byte? RoleId { get; set; }
        [StringLength(200)]
        public string Fullname { get; set; }
        [StringLength(100)]
        public string Username { get; set; }
        [StringLength(100)]
        public string Password { get; set; }
        [StringLength(100)]
        public string Email { get; set; }
        [StringLength(15)]
        public string Phone { get; set; }
        [Column("FaceBookID")]
        [StringLength(100)]
        public string FaceBookId { get; set; }
        [StringLength(500)]
        public string FaceBookToken { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDateUtc { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedDateUtc { get; set; }
        public bool? Active { get; set; }
        public bool? MappingToUsTransport { get; set; }

        [ForeignKey(nameof(AvatarId))]
        [InverseProperty(nameof(File.User))]
        public virtual File Avatar { get; set; }
        [ForeignKey(nameof(RoleId))]
        [InverseProperty("User")]
        public virtual Role Role { get; set; }
        [ForeignKey(nameof(StoreId))]
        [InverseProperty("User")]
        public virtual Store Store { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Customer> Customer { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<LiveStream> LiveStream { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Order> Order { get; set; }
    }
}
