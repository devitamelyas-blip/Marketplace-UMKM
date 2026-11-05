using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UmkmJatim.Domain.Entities;

namespace UmkmJatim.Infrastructure.Persistence.Configurations;

public class ShopConfiguration : IEntityTypeConfiguration<Shop>
{
    public void Configure(EntityTypeBuilder<Shop> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).IsRequired().HasMaxLength(150);
        builder.Property(s => s.Slug).IsRequired().HasMaxLength(200);
        builder.Property(s => s.City).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Province).IsRequired().HasMaxLength(100);

        builder.HasIndex(s => s.Slug).IsUnique();
        builder.HasIndex(s => s.OwnerId).IsUnique();

        builder.HasOne(s => s.Owner)
            .WithOne(u => u.Shop)
            .HasForeignKey<Shop>(s => s.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
