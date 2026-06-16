using Api.Data.Entities;
using Api.Data.Repositories.Interfaces;
using Api.Features.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Checks;

public record CheckItemRequest(string Upc, int Quantity);
public record CreateCheckRequest(string? CardNumber, List<CheckItemRequest> Items);

public record CheckDetailsResponse(
    string CheckNumber,
    string EmployeeId,
    string? CardNumber,
    DateTime PrintDate,
    decimal SumTotal,
    decimal Vat,
    IEnumerable<SaleDetailsEntity> Items);

[ApiController]
[Route("api/checks")]
[Authorize]
public class ChecksController : ControllerBase
{
    private const decimal VatRate = 0.2m;

    private async Task<string?> CurrentEmployeeId(IEmployeeRepository employees)
    {
        var username = User.Identity?.Name;
        if (username is null) return null;
        var employee = await employees.GetByUsername(username);
        return employee?.EmployeeId;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] string? employeeId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromServices] IStoreCheckRepository checks,
        [FromServices] IEmployeeRepository employees)
    {
        if (!User.IsInRole(EmployeeRoles.Manager))
            employeeId = await CurrentEmployeeId(employees);

        return Ok(await checks.GetFiltered(employeeId, from, to));
    }

    [HttpGet("{checkNumber}")]
    public async Task<IActionResult> GetById(
        string checkNumber,
        [FromServices] IStoreCheckRepository checks,
        [FromServices] ISaleRepository sales)
    {
        var check = await checks.GetById(checkNumber);
        if (check is null) return NotFound();

        var items = await sales.GetDetailedByCheck(checkNumber);
        return Ok(new CheckDetailsResponse(
            check.CheckNumber, check.EmployeeId, check.CardNumber,
            check.PrintDate, check.SumTotal, check.Vat, items));
    }

    [HttpPost]
    [Authorize(Roles = EmployeeRoles.Cashier)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCheckRequest request,
        [FromServices] IStoreCheckRepository checks,
        [FromServices] ISaleRepository sales,
        [FromServices] IStoreProductRepository storeProducts,
        [FromServices] ICustomerCardRepository cards,
        [FromServices] IEmployeeRepository employees)
    {
        if (request.Items is null || request.Items.Count == 0)
            return BadRequest(new { error = "Чек має містити щонайменше один товар." });

        var employeeId = await CurrentEmployeeId(employees);
        if (employeeId is null) return Unauthorized();

        decimal discountPercent = 0;
        if (!string.IsNullOrEmpty(request.CardNumber))
        {
            var card = await cards.GetById(request.CardNumber);
            if (card is null) return BadRequest(new { error = "Картку клієнта не знайдено." });
            discountPercent = card.Percent;
        }

        var checkNumber = await checks.GetNextCheckNumber();
        decimal rawTotal = 0;
        var saleRows = new List<SaleEntity>();

        foreach (var item in request.Items)
        {
            if (item.Quantity <= 0)
                return BadRequest(new { error = "Кількість товару має бути додатною." });

            var sp = await storeProducts.GetById(item.Upc);
            if (sp is null)
                return BadRequest(new { error = $"Товар з UPC {item.Upc} не знайдено." });
            if (sp.ProductsNumber < item.Quantity)
                return BadRequest(new { error = $"Недостатньо одиниць товару {item.Upc} на складі." });

            rawTotal += sp.SellingPrice * item.Quantity;
            saleRows.Add(new SaleEntity
            {
                Upc = item.Upc,
                CheckNumber = checkNumber,
                ProductNumber = item.Quantity,
                SellingPrice = sp.SellingPrice,
            });
        }

        var sumTotal = Math.Round(rawTotal * (1 - discountPercent / 100m), 4);
        var vat = Math.Round(sumTotal * VatRate, 4);

        var check = new StoreCheckEntity
        {
            CheckNumber = checkNumber,
            EmployeeId = employeeId,
            CardNumber = string.IsNullOrEmpty(request.CardNumber) ? null : request.CardNumber,
            PrintDate = DateTime.Now,
            SumTotal = sumTotal,
            Vat = vat,
        };

        try
        {
            await checks.CreateWithSales(check, saleRows);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        var items = await sales.GetDetailedByCheck(checkNumber);
        return CreatedAtAction(nameof(GetById), new { checkNumber }, new CheckDetailsResponse(
            check.CheckNumber, check.EmployeeId, check.CardNumber,
            check.PrintDate, check.SumTotal, check.Vat, items));
    }

    [HttpDelete("{checkNumber}")]
    [Authorize(Roles = EmployeeRoles.Manager)]
    public async Task<IActionResult> Delete(string checkNumber, [FromServices] IStoreCheckRepository checks)
    {
        if (await checks.GetById(checkNumber) is null) return NotFound();
        await checks.Delete(checkNumber);
        return NoContent();
    }

    [HttpGet("stats/sum")]
    public async Task<IActionResult> TotalSum(
        [FromQuery] string? employeeId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromServices] IStoreCheckRepository checks,
        [FromServices] IEmployeeRepository employees)
    {
        if (!User.IsInRole(EmployeeRoles.Manager))
            employeeId = await CurrentEmployeeId(employees);

        return Ok(new { total = await checks.GetTotalSum(employeeId, from, to) });
    }

    [HttpGet("stats/product-quantity")]
    [Authorize(Roles = EmployeeRoles.Manager)]
    public async Task<IActionResult> ProductQuantity(
        [FromQuery] int productId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromServices] IStoreCheckRepository checks)
        => Ok(new { quantity = await checks.GetTotalProductQuantity(productId, from, to) });
}
