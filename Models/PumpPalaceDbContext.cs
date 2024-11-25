using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PumpPalace.Models;

namespace PumpPalace.Models
{
    public class PumpPalaceDbContext : DbContext
    {
        public PumpPalaceDbContext(DbContextOptions<PumpPalaceDbContext> options) : base(options)
        {
        }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ContactForm> ContactForms { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Customer> Users { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Newsletter> Newsletters { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductStats> ProductStats { get; set; }
        public DbSet<Skin> Skins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}
