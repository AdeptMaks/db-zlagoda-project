using Api.Interfaces.Auth;
using Api.Interfaces.Utils;
using Api.Models;

namespace Api.Features.Auth;

public class AuthService(ICreateEmployeeRepository employeeRepository, IGenerateJWT generator) : IAuthService
{
    public async Task<Response<string>> CreateEmployee(CreateEmployeeRequest input)
    {
        var role = GetRole(input.EmployeeRole);
        if (string.IsNullOrWhiteSpace(role))
        {
            return Response<string>.Failure(new()
            {
                ["role"] = ["Role is out of available range of values"]
            });
        }

        var result = await employeeRepository.CreateEmployee(input, role);

        var token = generator.Generate(input);

        return Response<string>.Success(token);
    }

    private static string GetRole(EmpoloyeeRole role)
    {
        return role switch
        {
            EmpoloyeeRole.Cachier => "cachier",
            EmpoloyeeRole.Manager => "manager",
            _ => ""
        };
    }
}