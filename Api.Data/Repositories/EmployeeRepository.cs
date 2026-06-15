using Api.Data.Entities;
using Api.Data.Repositories.Interfaces;

namespace Api.Data.Repositories;

public class EmployeeRepository(string connectionString) : BaseRepository(connectionString), IEmployeeRepository
{
    private const string BASE_SELECT = @"
        SELECT
            id_employee     AS EmployeeId,
            username        AS Username,
            empl_role       AS EmployeeRole,
            empl_surname    AS Surname,
            empl_name       AS Firstname,
            empl_patronymic AS Patronymic,
            salary          AS Salary,
            date_of_birth   AS BirthDate,
            date_of_start   AS StartDate,
            phone_number    AS PhoneNumber,
            city            AS City,
            street          AS Street,
            zip_code        AS ZipCode
        FROM employee
    ";

    private const string GET_ALL_QUERY = $"{BASE_SELECT} ORDER BY empl_surname";

    private const string GET_CASHIERS_QUERY = $"{BASE_SELECT} WHERE empl_role = 'Cachier' ORDER BY empl_surname";

    private const string GET_BY_ID_QUERY = $"{BASE_SELECT} WHERE id_employee = @EmployeeId";

    private const string GET_BY_USERNAME_QUERY = @"
        SELECT
            id_employee     AS EmployeeId,
            username        AS Username,
            password        AS Password,
            empl_role       AS EmployeeRole,
            empl_surname    AS Surname,
            empl_name       AS Firstname,
            empl_patronymic AS Patronymic,
            salary          AS Salary,
            date_of_birth   AS BirthDate,
            date_of_start   AS StartDate,
            phone_number    AS PhoneNumber,
            city            AS City,
            street          AS Street,
            zip_code        AS ZipCode
        FROM employee
        WHERE username = @Username
    ";

    private const string SEARCH_BY_SURNAME_QUERY = $"{BASE_SELECT} WHERE empl_surname = @Surname";

    private const string CHECK_IF_UNIQUE_QUERY = @"
        SELECT COUNT(1) FROM employee WHERE username = @Username
    ";

    private const string CREATE_QUERY = @"
        INSERT INTO employee
            (id_employee, username, password, empl_surname, empl_name, empl_patronymic, empl_role,
             salary, date_of_birth, date_of_start, phone_number, city, street, zip_code)
        VALUES
            (@EmployeeId, @Username, @Password, @Surname, @Firstname, @Patronymic, @EmployeeRole,
             @Salary, @BirthDate, @StartDate, @PhoneNumber, @City, @Street, @ZipCode)
    ";

    private const string UPDATE_QUERY = @"
        UPDATE employee SET
            empl_surname    = @Surname,
            empl_name       = @Firstname,
            empl_patronymic = @Patronymic,
            empl_role       = @EmployeeRole,
            salary          = @Salary,
            date_of_birth   = @BirthDate,
            date_of_start   = @StartDate,
            phone_number    = @PhoneNumber,
            city            = @City,
            street          = @Street,
            zip_code        = @ZipCode
        WHERE id_employee = @EmployeeId
    ";

    private const string DELETE_QUERY = @"
        DELETE FROM employee WHERE id_employee = @EmployeeId
    ";

    public async Task<IEnumerable<EmployeeEntity>> GetAll()
        => await QueryAsync<EmployeeEntity>(GET_ALL_QUERY);

    public async Task<IEnumerable<EmployeeEntity>> GetAllCashiers()
        => await QueryAsync<EmployeeEntity>(GET_CASHIERS_QUERY);

    public async Task<IEnumerable<EmployeeEntity>> SearchBySurname(string surname)
        => await QueryAsync<EmployeeEntity>(SEARCH_BY_SURNAME_QUERY, new { Surname = surname });

    public async Task<EmployeeEntity?> GetById(string id)
        => await QuerySingleAsync<EmployeeEntity>(GET_BY_ID_QUERY, new { EmployeeId = id });

    public async Task<EmployeeEntity?> GetByUsername(string username)
        => await QuerySingleAsync<EmployeeEntity>(GET_BY_USERNAME_QUERY, new { Username = username });

    public async Task<bool> CheckIfUnique(string username)
    {
        var count = await QuerySingleAsync<int>(CHECK_IF_UNIQUE_QUERY, new { Username = username });
        return count == 0;
    }

    public async Task Create(EmployeeEntity input)
        => await ExecuteAsync(CREATE_QUERY, input);

    public async Task Update(EmployeeEntity input)
        => await ExecuteAsync(UPDATE_QUERY, input);

    public async Task Delete(string id)
        => await ExecuteAsync(DELETE_QUERY, new { EmployeeId = id });
}
