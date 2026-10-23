using AdvancedEFCoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AdvancedEFCoreAPI.Data;

public class ApplicationDbContext : DbContext
{
public ApplicationDbContext(
DbContextOptions<ApplicationDbContext> options)
: base(options)
{
}


public DbSet<Product> Products { get; set; }

public DbSet<Category> Categories { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Product configuration
    modelBuilder.Entity<Product>(entity =>
    {
        entity.HasKey(p => p.Id);

        entity.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        entity.Property(p => p.Price)
            .HasPrecision(18, 2);

        entity.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(p => p.Name);

        entity.HasQueryFilter(p => !p.IsDeleted);

        entity.Property(p => p.RowVersion)
            .IsRowVersion();
    });

    // Category configuration
    modelBuilder.Entity<Category>(entity =>
    {
        entity.HasKey(c => c.Id);

        entity.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);
    });
}


}
