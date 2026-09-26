
using DokanHisab.Data;
using DokanHisab.Domain.Entities;
using DokanHisab.Infrastructure.Authentication.Email;
using DokanHisab.Infrastructure.Authentication.Otp;
using DokanHisab.Infrastructure.Authentication.Password;
using Microsoft.EntityFrameworkCore;

namespace DokanHisab.Features.Authentication.Register;

public static class RegisterEndpoint
{
    public static void MapRegisterEndpoint(
        IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
            "/api/auth/register",
            async (
                RegisterRequest request,
                ApplicationDbContext dbContext,
                IPasswordHasher passwordHasher,
                IOtpService otpService,
                IEmailService emailService,
                CancellationToken cancellationToken) =>
            {
                // -----------------------------
                // Validation
                // -----------------------------

                if (string.IsNullOrWhiteSpace(request.FirstName))
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = "First name is required."
                    });
                }

                if (string.IsNullOrWhiteSpace(request.LastName))
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = "Last name is required."
                    });
                }

                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = "Email is required."
                    });
                }

                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = "Password is required."
                    });
                }

                if (request.Password.Length < 8)
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        message = "Password must be at least 8 characters."
                    });
                }

                // -----------------------------
                // Normalize email
                // -----------------------------

                var normalizedEmail =
                    request.Email.Trim().ToLowerInvariant();

                // -----------------------------
                // Check existing user
                // -----------------------------

                var existingUser = await dbContext.Users
                    .FirstOrDefaultAsync(
                        user => user.Email == normalizedEmail,
                        cancellationToken);

                // -----------------------------
                // Verified user
                // -----------------------------

                if (existingUser is not null &&
                    existingUser.IsEmailVerified)
                {
                    return Results.Conflict(new
                    {
                        success = false,
                        message = "An account with this email already exists."
                    });
                }

                // -----------------------------
                // Existing but unverified user
                // -----------------------------

                if (existingUser is not null &&
                    !existingUser.IsEmailVerified)
                {
                    var otp = otpService.GenerateCode();
                    var otpHash = otpService.HashCode(otp);

                    var expiresAt =
                        DateTime.UtcNow.AddMinutes(10);

                    var verificationOtp =
                        new EmailVerificationOtp(
                            existingUser.Id,
                            otpHash,
                            expiresAt);

                    dbContext.EmailVerificationOtps.Add(
                        verificationOtp);

                    await dbContext.SaveChangesAsync(
                        cancellationToken);

                    await emailService.SendEmailVerificationOtpAsync(
                        existingUser.Email,
                        existingUser.FirstName,
                        otp,
                        cancellationToken);

                    return Results.Ok(new
                    {
                        success = true,
                        message =
                            "This email is already registered but not verified. A new verification code has been sent."
                    });
                }

                // -----------------------------
                // Create new user
                // -----------------------------

                var passwordHash =
                    passwordHasher.Hash(request.Password);

                var user = new User(
                    request.FirstName,
                    request.LastName,
                    normalizedEmail,
                    passwordHash);

                dbContext.Users.Add(user);

                // -----------------------------
                // Generate OTP
                // -----------------------------

                var verificationCode =
                    otpService.GenerateCode();

                var verificationCodeHash =
                    otpService.HashCode(
                        verificationCode);

                var otpExpiresAt =
                    DateTime.UtcNow.AddMinutes(10);

                var emailVerificationOtp =
                    new EmailVerificationOtp(
                        user.Id,
                        verificationCodeHash,
                        otpExpiresAt);

                dbContext.EmailVerificationOtps.Add(
                    emailVerificationOtp);

                // -----------------------------
                // Save user + OTP
                // -----------------------------

                await dbContext.SaveChangesAsync(
                    cancellationToken);

                // -----------------------------
                // Send verification email
                // -----------------------------

                await emailService.SendEmailVerificationOtpAsync(
                    user.Email,
                    user.FirstName,
                    verificationCode,
                    cancellationToken);

                // -----------------------------
                // Response
                // -----------------------------

                var response = new RegisterResponse(
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.Status.ToString(),
                    user.IsEmailVerified);

                return Results.Created(
                    $"/api/auth/users/{user.Id}",
                    new
                    {
                        success = true,
                        message =
                            "Registration successful. A verification code has been sent to your email.",
                        data = response
                    });
            });
    }
}

