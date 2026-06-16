using Api.Data.Entities;
using Api.Data.Repositories.Interfaces;
using Api.Features.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Categories;

public record CategoryRequest(string CategoryName);

[ApiController]
[Route("api/categories")]
[Authorize]
public class CategoryController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromServices] ICategoryRepository repository)
        => Ok(await repository.GetAll());

    [HttpGet("{number:int}")]
    public async Task<IActionResult> GetById(int number, [FromServices] ICategoryRepository repository)
    {
        var category = await repository.GetById(number);
        return category is null ? NotFound() : Ok(category);
    }

    [HttpPost]
    [Authorize(Roles = EmployeeRoles.Manager)]
    public async Task<IActionResult> Create(
        [FromBody] CategoryRequest request, [FromServices] ICategoryRepository repository)
    {
        if (string.IsNullOrWhiteSpace(request.CategoryName) || request.CategoryName.Length > 50)
            return BadRequest(new { error = "Назва категорії обовʼязкова і має бути до 50 символів." });

        var number = await repository.GetNextNumber();
        var entity = new CategoryEntity { CategoryNumber = number, CategoryName = request.CategoryName };
        await repository.Create(entity);

        return CreatedAtAction(nameof(GetById), new { number }, entity);
    }

    [HttpPut("{number:int}")]
    [Authorize(Roles = EmployeeRoles.Manager)]
    public async Task<IActionResult> Update(
        int number, [FromBody] CategoryRequest request, [FromServices] ICategoryRepository repository)
    {
        if (string.IsNullOrWhiteSpace(request.CategoryName) || request.CategoryName.Length > 50)
            return BadRequest(new { error = "Назва категорії обовʼязкова і має бути до 50 символів." });

        if (await repository.GetById(number) is null)
            return NotFound();

        var entity = new CategoryEntity { CategoryNumber = number, CategoryName = request.CategoryName };
        await repository.Update(entity);
        return Ok(entity);
    }

    [HttpDelete("{number:int}")]
    [Authorize(Roles = EmployeeRoles.Manager)]
    public async Task<IActionResult> Delete(int number, [FromServices] ICategoryRepository repository)
    {
        if (await repository.GetById(number) is null)
            return NotFound();

        await repository.Delete(number);
        return NoContent();
    }
}
