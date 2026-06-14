using Api.Features.Auth;
using Api.Models.Utils;

namespace Api.Interfaces.Auth;

public interface IAuthService
{
    Task<Response<string>> RegisterEmployee(CreateEmployeeRequest input);
}
