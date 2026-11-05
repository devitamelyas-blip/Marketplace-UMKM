using MediatR;
using Microsoft.EntityFrameworkCore;
using UmkmJatim.Application.Common.Interfaces;
using UmkmJatim.Domain.Exceptions;

namespace UmkmJatim.Application.Users.Commands;

public record DeleteAddressCommand(int Id) : IRequest<Unit>;

public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public DeleteAddressCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId == null)
            throw new UnauthorizedException();

        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (address == null)
            throw new NotFoundException("Address", request.Id);

        if (address.UserId != _currentUser.UserId.Value)
            throw new ForbiddenException("Anda tidak memiliki izin menghapus alamat ini.");

        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
