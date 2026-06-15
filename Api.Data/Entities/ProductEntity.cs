namespace Api.Data.Entities;

public class ProductEntity
{
    public int ProductId { get; set; }
    public int CategoryNumber { get; set; }
    public string ProductName { get; set; } = "";
    public string Characteristics { get; set; } = "";
}
