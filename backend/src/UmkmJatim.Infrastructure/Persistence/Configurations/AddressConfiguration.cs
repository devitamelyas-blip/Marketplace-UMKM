using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UmkmJatim.Domain.Entities;

namespace UmkmJatim.Infrastructure.Persistence.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Label).IsRequired().HasMaxLength(50);
        builder.Property(a => a.RecipientName).IsRequired().HasMaxLength(100);
        builder.Property(a => a.PhoneNumber).IsRequired().HasMaxLength(25);
        builder.Property(a => a.Street).IsRequired().HasMaxLength(255);
        builder.Property(a => a.City).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Province).IsRequired().HasMaxLength(100);
        builder.Property(a => a.PostalCode).IsRequired().HasMaxLength(10);

        builder.HasIndex(a => a.UserId);

        builder.HasOne(a => a.User)
            .WithMany(u => u.Addresses)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
