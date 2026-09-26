using DokanHisab.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DokanHisab.Data.Configurations;

public class EmailVerificationOtpConfiguration
    : IEntityTypeConfiguration<EmailVerificationOtp>
{
    public void Configure(
        EntityTypeBuilder<EmailVerificationOtp> builder)
    {
        builder.ToTable("email_verification_otps");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.CodeHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.ExpiresAt)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.VerifiedAt)
            .IsRequired(false);

        builder.Property(x => x.FailedAttempts)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.UserId,
            x.CreatedAt
        });

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}