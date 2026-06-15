using Api.Data.Entities;
using Api.Data.Repositories.Interfaces;

namespace Api.Data.Repositories;

public class StoreProductRepository(string connectionString) : BaseRepository(connectionString), IStoreProductRepository
{
    private const string GET_ALL_QUERY = @"
        SELECT
            ""UPC""               AS Upc,
            ""UPC_prom""          AS UpcProm,
            id_product          AS ProductId,
            selling_price       AS SellingPrice,
            products_number     AS ProductsNumber,
            promotional_product AS PromotionalProduct
        FROM store_product
    ";

    private const string GET_BY_ID_QUERY = @"
        SELECT
            ""UPC""               AS Upc,
            ""UPC_prom""          AS UpcProm,
            id_product          AS ProductId,
            selling_price       AS SellingPrice,
            products_number     AS ProductsNumber,
            promotional_product AS PromotionalProduct
        FROM store_product
        WHERE ""UPC"" = @Upc
    ";

    private const string CREATE_QUERY = @"
        INSERT INTO store_product (""UPC"", ""UPC_prom"", id_product, selling_price, products_number, promotional_product)
        VALUES (@Upc, @UpcProm, @ProductId, @SellingPrice, @ProductsNumber, @PromotionalProduct)
    ";

    private const string UPDATE_QUERY = @"
        UPDATE store_product SET
            ""UPC_prom""          = @UpcProm,
            id_product          = @ProductId,
            selling_price       = @SellingPrice,
            products_number     = @ProductsNumber,
            promotional_product = @PromotionalProduct
        WHERE ""UPC"" = @Upc
    ";

    private const string DELETE_QUERY = @"
        DELETE FROM store_product WHERE ""UPC"" = @Upc
    ";

    public async Task<IEnumerable<StoreProductEntity>> GetAll()
        => await QueryAsync<StoreProductEntity>(GET_ALL_QUERY);

    public async Task<StoreProductEntity?> GetById(string upc)
        => await QuerySingleAsync<StoreProductEntity>(GET_BY_ID_QUERY, new { Upc = upc });

    public async Task Create(StoreProductEntity input)
        => await ExecuteAsync(CREATE_QUERY, input);

    public async Task Update(StoreProductEntity input)
        => await ExecuteAsync(UPDATE_QUERY, input);

    public async Task Delete(string upc)
        => await ExecuteAsync(DELETE_QUERY, new { Upc = upc });
}
