using Api.Data.Repositories.Interfaces;
using Api.Features.Shared;
using Api.Interfaces.Auth;
using Api.Interfaces.Utils;
using Api.Models.Utils;

namespace Api.Features.Auth;

public class AuthService(IGenerateJWT generator, IEmployeeRepository employeeRepository, IPasswordHasher passwordHasher) : IAuthService
{
    public async Task<Response<string>> RegisterEmployee(CreateEmployeeRequest input)
    {
        var errors = new Dictionary<string, string[]>();
        var isUnique = await employeeRepository.CheckIfUnique(input.Username);

        if (!isUnique)
        {
            errors["Username"] = ["Username is already in use"];
            return Response<string>.Failure(errors);
        }

        var hashedPassword = passwordHasher.HashPassword(input.Password);

        var id = Guid.NewGuid().ToString()[0..9];

        await employeeRepository.Create(input.ToEntity(id, hashedPassword));

        var token = generator.Generate(input.Username, input.EmployeeRole);

        return Response<string>.Success(token);
    }

    public async Task<Response<string>> AuthorizeEmployee(LoginRequest input)
    {
        // validation logic
        // we need to validate password and login and then return jwt token with role taken from the db

        var role = EmpoloyeeRole.Cachier;
        var token = generator.Generate(input.Username, role);

        return Response<string>.Success(token);
    }
}
