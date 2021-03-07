using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace LiveStreamStore.Lib.Data.DBContext.Models
{
    public partial class LiveStreamStoreContext : DbContext
    {
        public LiveStreamStoreContext()
        {
        }

        public LiveStreamStoreContext(DbContextOptions<LiveStreamStoreContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<District> District { get; set; }
        public virtual DbSet<File> File { get; set; }
        public virtual DbSet<LiveStream> LiveStream { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderDetail> OrderDetail { get; set; }
        public virtual DbSet<OrderTemp> OrderTemp { get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductInfo> ProductInfo { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCart { get; set; }
        public virtual DbSet<StateProvince> StateProvince { get; set; }
        public virtual DbSet<Store> Store { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Ward> Ward { get; set; }
        public virtual DbSet<ResultLiveStreamFilter> ResultLiveStreamFilter { get; set; }
        public virtual DbSet<ResultOrderTempFilter> ResultOrderTempFilter { get; set; }
        public virtual DbSet<ResultOrderFilter> ResultOrderFilter { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                           .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                           .AddJsonFile("appsettings.json")
                           .Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("LiveStreamStore"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.Property(e => e.CreatedDateUtc).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.DistrictId).IsUnicode(false);

                entity.Property(e => e.StateProvinceId).IsUnicode(false);

                entity.Property(e => e.WardId).IsUnicode(false);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Address)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Address_Customer");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.CommentFaceBookId).IsUnicode(false);

                entity.Property(e => e.CreatedDateUtc).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.FaceBookId).IsUnicode(false);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Comment_Customer");

                entity.HasOne(d => d.LiveStream)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.LiveStreamId)
                    .HasConstraintName("FK_Comment_LiveStream");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.CreatedDateUtc).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.FaceBookId).IsUnicode(false);

                entity.Property(e => e.Password).IsUnicode(false);

                entity.Property(e => e.Phone).IsUnicode(false);

                entity.HasOne(d => d.Avatar)
                    .WithMany(p => p.Customer)
                    .HasForeignKey(d => d.AvatarId)
                    .HasConstraintName("FK_Customer_File");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Customer)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Customer_User");
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.Property(e => e.Id).IsUnicode(false);

                entity.Property(e => e.StateProvinceId).IsUnicode(false);

                entity.HasOne(d => d.StateProvince)
                    .WithMany(p => p.District)
                    .HasForeignKey(d => d.StateProvinceId)
                    .HasConstraintName("FK_District_StateProvince");
            });

            modelBuilder.Entity<File>(entity =>
            {
                entity.Property(e => e.CreatedDateUtc).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Domain).IsUnicode(false);

                entity.Property(e => e.FileExtension).IsUnicode(false);

                entity.Property(e => e.FilePath).IsUnicode(false);

                entity.Property(e => e.FileUrl).IsUnicode(false);
            });

            modelBuilder.Entity<LiveStream>(entity =>
            {
                entity.Property(e => e.Code).IsUnicode(false);

                entity.Property(e => e.CreatedDateUtc).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Link).IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.LiveStream)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_LiveStream_User");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.Code).IsUnicode(false);

                entity.Property(e => e.CreatedDateUtc).HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.Order)
                    .HasForeignKey(d => d.AddressId)
                    .HasConstraintName("FK_Order_Address");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Order)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Order_Customer");

                entity.HasOne(d => d.LiveStream)
                    .WithMany(p => p.Order)
                    .HasForeignKey(d => d.LiveStreamId)
                    .HasConstraintName("FK_Order_LiveStream");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.Order)
                    .HasForeignKey(d => d.PaymentId)
                    .HasConstraintName("FK_Order_Payment");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Order)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Order_User");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.CreatedDateUtc).HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetail)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_OrderDetail_Order");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetail)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_OrderDetail_Product");
            });

            modelBuilder.Entity<OrderTemp>(entity =>
            {
                entity.Property(e => e.CommentFaceBookId).IsUnicode(false);

                entity.Property(e => e.CreatedDateUtc).HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.OrderTemp)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_OrderTemp_Customer");

                entity.HasOne(d => d.LiveStream)
                    .WithMany(p => p.OrderTemp)
                    .HasForeignKey(d => d.LiveStreamId)
                    .HasConstraintName("FK_OrderTemp_LiveStream");

                entity.HasOne(d => d.OrderDetail)
                    .WithMany(p => p.OrderTemp)
                    .HasForeignKey(d => d.OrderDetailId)
                    .HasConstraintName("FK_OrderTemp_OrderDetail");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Code).IsUnicode(false);

                entity.Property(e => e.CreatedDateUtc).HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.LiveStream)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.LiveStreamId)
                    .HasConstraintName("FK_Product_LiveStream");

                entity.HasOne(d => d.ProductInfo)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.ProductInfoId)
                    .HasConstraintName("FK_Product_ProductInfo");
            });

            modelBuilder.Entity<ProductInfo>(entity =>
            {
                entity.Property(e => e.Barcode).IsUnicode(false);

                entity.Property(e => e.CreatedDateUtc).HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.ProductInfo)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_ProductInfo_Category");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.ProductInfo)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK_ProductInfo_File");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.CreatedDateUtc).HasDefaultValueSql("(getutcdate())");
            });

            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.ShoppingCart)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_ShoppingCart_Customer");

                entity.HasOne(d => d.Livestream)
                    .WithMany(p => p.ShoppingCart)
                    .HasForeignKey(d => d.LivestreamId)
                    .HasConstraintName("FK_ShoppingCart_LiveStream");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ShoppingCart)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ShoppingCart_Product");
            });

            modelBuilder.Entity<StateProvince>(entity =>
            {
                entity.Property(e => e.Id).IsUnicode(false);

                entity.Property(e => e.AirPortCode).IsUnicode(false);

                entity.Property(e => e.Published).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code).IsUnicode(false);

                entity.Property(e => e.CreatedDateUtc).HasDefaultValueSql("(getutcdate())");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.CreatedDateUtc).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.FaceBookId).IsUnicode(false);

                entity.Property(e => e.FaceBookToken).IsUnicode(false);

                entity.Property(e => e.Password).IsUnicode(false);

                entity.Property(e => e.Phone).IsUnicode(false);

                entity.Property(e => e.Username).IsUnicode(false);

                entity.HasOne(d => d.Avatar)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.AvatarId)
                    .HasConstraintName("FK_User_File");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_User_Role");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.StoreId)
                    .HasConstraintName("FK_User_Store");
            });

            modelBuilder.Entity<Ward>(entity =>
            {
                entity.Property(e => e.Id).IsUnicode(false);

                entity.Property(e => e.DistrictId).IsUnicode(false);

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Ward)
                    .HasForeignKey(d => d.DistrictId)
                    .HasConstraintName("FK_Ward_District");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
