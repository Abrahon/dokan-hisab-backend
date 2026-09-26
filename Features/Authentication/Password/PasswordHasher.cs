using Microsoft.AspNetCore.Identity;

namespace DokanHisab.Infrastructure.Authentication.Password;

public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string passwordHash);
}

public sealed class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<object> _hasher = new();

    public string Hash(string password)
    {
        return _hasher.HashPassword(
            new object(),
            password);
    }

    public bool Verify(string password, string passwordHash)
    {
        var result = _hasher.VerifyHashedPassword(
            new object(),
            passwordHash,
            password);

        return result == PasswordVerificationResult.Success ||
               result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}