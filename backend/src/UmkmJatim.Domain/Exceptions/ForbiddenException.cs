namespace UmkmJatim.Domain.Exceptions;

public class ForbiddenException : DomainException
{
    public ForbiddenException(string message = "Anda tidak memiliki hak akses untuk sumber daya ini.")
        : base(message)
    {
    }
}
