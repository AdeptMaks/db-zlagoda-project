namespace Api.Features.Auth;

public class CreateEmployeeRequest
{
    public EmpoloyeeRole EmployeeRole { get; set; }
    public string Surname { get; set; } = "";
    public string Firstname { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string? Patronymic { get; set; }
    public decimal Salary { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime BirthDate { get; set; }
    public string PhoneNumber { get; set; } = "";
    public Address? AddressInfo { get; set; }
}

public enum EmpoloyeeRole
{
    Cachier,
    Manager
}

public class Address
{
    public string City { get; set; } = "";
    public string Street { get; set; } = "";
    public string ZipCode { get; set; } = "";
}