namespace Api.Data.Entities;

public class StoreProductEntity
{
    public string Upc { get; set; } = "";
    public string? UpcProm { get; set; }
    public int ProductId { get; set; }
    public decimal SellingPrice { get; set; }
    public int ProductsNumber { get; set; }
    public bool PromotionalProduct { get; set; }
}
