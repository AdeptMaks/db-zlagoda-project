using Api.Data.Entities;
using Api.Data.Repositories.Interfaces;

namespace Api.Data.Repositories;

public class EmployeeRepository(string connectionString) : BaseRepository(connectionString), IEmployeeRepository
{
    public async Task<bool> CheckIfUnique(string login)
    {
        return true;
    }
    public async Task Create(EmployeeEntity input)
    {
        return;
    }
}