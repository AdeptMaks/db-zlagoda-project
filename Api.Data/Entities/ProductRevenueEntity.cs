namespace Api.Data.Entities;

public class ProductRevenueEntity
{
    public string ProductName { get; set; } = "";
    public long TotalQuantity { get; set; }
    public decimal TotalRevenue { get; set; }
}
