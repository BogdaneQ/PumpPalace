using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Newsletter> Newsletters { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductStats> ProductStats { get; set; }
        public DbSet<Skin> Skins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Definiowanie globalnej konwersji dla wszystkich DateTime/DateTime? w modelu
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType.ClrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?));

                foreach (var property in properties)
                {
                    var converter = new ValueConverter<DateTime, DateTime>(
                        v => v.ToUniversalTime().AddHours(1), // Konwersja do UTC i dodanie jednej godziny przy zapisie
                        v => DateTime.SpecifyKind(v.AddHours(1), DateTimeKind.Utc) // Dodanie jednej godziny i ustawienie UTC przy odczycie
                    );


                    modelBuilder.Entity(entityType.ClrType)
                        .Property(property.Name)
                        .HasConversion(converter);
                }
            }

            base.OnModelCreating(modelBuilder);
        }



    }
}
