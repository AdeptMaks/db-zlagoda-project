using Api.Features.Auth;
using Api.Models;

namespace Api.Interfaces.Auth;

public interface IAuthService
{
    Task<Response<string>> CreateEmployee(CreateEmployeeRequest input);
}