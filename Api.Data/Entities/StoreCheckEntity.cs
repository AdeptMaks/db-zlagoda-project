namespace Api.Data.Entities;

public class StoreCheckEntity
{
    public string CheckNumber { get; set; } = "";
    public string EmployeeId { get; set; } = "";
    public string? CardNumber { get; set; }
    public DateTime PrintDate { get; set; }
    public decimal SumTotal { get; set; }
    public decimal Vat { get; set; }
}
