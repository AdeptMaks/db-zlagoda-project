using Api.Data.Entities;
using Api.Data.Repositories.Interfaces;
using Api.Features.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.CustomerCards;

public record CustomerCardRequest(
    string Surname,
    string Firstname,
    string? Patronymic,
    string PhoneNumber,
    string? City,
    string? Street,
    string? ZipCode,
    int Percent);

[ApiController]
[Route("api/customer-cards")]
[Authorize]
public class CustomerCardController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] string? search,
        [FromQuery] int? percent,
        [FromServices] ICustomerCardRepository repository)
    {
        if (!string.IsNullOrWhiteSpace(search))
            return Ok(await repository.SearchBySurname(search));
        if (percent.HasValue)
            return Ok(await repository.GetByPercent(percent.Value));
        return Ok(await repository.GetAll());
    }

    [HttpGet("{cardNumber}")]
    public async Task<IActionResult> GetById(string cardNumber, [FromServices] ICustomerCardRepository repository)
    {
        var card = await repository.GetById(cardNumber);
        return card is null ? NotFound() : Ok(card);
    }

    private static string? Validate(CustomerCardRequest r)
    {
        if (string.IsNullOrWhiteSpace(r.Surname) || r.Surname.Length > 50)
            return "Прізвище обовʼязкове і має бути до 50 символів.";
        if (string.IsNullOrWhiteSpace(r.Firstname) || r.Firstname.Length > 50)
            return "Імʼя обовʼязкове і має бути до 50 символів.";
        if (string.IsNullOrWhiteSpace(r.PhoneNumber) || r.PhoneNumber.Length > 13)
            return "Телефон обовʼязковий і має бути до 13 символів.";
        if (r.Percent < 0 || r.Percent > 100)
            return "Відсоток має бути в межах 0–100.";
        if (!string.IsNullOrEmpty(r.ZipCode) && r.ZipCode.Length > 9)
            return "Індекс має бути до 9 символів.";
        return null;
    }

    private static CustomerCardEntity ToEntity(string cardNumber, CustomerCardRequest r) => new()
    {
        CardNumber = cardNumber,
        Surname = r.Surname,
        Firstname = r.Firstname,
        Patronymic = r.Patronymic,
        PhoneNumber = r.PhoneNumber,
        City = r.City,
        Street = r.Street,
        ZipCode = r.ZipCode,
        Percent = r.Percent,
    };

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CustomerCardRequest request, [FromServices] ICustomerCardRepository repository)
    {
        if (Validate(request) is { } error) return BadRequest(new { error });

        var cardNumber = await repository.GetNextCardNumber();
        var entity = ToEntity(cardNumber, request);
        await repository.Create(entity);
        return CreatedAtAction(nameof(GetById), new { cardNumber }, entity);
    }

    [HttpPut("{cardNumber}")]
    public async Task<IActionResult> Update(
        string cardNumber, [FromBody] CustomerCardRequest request, [FromServices] ICustomerCardRepository repository)
    {
        if (Validate(request) is { } error) return BadRequest(new { error });
        if (await repository.GetById(cardNumber) is null) return NotFound();

        var entity = ToEntity(cardNumber, request);
        await repository.Update(entity);
        return Ok(entity);
    }

    [HttpDelete("{cardNumber}")]
    [Authorize(Roles = EmployeeRoles.Manager)]
    public async Task<IActionResult> Delete(string cardNumber, [FromServices] ICustomerCardRepository repository)
    {
        if (await repository.GetById(cardNumber) is null) return NotFound();
        await repository.Delete(cardNumber);
        return NoContent();
    }
}
