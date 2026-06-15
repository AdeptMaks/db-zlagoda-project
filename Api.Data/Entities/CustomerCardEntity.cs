namespace Api.Data.Entities;

public class CustomerCardEntity
{
    public string CardNumber { get; set; } = "";
    public string Surname { get; set; } = "";
    public string Firstname { get; set; } = "";
    public string? Patronymic { get; set; }
    public string PhoneNumber { get; set; } = "";
    public string? City { get; set; }
    public string? Street { get; set; }
    public string? ZipCode { get; set; }
    public int Percent { get; set; }
}
