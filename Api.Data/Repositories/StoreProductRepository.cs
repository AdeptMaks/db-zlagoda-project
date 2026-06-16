using Api.Data.Entities;
using Api.Data.Repositories.Interfaces;

namespace Api.Data.Repositories;

public class StoreProductRepository(string connectionString) : BaseRepository(connectionString), IStoreProductRepository
{
    private const string BASE_SELECT = @"
        SELECT
            ""UPC""               AS Upc,
            ""UPC_prom""          AS UpcProm,
            id_product          AS ProductId,
            selling_price       AS SellingPrice,
            products_number     AS ProductsNumber,
            promotional_product AS PromotionalProduct
        FROM store_product
    ";

    private const string DETAILED_SELECT = @"
        SELECT
            sp.""UPC""               AS Upc,
            sp.""UPC_prom""          AS UpcProm,
            sp.id_product          AS ProductId,
            p.product_name         AS ProductName,
            p.characteristics      AS Characteristics,
            p.category_number      AS CategoryNumber,
            sp.selling_price       AS SellingPrice,
            sp.products_number     AS ProductsNumber,
            sp.promotional_product AS PromotionalProduct
        FROM store_product sp
        JOIN product p ON p.id_product = sp.id_product
    ";

    private const string GET_BY_ID_QUERY = $@"{BASE_SELECT} WHERE ""UPC"" = @Upc";

    private const string GET_DETAILED_BY_UPC_QUERY = $@"{DETAILED_SELECT} WHERE sp.""UPC"" = @Upc";

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

    private static string OrderBy(string sort) => sort?.ToLowerInvariant() switch
    {
        "quantity" => "ORDER BY sp.products_number DESC",
        _ => "ORDER BY p.product_name",
    };

    public async Task<IEnumerable<StoreProductEntity>> GetAll()
        => await QueryAsync<StoreProductEntity>(BASE_SELECT);

    public async Task<IEnumerable<StoreProductDetailsEntity>> GetAllDetailed(string sort)
        => await QueryAsync<StoreProductDetailsEntity>($"{DETAILED_SELECT} {OrderBy(sort)}");

    public async Task<IEnumerable<StoreProductDetailsEntity>> GetByPromotional(bool promotional, string sort)
        => await QueryAsync<StoreProductDetailsEntity>(
            $"{DETAILED_SELECT} WHERE sp.promotional_product = @Promotional {OrderBy(sort)}",
            new { Promotional = promotional });

    public async Task<StoreProductEntity?> GetById(string upc)
        => await QuerySingleAsync<StoreProductEntity>(GET_BY_ID_QUERY, new { Upc = upc });

    public async Task<StoreProductDetailsEntity?> GetDetailedByUpc(string upc)
        => await QuerySingleAsync<StoreProductDetailsEntity>(GET_DETAILED_BY_UPC_QUERY, new { Upc = upc });

    public async Task Create(StoreProductEntity input)
        => await ExecuteAsync(CREATE_QUERY, input);

    public async Task Update(StoreProductEntity input)
        => await ExecuteAsync(UPDATE_QUERY, input);

    public async Task Delete(string upc)
        => await ExecuteAsync(DELETE_QUERY, new { Upc = upc });
}
