using Api.Data.Entities;
using Api.Data.Repositories.Interfaces;
using Api.Features.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.StoreProducts;

public record StoreProductRequest(
    string Upc,
    string? UpcProm,
    int ProductId,
    decimal SellingPrice,
    int ProductsNumber,
    bool PromotionalProduct);

[ApiController]
[Route("api/store-products")]
[Authorize]
public class StoreProductController : ControllerBase
{
    private const decimal PromoMultiplier = 0.8m;

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] string? sort,
        [FromQuery] bool? promotional,
        [FromServices] IStoreProductRepository repository)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            sort = "name";
        }

        if (promotional.HasValue)
            return Ok(await repository.GetByPromotional(promotional.Value, sort));
        return Ok(await repository.GetAllDetailed(sort));
    }

    [HttpGet("{upc}")]
    public async Task<IActionResult> GetByUpc(string upc, [FromServices] IStoreProductRepository repository)
    {
        var product = await repository.GetDetailedByUpc(upc);
        return product is null ? NotFound() : Ok(product);
    }

    private static string? Validate(StoreProductRequest r)
    {
        if (string.IsNullOrWhiteSpace(r.Upc) || r.Upc.Length > 12)
            return "UPC обовʼязковий і має бути до 12 символів.";
        if (r.SellingPrice < 0) return "Ціна не може бути відʼємною.";
        if (r.ProductsNumber < 0) return "Кількість не може бути відʼємною.";
        return null;
    }

    private static async Task<decimal> ResolvePrice(
        StoreProductRequest r, IStoreProductRepository repository)
    {
        if (r.PromotionalProduct && !string.IsNullOrEmpty(r.UpcProm))
        {
            var basis = await repository.GetById(r.UpcProm);
            if (basis is not null)
                return Math.Round(basis.SellingPrice * PromoMultiplier, 4);
        }
        return r.SellingPrice;
    }

    [HttpPost]
    [Authorize(Roles = EmployeeRoles.Manager)]
    public async Task<IActionResult> Create(
        [FromBody] StoreProductRequest request, [FromServices] IStoreProductRepository repository)
    {
        if (Validate(request) is { } error) return BadRequest(new { error });
        if (await repository.GetById(request.Upc) is not null)
            return Conflict(new { error = "Товар з таким UPC вже існує." });

        var entity = new StoreProductEntity
        {
            Upc = request.Upc,
            UpcProm = request.UpcProm,
            ProductId = request.ProductId,
            SellingPrice = await ResolvePrice(request, repository),
            ProductsNumber = request.ProductsNumber,
            PromotionalProduct = request.PromotionalProduct,
        };
        await repository.Create(entity);
        return CreatedAtAction(nameof(GetByUpc), new { upc = entity.Upc }, entity);
    }

    [HttpPut("{upc}")]
    [Authorize(Roles = EmployeeRoles.Manager)]
    public async Task<IActionResult> Update(
        string upc, [FromBody] StoreProductRequest request, [FromServices] IStoreProductRepository repository)
    {
        if (Validate(request) is { } error) return BadRequest(new { error });
        if (await repository.GetById(upc) is null) return NotFound();

        var entity = new StoreProductEntity
        {
            Upc = upc,
            UpcProm = request.UpcProm,
            ProductId = request.ProductId,
            SellingPrice = await ResolvePrice(request, repository),
            ProductsNumber = request.ProductsNumber,
            PromotionalProduct = request.PromotionalProduct,
        };
        await repository.Update(entity);
        return Ok(entity);
    }

    [HttpDelete("{upc}")]
    [Authorize(Roles = EmployeeRoles.Manager)]
    public async Task<IActionResult> Delete(string upc, [FromServices] IStoreProductRepository repository)
    {
        if (await repository.GetById(upc) is null) return NotFound();
        await repository.Delete(upc);
        return NoContent();
    }
}
