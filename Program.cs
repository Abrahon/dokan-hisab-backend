using DotNetEnv;
using DokanHisab.Data;
using DokanHisab.Features.Authentication.Otp;
using DokanHisab.Features.Authentication.Register;
using DokanHisab.Infrastructure.Authentication.Email;
using DokanHisab.Infrastructure.Authentication.Otp;
using DokanHisab.Infrastructure.Authentication.Password;
using Microsoft.EntityFrameworkCore;

// Load .env file
Env.Load();

// TEMPORARY: Check .env values
Console.WriteLine(
    $"EMAIL_USERNAME: {Environment.GetEnvironmentVariable("EMAIL_USERNAME")}"
);

Console.WriteLine(
    $"EMAIL_HOST: {Environment.GetEnvironmentVariable("EMAIL_HOST")}"
);

var builder = WebApplication.CreateBuilder(args);

// OpenAPI
builder.Services.AddOpenApi();

// Database
var connectionString =
    $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
    $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
    $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
    $"Username={Environment.GetEnvironmentVariable("DB_USERNAME")};" +
    $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// Password hashing
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// OTP service
builder.Services.AddScoped<IOtpService, OtpService>();

// Email configuration
builder.Services.Configure<EmailOptions>(options =>
{
    options.Host =
        Environment.GetEnvironmentVariable("EMAIL_HOST")
        ?? string.Empty;

    options.Port =
        int.TryParse(
            Environment.GetEnvironmentVariable("EMAIL_PORT"),
            out var port)
            ? port
            : 587;

    options.Username =
        Environment.GetEnvironmentVariable("EMAIL_USERNAME")
        ?? string.Empty;

    options.Password =
        Environment.GetEnvironmentVariable("EMAIL_PASSWORD")
        ?? string.Empty;

    options.FromName =
        Environment.GetEnvironmentVariable("EMAIL_FROM_NAME")
        ?? "DokanHisab";

    options.FromEmail =
        Environment.GetEnvironmentVariable("EMAIL_FROM_EMAIL")
        ?? string.Empty;

    options.UseSsl =
        bool.TryParse(
            Environment.GetEnvironmentVariable("EMAIL_USE_SSL"),
            out var useSsl)
            && useSsl;
});

// Email service
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

// OpenAPI
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Authentication endpoints
RegisterEndpoint.MapRegisterEndpoint(app);

app.Run();