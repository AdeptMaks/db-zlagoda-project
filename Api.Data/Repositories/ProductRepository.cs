using Api.Data.Entities;
using Api.Data.Repositories.Interfaces;

namespace Api.Data.Repositories;

public class ProductRepository(string connectionString) : BaseRepository(connectionString), IProductRepository
{
    private const string BASE_SELECT = @"
        SELECT
            id_product      AS ProductId,
            category_number AS CategoryNumber,
            product_name    AS ProductName,
            characteristics AS Characteristics
        FROM product
    ";

    private const string GET_ALL_QUERY = $"{BASE_SELECT} ORDER BY product_name";

    private const string GET_BY_CATEGORY_QUERY =
        $"{BASE_SELECT} WHERE category_number = @CategoryNumber ORDER BY product_name";

    private const string SEARCH_BY_NAME_QUERY =
        $"{BASE_SELECT} WHERE product_name ILIKE '%' || @Name || '%' ORDER BY product_name";

    private const string GET_BY_ID_QUERY = $"{BASE_SELECT} WHERE id_product = @ProductId";

    private const string NEXT_ID_QUERY = @"
        SELECT COALESCE(MAX(id_product), 0) + 1 FROM product
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

    public async Task<IEnumerable<ProductEntity>> GetByCategory(int categoryNumber)
        => await QueryAsync<ProductEntity>(GET_BY_CATEGORY_QUERY, new { CategoryNumber = categoryNumber });

    public async Task<IEnumerable<ProductEntity>> SearchByName(string name)
        => await QueryAsync<ProductEntity>(SEARCH_BY_NAME_QUERY, new { Name = name });

    public async Task<ProductEntity?> GetById(int id)
        => await QuerySingleAsync<ProductEntity>(GET_BY_ID_QUERY, new { ProductId = id });

    public async Task<int> GetNextId()
        => await QuerySingleAsync<int>(NEXT_ID_QUERY);

    public async Task Create(ProductEntity input)
        => await ExecuteAsync(CREATE_QUERY, input);

    public async Task Update(ProductEntity input)
        => await ExecuteAsync(UPDATE_QUERY, input);

    public async Task Delete(int id)
        => await ExecuteAsync(DELETE_QUERY, new { ProductId = id });
}
