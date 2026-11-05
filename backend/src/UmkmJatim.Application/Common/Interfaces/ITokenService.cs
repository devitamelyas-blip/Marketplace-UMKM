using UmkmJatim.Domain.Entities;

namespace UmkmJatim.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(ApplicationUser user);
    RefreshToken GenerateRefreshToken(string? ipAddress);
    Guid? ValidateAccessToken(string token);
}
