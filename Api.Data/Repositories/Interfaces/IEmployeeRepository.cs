using Api.Data.Entities;

namespace Api.Data.Repositories.Interfaces;

public interface IEmployeeRepository
{
    Task<IEnumerable<EmployeeEntity>> GetAll();
    Task<IEnumerable<EmployeeEntity>> GetAllCashiers();
    Task<IEnumerable<EmployeeEntity>> SearchBySurname(string surname);
    Task<EmployeeEntity?> GetById(string id);
    Task<EmployeeEntity?> GetByUsername(string username);
    Task<bool> CheckIfUnique(string username);
    Task Create(EmployeeEntity input);
    Task Update(EmployeeEntity input);
    Task Delete(string id);
}
