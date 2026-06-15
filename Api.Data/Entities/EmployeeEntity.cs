namespace Api.Data.Entities;

public class EmployeeEntity
{
    public string EmployeeId { get; set; } = "";
    public string EmployeeRole { get; set; } = "";
    public string Surname { get; set; } = "";
    public string Firstname { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string? Patronymic { get; set; }
    public decimal Salary { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime BirthDate { get; set; }
    public string PhoneNumber { get; set; } = "";
    public AddressEntity? AddressInfo { get; set; }
}

public class AddressEntity
{
    public string City { get; set; } = "";
    public string Street { get; set; } = "";
    public string ZipCode { get; set; } = "";
}

