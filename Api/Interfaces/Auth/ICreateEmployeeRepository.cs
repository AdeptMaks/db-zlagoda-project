using Api.Features.Auth;

namespace Api.Interfaces.Auth;

public interface ICreateEmployeeRepository
{
    Task<bool> CreateEmployee(CreateEmployeeRequest input, string role);
}