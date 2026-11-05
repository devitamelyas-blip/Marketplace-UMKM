using UmkmJatim.Domain.Interfaces;

namespace UmkmJatim.Domain.Entities;

public class Shop : IHasUpdatedAt
{
    public int Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string City { get; set; } = string.Empty;
    public string Province { get; set; } = "Jawa Timur";
    public string? BannerUrl { get; set; }
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual ApplicationUser Owner { get; set; } = null!;
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
