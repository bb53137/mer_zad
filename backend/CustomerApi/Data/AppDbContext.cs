using CustomerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var customer = modelBuilder.Entity<Customer>();

        customer.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        customer.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        customer.Property(x => x.Email).HasMaxLength(255).IsRequired();
        customer.Property(x => x.Phone).HasMaxLength(50);
        customer.Property(x => x.City).HasMaxLength(100).IsRequired();
        customer.Property(x => x.Country).HasMaxLength(100).IsRequired();

        customer.HasIndex(x => x.Email).IsUnique();
        customer.HasIndex(x => x.City);
        customer.HasIndex(x => x.Country);
        customer.HasIndex(x => x.IsActive);
    }
}