using MediatR;
using Microsoft.EntityFrameworkCore;
using UmkmJatim.Application.Common.Interfaces;
using UmkmJatim.Application.Users.DTOs;
using UmkmJatim.Domain.Exceptions;

namespace UmkmJatim.Application.Users.Queries;

public record GetAddressesQuery : IRequest<List<AddressDto>>;

public class GetAddressesQueryHandler : IRequestHandler<GetAddressesQuery, List<AddressDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetAddressesQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<AddressDto>> Handle(GetAddressesQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId == null)
            throw new UnauthorizedException();

        var addresses = await _context.Addresses
            .AsNoTracking()
            .Where(a => a.UserId == _currentUser.UserId.Value)
            .OrderByDescending(a => a.IsDefault)
            .ThenBy(a => a.Id)
            .Select(a => new AddressDto(
                a.Id,
                a.Label,
                a.RecipientName,
                a.PhoneNumber,
                a.Street,
                a.City,
                a.Province,
                a.PostalCode,
                a.IsDefault
            ))
            .ToListAsync(cancellationToken);

        return addresses;
    }
}
