namespace DokanHisab.Infrastructure.Authentication.Otp;

public interface IOtpService
{
    string GenerateCode();

    string HashCode(string code);

    bool VerifyCode(string code, string hash);
}