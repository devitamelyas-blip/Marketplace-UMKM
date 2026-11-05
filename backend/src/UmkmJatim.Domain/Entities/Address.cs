namespace UmkmJatim.Domain.Entities;

public class Address
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string Label { get; set; } = string.Empty; // e.g. "Rumah", "Kantor"
    public string RecipientName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Province { get; set; } = "Jawa Timur";
    public string PostalCode { get; set; } = string.Empty;
    public bool IsDefault { get; set; } = false;

    // Navigation
    public virtual ApplicationUser User { get; set; } = null!;
}
