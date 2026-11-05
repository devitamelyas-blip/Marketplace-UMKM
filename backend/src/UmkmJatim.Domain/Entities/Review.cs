namespace UmkmJatim.Domain.Entities;

public class Review
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int OrderItemId { get; set; }
    public Guid CustomerId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual Product Product { get; set; } = null!;
    public virtual OrderItem OrderItem { get; set; } = null!;
    public virtual ApplicationUser Customer { get; set; } = null!;
    public virtual ICollection<ReviewImage> Images { get; set; } = new List<ReviewImage>();
}
