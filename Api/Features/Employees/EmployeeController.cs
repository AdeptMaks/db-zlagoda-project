using Api.Data.Repositories.Interfaces;
using Api.Features.Auth;
using Api.Features.Shared;
using Api.Interfaces.Utils;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Employees;

[ApiController]
[Route("api/employees")]
[Authorize]
public class EmployeeController : ControllerBase
{
    // GET /api/employees
    // Manager: усі працівники, відсортовані за прізвищем (вимога #5)
    [HttpGet]
    [Authorize(Roles = EmployeeRoles.Manager)]
    public async Task<IActionResult> GetAll([FromServices] IEmployeeRepository repository)
    {
        var employees = await repository.GetAll();
        return Ok(employees.Select(e => e.ToResponse()));
    }

    [HttpGet("cashiers")]
    [Authorize(Roles = EmployeeRoles.Manager)]
    public async Task<IActionResult> GetAllCashiers([FromServices] IEmployeeRepository repository)
    {
        var cashiers = await repository.GetAllCashiers();
        return Ok(cashiers.Select(e => e.ToResponse()));
    }


    [HttpGet("search")]
    [Authorize(Roles = EmployeeRoles.Manager)]
    public async Task<IActionResult> SearchBySurname(
        [FromQuery] string surname,
        [FromServices] IEmployeeRepository repository)
    {
        var employees = await repository.SearchBySurname(surname);
        return Ok(employees.Select(e => e.ToContactResponse()));
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe([FromServices] IEmployeeRepository repository)
    {
        var username = User.Identity?.Name;
        if (username is null)
            return Unauthorized();

        var employee = await repository.GetByUsername(username);
        if (employee is null)
            return NotFound();

        return Ok(employee.ToResponse());
    }

    [HttpGet("{id}")]
    [Authorize(Roles = EmployeeRoles.Manager)]
    public async Task<IActionResult> GetById(string id, [FromServices] IEmployeeRepository repository)
    {
        var employee = await repository.GetById(id);
        if (employee is null)
            return NotFound();

        return Ok(employee.ToResponse());
    }

    [HttpPost]
    [Authorize(Roles = EmployeeRoles.Manager)]
    public async Task<IActionResult> Create(
        [FromBody] CreateEmployeeRequest request,
        [FromServices] IValidator<CreateEmployeeRequest> validator,
        [FromServices] IEmployeeRepository repository,
        [FromServices] IPasswordHasher passwordHasher)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var isUnique = await repository.CheckIfUnique(request.Username);

        if (!isUnique)
            return Conflict(new { Error = "Username is already in use" });

        var hashedPassword = passwordHasher.HashPassword(request.Password);
        var id = Guid.NewGuid().ToString()[..9];
        var entity = request.ToEntity(id, hashedPassword);

        await repository.Create(entity);

        return CreatedAtAction(nameof(GetById), new { id }, entity.ToResponse());
    }


    [HttpPut("{id}")]
    [Authorize(Roles = EmployeeRoles.Manager)]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdateEmployeeRequest request,
        [FromServices] IValidator<UpdateEmployeeRequest> validator,
        [FromServices] IEmployeeRepository repository)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var existing = await repository.GetById(id);
        if (existing is null)
            return NotFound();

        var entity = request.ToEntity(id);
        await repository.Update(entity);

        return Ok(entity.ToResponse());
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = EmployeeRoles.Manager)]
    public async Task<IActionResult> Delete(string id, [FromServices] IEmployeeRepository repository)
    {
        var existing = await repository.GetById(id);
        if (existing is null)
            return NotFound();

        await repository.Delete(id);

        return NoContent();
    }
}
