using Api.Features.Auth;
using Api.Interfaces.Auth;

namespace Api.Data.Repositories;

public class UserRepository : ICreateEmployeeRepository
{
    // TODO: resolve the way Dapper executes queries
    private static string QUERY_CREATE_EMPLOYEE = @"
        INSERT INTO users (username, password, firstname, surname, patronymic, salary, start_date, birth_date, phone_number, city, street, zip_code)
        VALUES ()
    ";

    public async Task<bool> CreateEmployee(CreateEmployeeRequest input, string role)
    {
        return false;
    }
}
