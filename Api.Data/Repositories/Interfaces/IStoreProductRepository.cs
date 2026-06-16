using Api.Data.Entities;

namespace Api.Data.Repositories.Interfaces;

public interface IStoreProductRepository
{
    Task<IEnumerable<StoreProductEntity>> GetAll();
    Task<IEnumerable<StoreProductDetailsEntity>> GetAllDetailed(string sort);
    Task<IEnumerable<StoreProductDetailsEntity>> GetByPromotional(bool promotional, string sort);
    Task<StoreProductEntity?> GetById(string upc);
    Task<StoreProductDetailsEntity?> GetDetailedByUpc(string upc);
    Task Create(StoreProductEntity input);
    Task Update(StoreProductEntity input);
    Task Delete(string upc);
}
