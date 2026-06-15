using Api.Data.Entities;
using Api.Features.Employees;

namespace Api.Features.Auth;

public static class EmployeeExtensions
{
    public static EmployeeEntity ToEntity(this CreateEmployeeRequest request, string id, string hashedPassword) =>
        new()
        {
            EmployeeId = id,
            EmployeeRole = request.EmployeeRole.ToString(),
            Surname = request.Surname,
            Firstname = request.Firstname,
            Username = request.Username,
            Password = hashedPassword,
            Patronymic = request.Patronymic,
            Salary = request.Salary,
            StartDate = request.StartDate,
            BirthDate = request.BirthDate,
            PhoneNumber = request.PhoneNumber,
            City = request.AddressInfo?.City ?? "",
            Street = request.AddressInfo?.Street ?? "",
            ZipCode = request.AddressInfo?.ZipCode ?? "",
        };

    public static EmployeeResponse ToResponse(this EmployeeEntity entity) => new()
    {
        EmployeeId = entity.EmployeeId,
        EmployeeRole = entity.EmployeeRole,
        Surname = entity.Surname,
        Firstname = entity.Firstname,
        Patronymic = entity.Patronymic,
        Salary = entity.Salary,
        StartDate = entity.StartDate,
        BirthDate = entity.BirthDate,
        PhoneNumber = entity.PhoneNumber,
        City = entity.City,
        Street = entity.Street,
        ZipCode = entity.ZipCode,
    };

    public static EmployeeContactResponse ToContactResponse(this EmployeeEntity entity) => new()
    {
        Surname = entity.Surname,
        Firstname = entity.Firstname,
        Patronymic = entity.Patronymic,
        PhoneNumber = entity.PhoneNumber,
        City = entity.City,
        Street = entity.Street,
        ZipCode = entity.ZipCode,
    };

    public static EmployeeEntity ToEntity(this UpdateEmployeeRequest request, string id) => new()
    {
        EmployeeId = id,
        EmployeeRole = request.EmployeeRole.ToString(),
        Surname = request.Surname,
        Firstname = request.Firstname,
        Patronymic = request.Patronymic,
        Salary = request.Salary,
        StartDate = request.StartDate,
        BirthDate = request.BirthDate,
        PhoneNumber = request.PhoneNumber,
        City = request.AddressInfo?.City ?? "",
        Street = request.AddressInfo?.Street ?? "",
        ZipCode = request.AddressInfo?.ZipCode ?? "",
    };
}
