namespace UmkmJatim.Domain.Entities;

public class ProductImage
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Url { get; set; } = string.Empty;
    public int SortOrder { get; set; } = 0;
    public bool IsPrimary { get; set; } = false;

    // Navigation
    public virtual Product Product { get; set; } = null!;
}
