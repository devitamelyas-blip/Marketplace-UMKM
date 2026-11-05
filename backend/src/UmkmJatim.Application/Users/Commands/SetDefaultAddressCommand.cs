using MediatR;
using Microsoft.EntityFrameworkCore;
using UmkmJatim.Application.Common.Interfaces;
using UmkmJatim.Domain.Exceptions;

namespace UmkmJatim.Application.Users.Commands;

public record SetDefaultAddressCommand(int Id) : IRequest<Unit>;

public class SetDefaultAddressCommandHandler : IRequestHandler<SetDefaultAddressCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public SetDefaultAddressCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(SetDefaultAddressCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId == null)
            throw new UnauthorizedException();

        var userId = _currentUser.UserId.Value;

        var targetAddress = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == request.Id && a.UserId == userId, cancellationToken);

        if (targetAddress == null)
            throw new NotFoundException("Address", request.Id);

        var currentDefaults = await _context.Addresses
            .Where(a => a.UserId == userId && a.IsDefault)
            .ToListAsync(cancellationToken);

        foreach (var addr in currentDefaults)
        {
            addr.IsDefault = false;
        }

        targetAddress.IsDefault = true;
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
