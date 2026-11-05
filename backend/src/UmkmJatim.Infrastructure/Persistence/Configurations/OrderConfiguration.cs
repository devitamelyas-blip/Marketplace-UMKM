using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UmkmJatim.Domain.Entities;

namespace UmkmJatim.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(o => o.OrderNumber)
            .IsUnique();

        builder.Property(o => o.MidtransOrderId)
            .HasMaxLength(100);

        builder.HasIndex(o => o.MidtransOrderId)
            .IsUnique()
            .HasFilter("[MidtransOrderId] IS NOT NULL");

        builder.Property(o => o.ShippingCourier)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.ShippingService)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.SubTotal)
            .HasPrecision(18, 2);

        builder.Property(o => o.ShippingCost)
            .HasPrecision(18, 2);

        builder.Property(o => o.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasIndex(o => new { o.CustomerId, o.CreatedAt });

        builder.HasOne(o => o.Customer)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.ShippingAddress)
            .WithMany()
            .HasForeignKey(o => o.ShippingAddressId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
