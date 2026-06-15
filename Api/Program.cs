using System.Text.Json.Serialization;
using Api.Configuration;
using Api.Data.Repositories;
using Api.Data.Repositories.Interfaces;
using Api.Features.Auth;
using Api.Interfaces.Auth;
using Api.Interfaces.Utils;
using Api.Models.Utils;
using Api.Services;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Postgres")!;

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
var jwtOptions = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>()!;



builder.Services.AddMigrations(connectionString);
builder.Services.AddSwagger();

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddValidatorsFromAssemblyContaining<CreateEmployeeRequestValidator>();
builder.Services.AddOptions();



builder.Services.AddScoped<IEmployeeRepository>(_ => new EmployeeRepository(connectionString));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<IGenerateJWT, JwtGenerator>();
builder.Services.AddApiAuthentication(jwtOptions);




var app = builder.Build();

if (Environment.GetEnvironmentVariable("MIGRATE_ONLY") == "true")
{
    await app.ApplyMigrations();
    return;
}


app.UseSwaggerWithUi();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
