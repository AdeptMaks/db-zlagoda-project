using Api.Data.Entities;
using Api.Data.Repositories.Interfaces;

namespace Api.Data.Repositories;

public class ProductRepository(string connectionString) : BaseRepository(connectionString), IProductRepository
{
    private const string GET_ALL_QUERY = @"
        SELECT
            id_product      AS ProductId,
            category_number AS CategoryNumber,
            product_name    AS ProductName,
            characteristics AS Characteristics
        FROM product
    ";

    private const string GET_BY_ID_QUERY = @"
        SELECT
            id_product      AS ProductId,
            category_number AS CategoryNumber,
            product_name    AS ProductName,
            characteristics AS Characteristics
        FROM product
        WHERE id_product = @ProductId
    ";

    private const string CREATE_QUERY = @"
        INSERT INTO product (id_product, category_number, product_name, characteristics)
        VALUES (@ProductId, @CategoryNumber, @ProductName, @Characteristics)
    ";

    private const string UPDATE_QUERY = @"
        UPDATE product SET
            category_number = @CategoryNumber,
            product_name    = @ProductName,
            characteristics = @Characteristics
        WHERE id_product = @ProductId
    ";

    private const string DELETE_QUERY = @"
        DELETE FROM product WHERE id_product = @ProductId
    ";

    public async Task<IEnumerable<ProductEntity>> GetAll()
        => await QueryAsync<ProductEntity>(GET_ALL_QUERY);

    public async Task<ProductEntity?> GetById(int id)
        => await QuerySingleAsync<ProductEntity>(GET_BY_ID_QUERY, new { ProductId = id });

    public async Task Create(ProductEntity input)
        => await ExecuteAsync(CREATE_QUERY, input);

    public async Task Update(ProductEntity input)
        => await ExecuteAsync(UPDATE_QUERY, input);

    public async Task Delete(int id)
        => await ExecuteAsync(DELETE_QUERY, new { ProductId = id });
}
