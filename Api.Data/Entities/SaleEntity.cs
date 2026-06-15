namespace Api.Data.Entities;

public class SaleEntity
{
    public string Upc { get; set; } = "";
    public string CheckNumber { get; set; } = "";
    public int ProductNumber { get; set; }
    public decimal SellingPrice { get; set; }
}
