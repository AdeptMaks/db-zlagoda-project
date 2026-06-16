using System.Data;
using Api.Data.Entities;
using Api.Data.Repositories.Interfaces;
using Dapper;

namespace Api.Data.Repositories;

public class StatisticsRepository(string connectionString) : BaseRepository(connectionString), IStatisticsRepository
{
    private const string PRODUCT_REVENUE_QUERY = @"
        SELECT
            p.product_name        AS ProductName,
            SUM(s.product_number) AS TotalQuantity,
            SUM(s.product_number * s.selling_price) AS TotalRevenue
        FROM sale s
        JOIN store_product sp ON sp.""UPC"" = s.""UPC""
        JOIN product p        ON p.id_product = sp.id_product
        JOIN store_check c    ON c.check_number = s.check_number
        WHERE (@EmployeeId IS NULL OR c.id_employee = @EmployeeId)
          AND (@From IS NULL OR c.print_date >= @From)
          AND (@To IS NULL OR c.print_date <= @To)
        GROUP BY p.product_name
        ORDER BY TotalRevenue DESC
    ";

    private const string CATEGORY_BUYERS_QUERY = @"
        SELECT
            cc.card_number  AS CardNumber,
            cc.cust_surname AS Surname,
            cc.cust_name    AS Firstname
        FROM customer_card cc
        WHERE NOT EXISTS (
            SELECT *
            FROM store_product sp
            JOIN product p ON p.id_product = sp.id_product
            WHERE p.category_number = @CategoryNumber
              AND NOT EXISTS (
                  SELECT *
                  FROM store_check c
                  JOIN sale s ON s.check_number = c.check_number
                  WHERE c.card_number = cc.card_number
                    AND s.""UPC"" = sp.""UPC""
              )
        )
        ORDER BY cc.cust_surname
    ";

    public async Task<IEnumerable<ProductRevenueEntity>> GetProductRevenue(
        string? employeeId, DateTime? from, DateTime? to)
    {
        var parameters = new DynamicParameters();
        parameters.Add("EmployeeId", employeeId, DbType.String);
        parameters.Add("From", from, DbType.DateTime2);
        parameters.Add("To", to, DbType.DateTime2);
        return await QueryAsync<ProductRevenueEntity>(PRODUCT_REVENUE_QUERY, parameters);
    }

    public async Task<IEnumerable<CategoryBuyerEntity>> GetCategoryBuyers(int categoryNumber)
        => await QueryAsync<CategoryBuyerEntity>(CATEGORY_BUYERS_QUERY, new { CategoryNumber = categoryNumber });
}
