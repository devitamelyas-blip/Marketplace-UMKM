using MediatR;
using Microsoft.EntityFrameworkCore;
using UmkmJatim.Application.Common.Interfaces;

namespace UmkmJatim.Application.Auth.Commands;

public record LogoutCommand(string Token) : IRequest<Unit>;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public LogoutCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
            return Unit.Value;

        var tokenRecord = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == request.Token, cancellationToken);

        if (tokenRecord != null && tokenRecord.IsActive)
        {
            tokenRecord.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}
