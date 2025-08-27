using Microsoft.EntityFrameworkCore;
using OrderService.OrderService.Domain.Entities;

namespace OrderService.OrderService.Infrastructure.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasKey(o => o.Id);
            modelBuilder.Entity<OrderItem>().HasKey(oi => oi.Id);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey(oi => oi.OrderId);

            // initial SEED
            modelBuilder.Entity<Order>().HasData(
    new Order { Id = 1, CreatedAt = new DateTime(2025, 01, 01) } 
);

            modelBuilder.Entity<OrderItem>().HasData(
                new OrderItem { Id = 1, OrderId = 1, ProductId = 1, Quantity = 2 },
                new OrderItem { Id = 2, OrderId = 1, ProductId = 2, Quantity = 1 }
            );
        }
    }
}
