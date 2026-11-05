using MediatR;
using Microsoft.EntityFrameworkCore;
using UmkmJatim.Application.Auth.DTOs;
using UmkmJatim.Application.Common.Interfaces;
using UmkmJatim.Domain.Exceptions;

namespace UmkmJatim.Application.Auth.Commands;

public record RefreshTokenCommand(
    string Token,
    string? IpAddress = null
) : IRequest<LoginResponse>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(IApplicationDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenRecord = await _context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == request.Token, cancellationToken);

        if (tokenRecord == null || !tokenRecord.IsActive)
        {
            throw new UnauthorizedException("Refresh token tidak valid atau telah kedaluwarsa.");
        }

        var user = tokenRecord.User;
        if (!user.IsActive)
        {
            throw new ForbiddenException("Akun Anda tidak aktif.");
        }

        // Revoke token lama
        var newRefreshToken = _tokenService.GenerateRefreshToken(request.IpAddress);
        tokenRecord.RevokedAt = DateTime.UtcNow;
        tokenRecord.ReplacedByToken = newRefreshToken.Token;

        user.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        var newAccessToken = _tokenService.GenerateAccessToken(user);

        var userDto = new UserDto(
            user.Id,
            user.Email ?? string.Empty,
            user.FullName,
            user.Role,
            user.PhoneNumber,
            user.AvatarUrl,
            user.IsActive
        );

        return new LoginResponse(
            newAccessToken,
            newRefreshToken.Token,
            newRefreshToken.ExpiresAt,
            userDto
        );
    }
}
