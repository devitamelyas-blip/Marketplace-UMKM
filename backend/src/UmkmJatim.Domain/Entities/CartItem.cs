using UmkmJatim.Domain.Interfaces;

namespace UmkmJatim.Domain.Entities;

public class CartItem : IHasUpdatedAt
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}
