using Api.Data.Entities;

namespace Api.Data.Repositories.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<ProductEntity>> GetAll();
    Task<IEnumerable<ProductEntity>> GetByCategory(int categoryNumber);
    Task<IEnumerable<ProductEntity>> SearchByName(string name);
    Task<ProductEntity?> GetById(int id);
    Task<int> GetNextId();
    Task Create(ProductEntity input);
    Task Update(ProductEntity input);
    Task Delete(int id);
}
