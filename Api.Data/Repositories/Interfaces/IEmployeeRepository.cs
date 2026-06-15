using Api.Data.Entities;

namespace Api.Data.Repositories.Interfaces;

public interface IEmployeeRepository
{
    Task Create(EmployeeEntity input);
    Task<bool> CheckIfUnique(string login);
}
