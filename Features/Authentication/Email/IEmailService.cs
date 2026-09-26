namespace DokanHisab.Infrastructure.Authentication.Email;

public interface IEmailService
{
    Task SendEmailVerificationOtpAsync(
        string email,
        string firstName,
        string otp,
        CancellationToken cancellationToken = default);
}