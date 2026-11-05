using UmkmJatim.Domain.Enums;
using UmkmJatim.Domain.Interfaces;

namespace UmkmJatim.Domain.Entities;

public class Order : IHasUpdatedAt
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public int ShippingAddressId { get; set; }
    public string ShippingCourier { get; set; } = string.Empty;
    public string ShippingService { get; set; } = string.Empty;
    public decimal ShippingCost { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public string? MidtransOrderId { get; set; }
    public string? MidtransSnapToken { get; set; }
    public DateTime? PaymentDeadline { get; set; }
    public DateTime? PaidAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual ApplicationUser Customer { get; set; } = null!;
    public virtual Address ShippingAddress { get; set; } = null!;
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<PaymentLog> PaymentLogs { get; set; } = new List<PaymentLog>();
}
