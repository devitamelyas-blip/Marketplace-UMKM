using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UmkmJatim.Domain.Entities;

namespace UmkmJatim.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Slug)
            .IsRequired()
            .HasMaxLength(250);

        builder.HasIndex(p => p.Slug)
            .IsUnique();

        builder.Property(p => p.Description)
            .IsRequired();

        builder.Property(p => p.Price)
            .HasPrecision(18, 2);

        builder.Property(p => p.WeightGrams)
            .HasPrecision(10, 2);

        builder.Property(p => p.OriginCity)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.OriginProvince)
            .IsRequired()
            .HasMaxLength(100);

        // Optimistic Concurrency Token
        builder.Property(p => p.RowVersion)
            .IsRowVersion();

        builder.HasIndex(p => p.ShopId);
        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => p.OriginCity);
        builder.HasIndex(p => new { p.IsActive, p.CreatedAt });

        builder.HasOne(p => p.Shop)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.ShopId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
