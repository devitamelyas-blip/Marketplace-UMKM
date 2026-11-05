namespace UmkmJatim.Application.Users.DTOs;

public record AddressDto(
    int Id,
    string Label,
    string RecipientName,
    string PhoneNumber,
    string Street,
    string City,
    string Province,
    string PostalCode,
    bool IsDefault
);

public record CreateAddressDto(
    string Label,
    string RecipientName,
    string PhoneNumber,
    string Street,
    string City,
    string Province,
    string PostalCode,
    bool IsDefault
);
