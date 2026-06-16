using Api.Data.Entities;

namespace Api.Data.Repositories.Interfaces;

public interface IStoreCheckRepository
{
    Task<IEnumerable<StoreCheckEntity>> GetFiltered(string? employeeId, DateTime? from, DateTime? to);
    Task<StoreCheckEntity?> GetById(string checkNumber);
    Task<string> GetNextCheckNumber();
    Task CreateWithSales(StoreCheckEntity check, IEnumerable<SaleEntity> sales);
    Task Delete(string checkNumber);

    Task<decimal> GetTotalSum(string? employeeId, DateTime? from, DateTime? to);
    Task<int> GetTotalProductQuantity(int productId, DateTime? from, DateTime? to);
}
