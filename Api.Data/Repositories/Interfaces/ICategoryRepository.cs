using Api.Data.Entities;

namespace Api.Data.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<CategoryEntity>> GetAll();
    Task<CategoryEntity?> GetById(int id);
    Task Create(CategoryEntity input);
    Task Update(CategoryEntity input);
    Task Delete(int id);
}
