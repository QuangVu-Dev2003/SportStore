using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SportStore.DataAccessLayer.Models;

namespace SportStore.DataAccessLayer.Data
{
    public class SportStoreDbContext : IdentityDbContext<AppUser>
    {
        public SportStoreDbContext(DbContextOptions<SportStoreDbContext> options) : base(options) { }

        public DbSet<ProductModel> Products { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<BrandModel> Brands { get; set; }
        public DbSet<ProductCategoryModel> ProductCategories { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<OrderDetailModel> OrderDetails { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<CartModel> Carts { get; set; }
        public DbSet<CartItemModel> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductModel>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ProductModel>()
                .HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandId);

            modelBuilder.Entity<ProductCategoryModel>()
                .HasKey(pc => new { pc.ProductId, pc.CategoryId });

            modelBuilder.Entity<ProductCategoryModel>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategoryModels)
                .HasForeignKey(pc => pc.ProductId);

            modelBuilder.Entity<ProductCategoryModel>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProductCategoryModels)
                .HasForeignKey(pc => pc.CategoryId);

            modelBuilder.Entity<OrderModel>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderModel>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .IsRequired();

            modelBuilder.Entity<OrderDetailModel>()
                .Property(od => od.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderDetailModel>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetailModel>()
                .HasOne(od => od.Product)
                .WithMany()
                .HasForeignKey(od => od.ProductId);

            modelBuilder.Entity<OrderDetailModel>()
                .HasIndex(od => od.ProductId);

            modelBuilder.Entity<CartModel>()
                .Property(c => c.UserId)
                .IsRequired();

            modelBuilder.Entity<CartModel>()
                .HasMany(c => c.Items)
                .WithOne(i => i.Cart)
                .HasForeignKey(i => i.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartModel>()
                .HasIndex(c => c.UserId);

            modelBuilder.Entity<CartItemModel>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId);

            modelBuilder.Entity<OrderModel>()
                .HasIndex(o => o.UserId);
        }
    }
}