using Microsoft.EntityFrameworkCore;
using UmkmJatim.Domain.Entities;

namespace UmkmJatim.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<ApplicationUser> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Address> Addresses { get; }
    DbSet<Category> Categories { get; }
    DbSet<Shop> Shops { get; }
    DbSet<Product> Products { get; }
    DbSet<ProductImage> ProductImages { get; }
    DbSet<CartItem> CartItems { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<PaymentLog> PaymentLogs { get; }
    DbSet<Review> Reviews { get; }
    DbSet<ReviewImage> ReviewImages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
