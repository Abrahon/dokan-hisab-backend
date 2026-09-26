using DokanHisab.Domain.Enums;

namespace DokanHisab.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public UserStatus Status { get; private set; }

    public bool IsEmailVerified { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    private User()
    {
    }

    public User(
        string firstName,
        string lastName,
        string email,
        string passwordHash)
    {
        Id = Guid.NewGuid();

        FirstName = firstName.Trim();

        LastName = lastName.Trim();

        Email = email.Trim().ToLowerInvariant();

        PasswordHash = passwordHash;

        Status = UserStatus.Pending;

        IsEmailVerified = false;

        CreatedAt = DateTime.UtcNow;

        UpdatedAt = DateTime.UtcNow;
    }

    public void VerifyEmail()
    {
        IsEmailVerified = true;

        Status = UserStatus.Active;

        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePassword(string passwordHash)
    {
        PasswordHash = passwordHash;

        UpdatedAt = DateTime.UtcNow;
    }

    public void Suspend()
    {
        Status = UserStatus.Suspended;

        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = UserStatus.Deactivated;

        UpdatedAt = DateTime.UtcNow;
    }
}