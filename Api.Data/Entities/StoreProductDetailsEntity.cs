namespace Api.Data.Entities;

public class StoreProductDetailsEntity
{
    public string Upc { get; set; } = "";
    public string? UpcProm { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public string Characteristics { get; set; } = "";
    public int CategoryNumber { get; set; }
    public decimal SellingPrice { get; set; }
    public int ProductsNumber { get; set; }
    public bool PromotionalProduct { get; set; }
}
