namespace UmkmJatim.Application.Common.Interfaces;

public record ShippingOption(string Courier, string Service, string Description, decimal Cost, int EstimateDays);

public interface IShippingService
{
    Task<IReadOnlyList<ShippingOption>> GetShippingOptionsAsync(
        string originCity,
        string destinationCity,
        decimal weightGrams,
        CancellationToken cancellationToken = default);
}
