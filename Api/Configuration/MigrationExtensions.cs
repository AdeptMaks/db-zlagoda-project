using Api.Data.Migrations;
using FluentMigrator.Runner;

namespace Api.Configuration;

public static class MigrationExtensions
{
    public static IServiceCollection AddMigrations(this IServiceCollection services, string connectionString)
    {
        services
            .AddFluentMigratorCore()
            .ConfigureRunner(r => r
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(Init).Assembly).For.Migrations())
            .AddLogging(l => l.AddConsole());

        return services;
    }

    public static async Task<WebApplication> ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        runner.MigrateDown(0);
        
        runner.MigrateUp();

        return app;
    }
}
