using Api.Data.Entities;
using Api.Data.Repositories.Interfaces;

namespace Api.Data.Repositories;

public class CategoryRepository(string connectionString) : BaseRepository(connectionString), ICategoryRepository
{
    private const string GET_ALL_QUERY = @"
        SELECT
            category_number AS CategoryNumber,
            category_name   AS CategoryName
        FROM category
    ";

    private const string GET_BY_ID_QUERY = @"
        SELECT
            category_number AS CategoryNumber,
            category_name   AS CategoryName
        FROM category
        WHERE category_number = @CategoryNumber
    ";

    private const string CREATE_QUERY = @"
        INSERT INTO category (category_number, category_name)
        VALUES (@CategoryNumber, @CategoryName)
    ";

    private const string UPDATE_QUERY = @"
        UPDATE category SET
            category_name = @CategoryName
        WHERE category_number = @CategoryNumber
    ";

    private const string DELETE_QUERY = @"
        DELETE FROM category WHERE category_number = @CategoryNumber
    ";

    public async Task<IEnumerable<CategoryEntity>> GetAll()
        => await QueryAsync<CategoryEntity>(GET_ALL_QUERY);

    public async Task<CategoryEntity?> GetById(int id)
        => await QuerySingleAsync<CategoryEntity>(GET_BY_ID_QUERY, new { CategoryNumber = id });

    public async Task Create(CategoryEntity input)
        => await ExecuteAsync(CREATE_QUERY, input);

    public async Task Update(CategoryEntity input)
        => await ExecuteAsync(UPDATE_QUERY, input);

    public async Task Delete(int id)
        => await ExecuteAsync(DELETE_QUERY, new { CategoryNumber = id });
}
