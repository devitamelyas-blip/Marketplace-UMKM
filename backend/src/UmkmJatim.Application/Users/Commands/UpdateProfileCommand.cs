using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UmkmJatim.Application.Auth.DTOs;
using UmkmJatim.Application.Common.Interfaces;
using UmkmJatim.Domain.Entities;
using UmkmJatim.Domain.Exceptions;

namespace UmkmJatim.Application.Users.Commands;

public record UpdateProfileCommand(
    string FullName,
    string? PhoneNumber
) : IRequest<UserProfileDto>;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Nama lengkap wajib diisi.")
            .Length(2, 100);

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^[0-9+]*$").WithMessage("Nomor telepon tidak valid.");
    }
}

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, UserProfileDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUser _currentUser;

    public UpdateProfileCommandHandler(UserManager<ApplicationUser> userManager, ICurrentUser currentUser)
    {
        _userManager = userManager;
        _currentUser = currentUser;
    }

    public async Task<UserProfileDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId == null)
            throw new UnauthorizedException();

        var user = await _userManager.FindByIdAsync(_currentUser.UserId.Value.ToString());
        if (user == null)
            throw new NotFoundException("User", _currentUser.UserId.Value);

        user.FullName = request.FullName.Trim();
        user.PhoneNumber = request.PhoneNumber?.Trim();
        user.UpdatedAt = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return new UserProfileDto(
            user.Id,
            user.Email ?? string.Empty,
            user.FullName,
            user.Role,
            user.PhoneNumber,
            user.AvatarUrl,
            user.CreatedAt
        );
    }
}
