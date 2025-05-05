namespace DT.Common.DTOs;

public record Address(
    string Line1,
    string Line2,
    string City,
    string PostalCode,
    string Country);