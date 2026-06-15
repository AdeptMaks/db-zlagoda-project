using Api.Data.Entities;

namespace Api.Data.Repositories.Interfaces;

public interface IStoreProductRepository
{
    Task<IEnumerable<StoreProductEntity>> GetAll();
    Task<StoreProductEntity?> GetById(string upc);
    Task Create(StoreProductEntity input);
    Task Update(StoreProductEntity input);
    Task Delete(string upc);
}
