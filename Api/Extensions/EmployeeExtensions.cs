using Api.Data.Entities;

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
            AddressInfo = request.AddressInfo is null ? null : new AddressEntity
            {
                City = request.AddressInfo.City,
                Street = request.AddressInfo.Street,
                ZipCode = request.AddressInfo.ZipCode,
            },
        };
}
