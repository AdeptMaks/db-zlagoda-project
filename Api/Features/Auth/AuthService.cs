using System.ComponentModel.DataAnnotations;
using Api.Features.Shared;
using Api.Interfaces.Auth;
using Api.Interfaces.Utils;
using Api.Models.Utils;

namespace Api.Features.Auth;

public class AuthService(IGenerateJWT generator) : IAuthService
{
    public async Task<Response<string>> RegisterEmployee(CreateEmployeeRequest input)
    {
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
