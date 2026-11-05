using UmkmJatim.Application.Common.Interfaces;

namespace UmkmJatim.Infrastructure.ExternalServices.Shipping;

public class MockShippingService : IShippingService
{
    public Task<IReadOnlyList<ShippingOption>> GetShippingOptionsAsync(
        string originCity,
        string destinationCity,
        decimal weightGrams,
        CancellationToken cancellationToken = default)
    {
        var isSameCity = string.Equals(originCity?.Trim(), destinationCity?.Trim(), StringComparison.OrdinalIgnoreCase);

        // Perkiraan base cost
        decimal baseJneReg = isSameCity ? 9000m : 16000m;
        decimal baseJneYes = isSameCity ? 15000m : 28000m;
        decimal baseSicepat = isSameCity ? 10000m : 17000m;

        // Tambahan bobot per 1kg (1000g)
        decimal weightKg = Math.Max(1m, Math.Ceiling(weightGrams / 1000m));

        var options = new List<ShippingOption>
        {
            new(
                Courier: "JNE",
                Service: "REG",
                Description: "JNE Layanan Reguler",
                Cost: baseJneReg * weightKg,
                EstimateDays: isSameCity ? 1 : 2
            ),
            new(
                Courier: "JNE",
                Service: "YES",
                Description: "JNE Yakin Esok Sampai",
                Cost: baseJneYes * weightKg,
                EstimateDays: 1
            ),
            new(
                Courier: "SiCepat",
                Service: "REG",
                Description: "SiCepat Regular Package",
                Cost: baseSicepat * weightKg,
                EstimateDays: isSameCity ? 1 : 2
            )
        };

        return Task.FromResult<IReadOnlyList<ShippingOption>>(options);
    }
}
