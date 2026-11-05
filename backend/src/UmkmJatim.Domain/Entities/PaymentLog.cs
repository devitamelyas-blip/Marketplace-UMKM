namespace UmkmJatim.Domain.Entities;

public class PaymentLog
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string RawPayload { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual Order Order { get; set; } = null!;
}
