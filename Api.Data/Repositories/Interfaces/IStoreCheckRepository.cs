using Api.Data.Entities;

namespace Api.Data.Repositories.Interfaces;

public interface IStoreCheckRepository
{
    Task<IEnumerable<StoreCheckEntity>> GetAll();
    Task<StoreCheckEntity?> GetById(string checkNumber);
    Task Create(StoreCheckEntity input);
    Task Delete(string checkNumber);
}
