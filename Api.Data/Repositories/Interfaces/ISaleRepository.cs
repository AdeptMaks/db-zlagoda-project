using Api.Data.Entities;

namespace Api.Data.Repositories.Interfaces;

public interface ISaleRepository
{
    Task<IEnumerable<SaleEntity>> GetByCheckNumber(string checkNumber);
    Task<IEnumerable<SaleDetailsEntity>> GetDetailedByCheck(string checkNumber);
    Task Create(SaleEntity input);
    Task Delete(string upc, string checkNumber);
}
