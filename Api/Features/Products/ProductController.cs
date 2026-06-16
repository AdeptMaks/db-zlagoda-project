using Api.Data.Entities;
using Api.Data.Repositories.Interfaces;
using Api.Features.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Products;

public record ProductRequest(int CategoryNumber, string ProductName, string Characteristics);

[ApiController]
[Route("api/products")]
[Authorize]
public class ProductController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] string? search,
        [FromQuery] int? categoryNumber,
        [FromServices] IProductRepository repository)
    {
        if (!string.IsNullOrWhiteSpace(search))
            return Ok(await repository.SearchByName(search));
        if (categoryNumber.HasValue)
            return Ok(await repository.GetByCategory(categoryNumber.Value));
        return Ok(await repository.GetAll());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, [FromServices] IProductRepository repository)
    {
        var product = await repository.GetById(id);
        return product is null ? NotFound() : Ok(product);
    }

    private static string? Validate(ProductRequest r)
    {
        if (string.IsNullOrWhiteSpace(r.ProductName) || r.ProductName.Length > 50)
            return "Назва товару обовʼязкова і має бути до 50 символів.";
        if (r.Characteristics.Length > 100)
            return "Характеристики мають бути до 100 символів.";
        return null;
    }

    [HttpPost]
    [Authorize(Roles = EmployeeRoles.Manager)]
    public async Task<IActionResult> Create(
        [FromBody] ProductRequest request, [FromServices] IProductRepository repository)
    {
        if (Validate(request) is { } error) return BadRequest(new { error });

        var id = await repository.GetNextId();
        var entity = new ProductEntity
        {
            ProductId = id,
            CategoryNumber = request.CategoryNumber,
            ProductName = request.ProductName,
            Characteristics = request.Characteristics,
        };
        await repository.Create(entity);
        return CreatedAtAction(nameof(GetById), new { id }, entity);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = EmployeeRoles.Manager)]
    public async Task<IActionResult> Update(
        int id, [FromBody] ProductRequest request, [FromServices] IProductRepository repository)
    {
        if (Validate(request) is { } error) return BadRequest(new { error });
        if (await repository.GetById(id) is null) return NotFound();

        var entity = new ProductEntity
        {
            ProductId = id,
            CategoryNumber = request.CategoryNumber,
            ProductName = request.ProductName,
            Characteristics = request.Characteristics,
        };
        await repository.Update(entity);
        return Ok(entity);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = EmployeeRoles.Manager)]
    public async Task<IActionResult> Delete(int id, [FromServices] IProductRepository repository)
    {
        if (await repository.GetById(id) is null) return NotFound();
        await repository.Delete(id);
        return NoContent();
    }
}
