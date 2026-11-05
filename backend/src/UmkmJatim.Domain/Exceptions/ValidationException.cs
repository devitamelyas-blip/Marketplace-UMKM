namespace UmkmJatim.Domain.Exceptions;

public class ValidationException : DomainException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(string message = "Satu atau lebih kesalahan validasi terjadi.")
        : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("Satu atau lebih kesalahan validasi terjadi.")
    {
        Errors = errors;
    }
}
