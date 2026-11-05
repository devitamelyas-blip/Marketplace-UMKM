using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UmkmJatim.Application.Auth.DTOs;
using UmkmJatim.Application.Common.Interfaces;
using UmkmJatim.Domain.Entities;
using UmkmJatim.Domain.Exceptions;

namespace UmkmJatim.Application.Auth.Commands;

public record LoginCommand(
    string Email,
    string Password,
    string? IpAddress = null
) : IRequest<LoginResponse>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email wajib diisi.")
            .EmailAddress().WithMessage("Format email tidak valid.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password wajib diisi.");
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IApplicationDbContext _context;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IApplicationDbContext context)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _context = context;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new UnauthorizedException("Email atau password tidak sesuai.");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            throw new UnauthorizedException("Email atau password tidak sesuai.");
        }

        if (!user.IsActive)
        {
            throw new ForbiddenException("Akun Anda telah dinonaktifkan. Hubungi admin.");
        }

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(request.IpAddress);

        user.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

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
            accessToken,
            refreshToken.Token,
            refreshToken.ExpiresAt,
            userDto
        );
    }
}
