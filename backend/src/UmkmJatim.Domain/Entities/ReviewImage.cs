namespace UmkmJatim.Domain.Entities;

public class ReviewImage
{
    public int Id { get; set; }
    public int ReviewId { get; set; }
    public string Url { get; set; } = string.Empty;
    public int SortOrder { get; set; } = 0;

    // Navigation
    public virtual Review Review { get; set; } = null!;
}
