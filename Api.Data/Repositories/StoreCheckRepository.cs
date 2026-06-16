using System.Data;
using Api.Data.Entities;
using Api.Data.Repositories.Interfaces;
using Dapper;

namespace Api.Data.Repositories;

public class StoreCheckRepository(string connectionString) : BaseRepository(connectionString), IStoreCheckRepository
{
    private const string BASE_SELECT = @"
        SELECT
            check_number AS CheckNumber,
            id_employee  AS EmployeeId,
            card_number  AS CardNumber,
            print_date   AS PrintDate,
            sum_total    AS SumTotal,
            vat          AS Vat
        FROM store_check
    ";

    private const string GET_BY_ID_QUERY = $"{BASE_SELECT} WHERE check_number = @CheckNumber";

    private const string NEXT_NUMBER_QUERY = @"
        SELECT 'CHK' || LPAD((COALESCE(MAX(CAST(SUBSTRING(check_number FROM 4) AS INTEGER)), 0) + 1)::text, 7, '0')
        FROM store_check
    ";

    private const string CREATE_QUERY = @"
        INSERT INTO store_check (check_number, id_employee, card_number, print_date, sum_total, vat)
        VALUES (@CheckNumber, @EmployeeId, @CardNumber, @PrintDate, @SumTotal, @Vat)
    ";

    private const string SALE_INSERT_QUERY = @"
        INSERT INTO sale (""UPC"", check_number, product_number, selling_price)
        VALUES (@Upc, @CheckNumber, @ProductNumber, @SellingPrice)
    ";

    private const string DECREMENT_STOCK_QUERY = @"
        UPDATE store_product
        SET products_number = products_number - @ProductNumber
        WHERE ""UPC"" = @Upc AND products_number >= @ProductNumber
    ";

    private const string DELETE_QUERY = @"
        DELETE FROM store_check WHERE check_number = @CheckNumber
    ";

    private static (string where, DynamicParameters p) BuildFilter(
        string? employeeId, DateTime? from, DateTime? to)
    {
        var conditions = new List<string>();
        var p = new DynamicParameters();

        if (!string.IsNullOrEmpty(employeeId))
        {
            conditions.Add("id_employee = @EmployeeId");
            p.Add("EmployeeId", employeeId);
        }
        if (from.HasValue)
        {
            conditions.Add("print_date >= @From");
            p.Add("From", from.Value);
        }
        if (to.HasValue)
        {
            conditions.Add("print_date <= @To");
            p.Add("To", to.Value);
        }

        var where = conditions.Count > 0 ? " WHERE " + string.Join(" AND ", conditions) : "";
        return (where, p);
    }

    public async Task<IEnumerable<StoreCheckEntity>> GetFiltered(string? employeeId, DateTime? from, DateTime? to)
    {
        var (where, p) = BuildFilter(employeeId, from, to);
        return await QueryAsync<StoreCheckEntity>($"{BASE_SELECT}{where} ORDER BY print_date DESC", p);
    }

    public async Task<StoreCheckEntity?> GetById(string checkNumber)
        => await QuerySingleAsync<StoreCheckEntity>(GET_BY_ID_QUERY, new { CheckNumber = checkNumber });

    public async Task<string> GetNextCheckNumber()
        => await QuerySingleAsync<string>(NEXT_NUMBER_QUERY) ?? "CHK0000001";

    public async Task CreateWithSales(StoreCheckEntity check, IEnumerable<SaleEntity> sales)
    {
        using var conn = CreateConnection();
        conn.Open();
        using var tx = conn.BeginTransaction();
        try
        {
            await conn.ExecuteAsync(CREATE_QUERY, check, tx);

            foreach (var sale in sales)
            {
                await conn.ExecuteAsync(SALE_INSERT_QUERY, sale, tx);

                var affected = await conn.ExecuteAsync(
                    DECREMENT_STOCK_QUERY,
                    new { sale.Upc, sale.ProductNumber },
                    tx);

                if (affected == 0)
                    throw new InvalidOperationException($"Insufficient stock for UPC {sale.Upc}");
            }

            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    public async Task Delete(string checkNumber)
        => await ExecuteAsync(DELETE_QUERY, new { CheckNumber = checkNumber });

    public async Task<decimal> GetTotalSum(string? employeeId, DateTime? from, DateTime? to)
    {
        var (where, p) = BuildFilter(employeeId, from, to);
        return await QuerySingleAsync<decimal>(
            $"SELECT COALESCE(SUM(sum_total), 0) FROM store_check{where}", p);
    }

    public async Task<int> GetTotalProductQuantity(int productId, DateTime? from, DateTime? to)
    {
        var conditions = new List<string> { "sp.id_product = @ProductId" };
        var p = new DynamicParameters();
        p.Add("ProductId", productId);

        if (from.HasValue) { conditions.Add("c.print_date >= @From"); p.Add("From", from.Value); }
        if (to.HasValue) { conditions.Add("c.print_date <= @To"); p.Add("To", to.Value); }

        var sql = $@"
            SELECT COALESCE(SUM(s.product_number), 0)
            FROM sale s
            JOIN store_product sp ON sp.""UPC"" = s.""UPC""
            JOIN store_check c ON c.check_number = s.check_number
            WHERE {string.Join(" AND ", conditions)}";

        return await QuerySingleAsync<int>(sql, p);
    }
}
