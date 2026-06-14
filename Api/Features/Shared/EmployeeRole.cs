namespace Api.Features.Shared;

public static class EmployeeRoles
{
    public const string Cashier = nameof(EmpoloyeeRole.Cachier);
    public const string Manager = nameof(EmpoloyeeRole.Manager);
}

public enum EmpoloyeeRole
{
    Cachier,
    Manager
}