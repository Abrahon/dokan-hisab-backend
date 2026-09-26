using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DokanHisab.Data;

public class ApplicationDbContextFactory
    : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Load .env from the project directory
        Env.Load();

        var host =
            Environment.GetEnvironmentVariable("DB_HOST");

        var port =
            Environment.GetEnvironmentVariable("DB_PORT");

        var database =
            Environment.GetEnvironmentVariable("DB_NAME");

        var username =
            Environment.GetEnvironmentVariable("DB_USERNAME");

        var password =
            Environment.GetEnvironmentVariable("DB_PASSWORD");

        if (string.IsNullOrWhiteSpace(host))
            throw new InvalidOperationException(
                "DB_HOST is missing from .env");

        if (string.IsNullOrWhiteSpace(port))
            throw new InvalidOperationException(
                "DB_PORT is missing from .env");

        if (string.IsNullOrWhiteSpace(database))
            throw new InvalidOperationException(
                "DB_NAME is missing from .env");

        if (string.IsNullOrWhiteSpace(username))
            throw new InvalidOperationException(
                "DB_USERNAME is missing from .env");

        if (string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException(
                "DB_PASSWORD is missing from .env");

        var connectionString =
            $"Host={host};" +
            $"Port={port};" +
            $"Database={database};" +
            $"Username={username};" +
            $"Password={password}";

        var optionsBuilder =
            new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDbContext(
            optionsBuilder.Options);
    }
}