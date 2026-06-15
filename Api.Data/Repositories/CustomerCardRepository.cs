using Api.Data.Entities;
using Api.Data.Repositories.Interfaces;

namespace Api.Data.Repositories;

public class CustomerCardRepository(string connectionString) : BaseRepository(connectionString), ICustomerCardRepository
{
    private const string GET_ALL_QUERY = @"
        SELECT
            card_number     AS CardNumber,
            cust_surname    AS Surname,
            cust_name       AS Firstname,
            cust_patronymic AS Patronymic,
            phone_number    AS PhoneNumber,
            city            AS City,
            street          AS Street,
            zip_code        AS ZipCode,
            percent         AS Percent
        FROM customer_card
    ";

    private const string GET_BY_ID_QUERY = @"
        SELECT
            card_number     AS CardNumber,
            cust_surname    AS Surname,
            cust_name       AS Firstname,
            cust_patronymic AS Patronymic,
            phone_number    AS PhoneNumber,
            city            AS City,
            street          AS Street,
            zip_code        AS ZipCode,
            percent         AS Percent
        FROM customer_card
        WHERE card_number = @CardNumber
    ";

    private const string CREATE_QUERY = @"
        INSERT INTO customer_card
            (card_number, cust_surname, cust_name, cust_patronymic,
             phone_number, city, street, zip_code, percent)
        VALUES
            (@CardNumber, @Surname, @Firstname, @Patronymic,
             @PhoneNumber, @City, @Street, @ZipCode, @Percent)
    ";

    private const string UPDATE_QUERY = @"
        UPDATE customer_card SET
            cust_surname    = @Surname,
            cust_name       = @Firstname,
            cust_patronymic = @Patronymic,
            phone_number    = @PhoneNumber,
            city            = @City,
            street          = @Street,
            zip_code        = @ZipCode,
            percent         = @Percent
        WHERE card_number = @CardNumber
    ";

    private const string DELETE_QUERY = @"
        DELETE FROM customer_card WHERE card_number = @CardNumber
    ";

    public async Task<IEnumerable<CustomerCardEntity>> GetAll()
        => await QueryAsync<CustomerCardEntity>(GET_ALL_QUERY);

    public async Task<CustomerCardEntity?> GetById(string id)
        => await QuerySingleAsync<CustomerCardEntity>(GET_BY_ID_QUERY, new { CardNumber = id });

    public async Task Create(CustomerCardEntity input)
        => await ExecuteAsync(CREATE_QUERY, input);

    public async Task Update(CustomerCardEntity input)
        => await ExecuteAsync(UPDATE_QUERY, input);

    public async Task Delete(string id)
        => await ExecuteAsync(DELETE_QUERY, new { CardNumber = id });
}
