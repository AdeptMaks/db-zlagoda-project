using Api.Data.Entities;
using Api.Data.Repositories.Interfaces;

namespace Api.Data.Repositories;

public class StoreCheckRepository(string connectionString) : BaseRepository(connectionString), IStoreCheckRepository
{
    private const string GET_ALL_QUERY = @"
        SELECT
            check_number AS CheckNumber,
            id_employee  AS EmployeeId,
            card_number  AS CardNumber,
            print_date   AS PrintDate,
            sum_total    AS SumTotal,
            vat          AS Vat
        FROM store_check
    ";

    private const string GET_BY_ID_QUERY = @"
        SELECT
            check_number AS CheckNumber,
            id_employee  AS EmployeeId,
            card_number  AS CardNumber,
            print_date   AS PrintDate,
            sum_total    AS SumTotal,
            vat          AS Vat
        FROM store_check
        WHERE check_number = @CheckNumber
    ";

    private const string CREATE_QUERY = @"
        INSERT INTO store_check (check_number, id_employee, card_number, print_date, sum_total, vat)
        VALUES (@CheckNumber, @EmployeeId, @CardNumber, @PrintDate, @SumTotal, @Vat)
    ";

    private const string DELETE_QUERY = @"
        DELETE FROM store_check WHERE check_number = @CheckNumber
    ";

    public async Task<IEnumerable<StoreCheckEntity>> GetAll()
        => await QueryAsync<StoreCheckEntity>(GET_ALL_QUERY);

    public async Task<StoreCheckEntity?> GetById(string checkNumber)
        => await QuerySingleAsync<StoreCheckEntity>(GET_BY_ID_QUERY, new { CheckNumber = checkNumber });

    public async Task Create(StoreCheckEntity input)
        => await ExecuteAsync(CREATE_QUERY, input);

    public async Task Delete(string checkNumber)
        => await ExecuteAsync(DELETE_QUERY, new { CheckNumber = checkNumber });
}
