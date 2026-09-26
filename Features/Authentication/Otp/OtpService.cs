using System.Security.Cryptography;
using System.Text;
using DokanHisab.Infrastructure.Authentication.Otp;

namespace DokanHisab.Features.Authentication.Otp;

public sealed class OtpService : IOtpService
{
    public string GenerateCode()
    {
        var number = RandomNumberGenerator.GetInt32(
            100000,
            1000000);

        return number.ToString();
    }

    public string HashCode(string code)
    {
        var bytes = SHA256.HashData(
            Encoding.UTF8.GetBytes(code));

        return Convert.ToHexString(bytes);
    }

    public bool VerifyCode(
        string code,
        string hash)
    {
        var computedHash = HashCode(code);

        return CryptographicOperations.FixedTimeEquals(
            Convert.FromHexString(computedHash),
            Convert.FromHexString(hash));
    }
}