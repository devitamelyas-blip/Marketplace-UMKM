namespace UmkmJatim.Application.Auth.DTOs;

public record UserDto(
    Guid Id,
    string Email,
    string FullName,
    string Role,
    string? PhoneNumber,
    string? AvatarUrl,
    bool IsActive
);

public record UserProfileDto(
    Guid Id,
    string Email,
    string FullName,
    string Role,
    string? PhoneNumber,
    string? AvatarUrl,
    DateTime CreatedAt
);

public record RegisterResponse(
    Guid UserId,
    string Email,
    string FullName,
    string Role
);

public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserDto User
);
