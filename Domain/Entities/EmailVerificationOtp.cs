namespace DokanHisab.Domain.Entities;

public class EmailVerificationOtp
{
    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public string CodeHash { get; private set; } = string.Empty;

    public DateTime ExpiresAt { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? VerifiedAt { get; private set; }

    public int FailedAttempts { get; private set; }

    public User User { get; private set; } = null!;

    private EmailVerificationOtp()
    {
    }

    public EmailVerificationOtp(
        Guid userId,
        string codeHash,
        DateTime expiresAt)
    {
        Id = Guid.NewGuid();

        UserId = userId;

        CodeHash = codeHash;

        ExpiresAt = expiresAt;

        CreatedAt = DateTime.UtcNow;

        FailedAttempts = 0;
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow >= ExpiresAt;
    }

    public void RegisterFailedAttempt()
    {
        FailedAttempts++;
    }

    public void MarkVerified()
    {
        VerifiedAt = DateTime.UtcNow;
    }
}