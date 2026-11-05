using Microsoft.AspNetCore.Identity;
using UmkmJatim.Domain.Interfaces;

namespace UmkmJatim.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>, IHasUpdatedAt
{
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public string Role { get; set; } = "Customer";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Shop? Shop { get; set; }
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
