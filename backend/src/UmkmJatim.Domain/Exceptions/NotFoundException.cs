namespace UmkmJatim.Domain.Exceptions;

public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string entityName, object key)
        : base($"Entitas '{entityName}' dengan kunci ({key}) tidak ditemukan.")
    {
    }
}
