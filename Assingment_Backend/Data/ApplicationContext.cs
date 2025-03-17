using Assignment_Backend.Models;
using Assignment_Backend.Services;
using Assingment_Backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Assignment_Backend.Data
{
    public class ApplicationContext : IdentityDbContext<User>
    {
       // public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Product { get; set; }

        public DbSet<Brand> Brand { get; set; }

        public DbSet<Category> Category { get; set; }

        public DbSet<Cart> Cart { get; set; }

        public DbSet<CartItem> CartItem { get; set; }

        public DbSet<Order> Order { get; set; }

        public DbSet<OrderItems> OrderDetail { get; set; }

        public  DbSet<ProductSize> ProductSizes { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);  // Thêm dòng này

            modelBuilder.Entity<User>().HasMany(u => u.Orders).
                WithOne(u => u.User).
                HasForeignKey(o => o.UserId);

            modelBuilder.Entity<Brand>().
                HasMany(b => b.Products).
                WithOne(b => b.Brand);

            modelBuilder.Entity<Cart>().
                HasOne(c => c.User)
                .WithOne();

            modelBuilder.Entity<Cart>().
                HasMany(c => c.cartItems).
                WithOne(c => c.Cart)
                .HasForeignKey(c => c.CartId);

            modelBuilder.Entity<CartItem>().HasOne(c => c.Product)
                .WithMany();

            modelBuilder.Entity<Order>().HasMany(o => o.OrderDetails);

            modelBuilder.Entity<OrderItems>().HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId);

            modelBuilder.Entity<Product>()
                 .HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

        }
    }
}
