using Api.Data.Entities;

namespace Api.Data.Repositories.Interfaces;

public interface ICustomerCardRepository
{
    Task<IEnumerable<CustomerCardEntity>> GetAll();
    Task<CustomerCardEntity?> GetById(string id);
    Task Create(CustomerCardEntity input);
    Task Update(CustomerCardEntity input);
    Task Delete(string id);
}
