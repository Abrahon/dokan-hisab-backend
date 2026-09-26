using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace DokanHisab.Infrastructure.Authentication.Email;

public sealed class EmailService : IEmailService
{
    private readonly EmailOptions _options;

    public EmailService(
        IOptions<EmailOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendEmailVerificationOtpAsync(
        string email,
        string firstName,
        string otp,
        CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();

        message.From.Add(
            new MailboxAddress(
                _options.FromName,
                _options.FromEmail));

        message.To.Add(
            MailboxAddress.Parse(email));

        message.Subject = "DokanHisab Email Verification";

        message.Body = new BodyBuilder
        {
            HtmlBody = $"""
                <div style="font-family: Arial, sans-serif;">
                    <h2>Welcome to DokanHisab, {firstName}!</h2>

                    <p>
                        Thank you for creating your account.
                    </p>

                    <p>
                        Your email verification code is:
                    </p>

                    <h1 style="letter-spacing: 8px;">
                        {otp}
                    </h1>

                    <p>
                        This code will expire in
                        <strong>10 minutes</strong>.
                    </p>

                    <p>
                        If you did not create this account,
                        you can safely ignore this email.
                    </p>

                    <p>
                        Regards,<br />
                        DokanHisab Team
                    </p>
                </div>
                """
        }.ToMessageBody();

        using var client = new SmtpClient();

        var socketOption = _options.UseSsl
            ? SecureSocketOptions.SslOnConnect
            : SecureSocketOptions.StartTls;

        await client.ConnectAsync(
            _options.Host,
            _options.Port,
            socketOption,
            cancellationToken);

        await client.AuthenticateAsync(
            _options.Username,
            _options.Password,
            cancellationToken);

        await client.SendAsync(
            message,
            cancellationToken);

        await client.DisconnectAsync(
            true,
            cancellationToken);
    }
}