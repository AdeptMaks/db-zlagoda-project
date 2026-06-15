using Api.Features.Shared;

namespace Api.Features.Employees;

public class UpdateEmployeeRequest
{
    public EmpoloyeeRole EmployeeRole { get; set; }
    public string Surname { get; set; } = "";
    public string Firstname { get; set; } = "";
    public string? Patronymic { get; set; }
    public decimal Salary { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime BirthDate { get; set; }
    public string PhoneNumber { get; set; } = "";
    public Address? AddressInfo { get; set; }
}
