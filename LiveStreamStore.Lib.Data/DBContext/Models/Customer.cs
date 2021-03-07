using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveStreamStore.Lib.Data.DBContext.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Address = new HashSet<Address>();
            Comment = new HashSet<Comment>();
            Order = new HashSet<Order>();
            OrderTemp = new HashSet<OrderTemp>();
            ShoppingCart = new HashSet<ShoppingCart>();
        }

        [Key]
        public int Id { get; set; }
        public int? UserId { get; set; }
        [StringLength(200)]
        public string Fullname { get; set; }
        [StringLength(50)]
        public string Phone { get; set; }
        [StringLength(100)]
        public string Password { get; set; }
        [StringLength(100)]
        public string Email { get; set; }
        public int? AvatarId { get; set; }
        [StringLength(100)]
        public string FaceBookId { get; set; }
        public int? TotalPurchased { get; set; }
        public int? TotalCancellations { get; set; }
        public double? Rating { get; set; }
        [StringLength(500)]
        public string Note { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDateUtc { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedDateUtc { get; set; }

        [ForeignKey(nameof(AvatarId))]
        [InverseProperty(nameof(File.Customer))]
        public virtual File Avatar { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty("Customer")]
        public virtual User User { get; set; }
        [InverseProperty("Customer")]
        public virtual ICollection<Address> Address { get; set; }
        [InverseProperty("Customer")]
        public virtual ICollection<Comment> Comment { get; set; }
        [InverseProperty("Customer")]
        public virtual ICollection<Order> Order { get; set; }
        [InverseProperty("Customer")]
        public virtual ICollection<OrderTemp> OrderTemp { get; set; }
        [InverseProperty("Customer")]
        public virtual ICollection<ShoppingCart> ShoppingCart { get; set; }
    }
}
