using Api.Data.Entities;
using Api.Data.Repositories.Interfaces;

namespace Api.Data.Repositories;

public class SaleRepository(string connectionString) : BaseRepository(connectionString), ISaleRepository
{
    private const string GET_BY_CHECK_QUERY = @"
        SELECT
            ""UPC""          AS Upc,
            check_number   AS CheckNumber,
            product_number AS ProductNumber,
            selling_price  AS SellingPrice
        FROM sale
        WHERE check_number = @CheckNumber
    ";

    private const string GET_DETAILED_BY_CHECK_QUERY = @"
        SELECT
            s.""UPC""          AS Upc,
            p.product_name   AS ProductName,
            s.product_number AS ProductNumber,
            s.selling_price  AS SellingPrice
        FROM sale s
        JOIN store_product sp ON sp.""UPC"" = s.""UPC""
        JOIN product p ON p.id_product = sp.id_product
        WHERE s.check_number = @CheckNumber
    ";

    private const string CREATE_QUERY = @"
        INSERT INTO sale (""UPC"", check_number, product_number, selling_price)
        VALUES (@Upc, @CheckNumber, @ProductNumber, @SellingPrice)
    ";

    private const string DELETE_QUERY = @"
        DELETE FROM sale WHERE ""UPC"" = @Upc AND check_number = @CheckNumber
    ";

    public async Task<IEnumerable<SaleEntity>> GetByCheckNumber(string checkNumber)
        => await QueryAsync<SaleEntity>(GET_BY_CHECK_QUERY, new { CheckNumber = checkNumber });

    public async Task<IEnumerable<SaleDetailsEntity>> GetDetailedByCheck(string checkNumber)
        => await QueryAsync<SaleDetailsEntity>(GET_DETAILED_BY_CHECK_QUERY, new { CheckNumber = checkNumber });

    public async Task Create(SaleEntity input)
        => await ExecuteAsync(CREATE_QUERY, input);

    public async Task Delete(string upc, string checkNumber)
        => await ExecuteAsync(DELETE_QUERY, new { Upc = upc, CheckNumber = checkNumber });
}
