using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UmkmJatim.Application.Common.Interfaces;
using UmkmJatim.Domain.Enums;

namespace UmkmJatim.Infrastructure.BackgroundJobs;

public class OrderExpiryService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OrderExpiryService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(60);

    public OrderExpiryService(IServiceScopeFactory scopeFactory, ILogger<OrderExpiryService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OrderExpiryService telah dimulai.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessExpiredOrdersAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Terjadi kesalahan saat memproses pesanan kedaluwarsa.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("OrderExpiryService dihentikan.");
    }

    private async Task ProcessExpiredOrdersAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        var now = DateTime.UtcNow;

        var expiredOrders = await context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Where(o => o.Status == OrderStatus.Pending && o.PaymentDeadline != null && o.PaymentDeadline <= now)
            .ToListAsync(cancellationToken);

        if (!expiredOrders.Any())
            return;

        _logger.LogInformation("Ditemukan {Count} pesanan kedaluwarsa. Mengembalikan stok produk...", expiredOrders.Count);

        foreach (var order in expiredOrders)
        {
            order.Status = OrderStatus.Expired;
            order.UpdatedAt = DateTime.UtcNow;

            foreach (var item in order.OrderItems)
            {
                if (item.Product != null)
                {
                    item.Product.Stock += item.Quantity;
                    item.Product.UpdatedAt = DateTime.UtcNow;
                    _logger.LogInformation(
                        "Order {OrderNumber}: Stok produk {ProductName} dikembalikan sebanyak {Qty} unit.",
                        order.OrderNumber, item.ProductName, item.Quantity);
                }
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Berhasil memproses {Count} pesanan kedaluwarsa.", expiredOrders.Count);
    }
}
