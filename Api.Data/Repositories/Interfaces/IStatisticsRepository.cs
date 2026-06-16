using Api.Data.Entities;

namespace Api.Data.Repositories.Interfaces;

public interface IStatisticsRepository
{
    Task<IEnumerable<ProductRevenueEntity>> GetProductRevenue(string? employeeId, DateTime? from, DateTime? to);
    Task<IEnumerable<CategoryBuyerEntity>> GetCategoryBuyers(int categoryNumber);
}
