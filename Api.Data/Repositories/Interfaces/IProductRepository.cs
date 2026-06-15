using Api.Data.Entities;

namespace Api.Data.Repositories.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<ProductEntity>> GetAll();
    Task<ProductEntity?> GetById(int id);
    Task Create(ProductEntity input);
    Task Update(ProductEntity input);
    Task Delete(int id);
}
