namespace DokanHisab.Features.Authentication.Register;

public sealed record RegisterResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Status,
    bool IsEmailVerified);