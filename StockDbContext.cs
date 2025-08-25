using System;

public class StockDbContext
{
	public StockDbContext() : DbContext
	{

        public StockDbContext(DbContextOptions<StockDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Product>(e =>
            {
                e.ToTable("Products");
                e.HasKey(x => x.Id);
                e.Property(x => x.Nome).HasMaxLength(200).IsRequired();
                e.Property(x => x.Preco).HasColumnType("decimal(18,2)");
            });
        }
}
}
