namespace UmkmJatim.Domain.Exceptions;

public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message = "Autentikasi gagal atau sesi telah berakhir.")
        : base(message)
    {
    }
}
