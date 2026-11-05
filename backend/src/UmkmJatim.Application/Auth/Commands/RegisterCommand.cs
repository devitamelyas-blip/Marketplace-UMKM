using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UmkmJatim.Application.Auth.DTOs;
using UmkmJatim.Domain.Entities;
using UmkmJatim.Domain.Exceptions;

namespace UmkmJatim.Application.Auth.Commands;

public record RegisterCommand(
    string FullName,
    string Email,
    string Password,
    string ConfirmPassword,
    string PhoneNumber
) : IRequest<RegisterResponse>;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Nama lengkap wajib diisi.")
            .Length(2, 100).WithMessage("Nama lengkap minimal 2 karakter dan maksimal 100 karakter.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email wajib diisi.")
            .EmailAddress().WithMessage("Format email tidak valid.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password wajib diisi.")
            .MinimumLength(8).WithMessage("Password minimal 8 karakter.")
            .Matches(@"[A-Z]").WithMessage("Password harus mengandung minimal satu huruf kapital.")
            .Matches(@"[a-z]").WithMessage("Password harus mengandung minimal satu huruf kecil.")
            .Matches(@"[0-9]").WithMessage("Password harus mengandung minimal satu angka.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Konfirmasi password tidak cocok dengan password.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Nomor telepon wajib diisi.")
            .Matches(@"^[0-9+]+$").WithMessage("Nomor telepon hanya boleh berisi angka dan tanda +.");
    }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RegisterCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new ConflictException("Email ini sudah terdaftar. Silakan gunakan email lain atau login.");
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            Role = "Customer",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description });
            throw new ValidationException(errors);
        }

        await _userManager.AddToRoleAsync(user, "Customer");

        return new RegisterResponse(user.Id, user.Email, user.FullName, user.Role);
    }
}
