namespace Api.Features.Employees;

public class EmployeeContactResponse
{
    public string Surname { get; set; } = "";
    public string Firstname { get; set; } = "";
    public string? Patronymic { get; set; }
    public string PhoneNumber { get; set; } = "";
    public string City { get; set; } = "";
    public string Street { get; set; } = "";
    public string ZipCode { get; set; } = "";
}
