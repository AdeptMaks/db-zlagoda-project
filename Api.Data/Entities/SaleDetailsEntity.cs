namespace Api.Data.Entities;

public class SaleDetailsEntity
{
    public string Upc { get; set; } = "";
    public string ProductName { get; set; } = "";
    public int ProductNumber { get; set; }
    public decimal SellingPrice { get; set; }
}
