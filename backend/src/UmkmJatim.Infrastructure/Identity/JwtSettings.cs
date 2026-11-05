namespace UmkmJatim.Infrastructure.Identity;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "UmkmJatimAPI";
    public string Audience { get; set; } = "UmkmJatimClient";
    public int AccessTokenExpiresMinutes { get; set; } = 15;
    public int RefreshTokenExpiresDays { get; set; } = 7;
}
