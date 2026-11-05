using System.ComponentModel.DataAnnotations;
using UmkmJatim.Domain.Interfaces;

namespace UmkmJatim.Domain.Entities;

public class Product : IHasUpdatedAt
{
    public int Id { get; set; }
    public int ShopId { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public string OriginCity { get; set; } = string.Empty;
    public string OriginProvince { get; set; } = "Jawa Timur";
    public decimal WeightGrams { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsJatimBadge { get; set; } = false;
    public DateTime? BadgeVerifiedAt { get; set; }
    public Guid? BadgeVerifiedById { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual Shop Shop { get; set; } = null!;
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
