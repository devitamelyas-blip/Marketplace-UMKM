using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UmkmJatim.Application.Common.Interfaces;
using UmkmJatim.Application.Users.DTOs;
using UmkmJatim.Domain.Entities;
using UmkmJatim.Domain.Exceptions;

namespace UmkmJatim.Application.Users.Commands;

public record CreateAddressCommand(
    string Label,
    string RecipientName,
    string PhoneNumber,
    string Street,
    string City,
    string Province,
    string PostalCode,
    bool IsDefault
) : IRequest<AddressDto>;

public class CreateAddressCommandValidator : AbstractValidator<CreateAddressCommand>
{
    public CreateAddressCommandValidator()
    {
        RuleFor(x => x.Label).NotEmpty().MaximumLength(50);
        RuleFor(x => x.RecipientName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^[0-9+]+$");
        RuleFor(x => x.Street).NotEmpty().MaximumLength(255);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Province).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PostalCode).NotEmpty().Length(5).Matches(@"^[0-9]+$");
    }
}

public class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, AddressDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public CreateAddressCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<AddressDto> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId == null)
            throw new UnauthorizedException();

        var userId = _currentUser.UserId.Value;

        // Jika alamat ini diset default, nonaktifkan default alamat lain
        if (request.IsDefault)
        {
            var existingDefaults = await _context.Addresses
                .Where(a => a.UserId == userId && a.IsDefault)
                .ToListAsync(cancellationToken);

            foreach (var addr in existingDefaults)
            {
                addr.IsDefault = false;
            }
        }
        else
        {
            // Jika ini alamat pertama, otomatis set default
            var hasAnyAddress = await _context.Addresses.AnyAsync(a => a.UserId == userId, cancellationToken);
            if (!hasAnyAddress)
            {
                // Make it default
            }
        }

        var address = new Address
        {
            UserId = userId,
            Label = request.Label.Trim(),
            RecipientName = request.RecipientName.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            Street = request.Street.Trim(),
            City = request.City.Trim(),
            Province = request.Province.Trim(),
            PostalCode = request.PostalCode.Trim(),
            IsDefault = request.IsDefault
        };

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync(cancellationToken);

        return new AddressDto(
            address.Id,
            address.Label,
            address.RecipientName,
            address.PhoneNumber,
            address.Street,
            address.City,
            address.Province,
            address.PostalCode,
            address.IsDefault
        );
    }
}
