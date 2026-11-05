namespace UmkmJatim.Infrastructure.ExternalServices.Midtrans;

public class MidtransSettings
{
    public const string SectionName = "MidtransSettings";

    public string ServerKey { get; set; } = string.Empty;
    public string ClientKey { get; set; } = string.Empty;
    public bool IsProduction { get; set; } = false;
    public string SnapUrl { get; set; } = "https://app.sandbox.midtrans.com/snap/snap.js";

    public string BaseUrl => IsProduction 
        ? "https://app.midtrans.com/snap/v1" 
        : "https://app.sandbox.midtrans.com/snap/v1";
}
