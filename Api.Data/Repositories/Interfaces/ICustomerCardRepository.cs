using Api.Data.Entities;

namespace Api.Data.Repositories.Interfaces;

public interface ICustomerCardRepository
{
    Task<IEnumerable<CustomerCardEntity>> GetAll();
    Task<IEnumerable<CustomerCardEntity>> SearchBySurname(string surname);
    Task<IEnumerable<CustomerCardEntity>> GetByPercent(int percent);
    Task<CustomerCardEntity?> GetById(string id);
    Task<string> GetNextCardNumber();
    Task Create(CustomerCardEntity input);
    Task Update(CustomerCardEntity input);
    Task Delete(string id);
}
