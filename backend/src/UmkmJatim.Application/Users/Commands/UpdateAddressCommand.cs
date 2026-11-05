using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UmkmJatim.Application.Common.Interfaces;
using UmkmJatim.Application.Users.DTOs;
using UmkmJatim.Domain.Exceptions;

namespace UmkmJatim.Application.Users.Commands;

public record UpdateAddressCommand(
    int Id,
    string Label,
    string RecipientName,
    string PhoneNumber,
    string Street,
    string City,
    string Province,
    string PostalCode,
    bool IsDefault
) : IRequest<AddressDto>;

public class UpdateAddressCommandValidator : AbstractValidator<UpdateAddressCommand>
{
    public UpdateAddressCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Label).NotEmpty().MaximumLength(50);
        RuleFor(x => x.RecipientName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^[0-9+]+$");
        RuleFor(x => x.Street).NotEmpty().MaximumLength(255);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Province).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PostalCode).NotEmpty().Length(5).Matches(@"^[0-9]+$");
    }
}

public class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, AddressDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public UpdateAddressCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<AddressDto> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId == null)
            throw new UnauthorizedException();

        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (address == null)
            throw new NotFoundException("Address", request.Id);

        if (address.UserId != _currentUser.UserId.Value)
            throw new ForbiddenException("Anda tidak memiliki akses ke alamat ini.");

        if (request.IsDefault && !address.IsDefault)
        {
            var existingDefaults = await _context.Addresses
                .Where(a => a.UserId == address.UserId && a.IsDefault)
                .ToListAsync(cancellationToken);

            foreach (var a in existingDefaults)
            {
                a.IsDefault = false;
            }
        }

        address.Label = request.Label.Trim();
        address.RecipientName = request.RecipientName.Trim();
        address.PhoneNumber = request.PhoneNumber.Trim();
        address.Street = request.Street.Trim();
        address.City = request.City.Trim();
        address.Province = request.Province.Trim();
        address.PostalCode = request.PostalCode.Trim();
        address.IsDefault = request.IsDefault;

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
