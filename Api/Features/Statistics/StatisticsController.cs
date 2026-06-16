using Api.Data.Repositories.Interfaces;
using Api.Features.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Statistics;

[ApiController]
[Route("api/statistics")]
[Authorize(Roles = EmployeeRoles.Manager)]
public class StatisticsController : ControllerBase
{
    [HttpGet("product-revenue")]
    public async Task<IActionResult> ProductRevenue(
        [FromQuery] string? employeeId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromServices] IStatisticsRepository repository)
        => Ok(await repository.GetProductRevenue(employeeId, from, to));

    [HttpGet("category-buyers")]
    public async Task<IActionResult> CategoryBuyers(
        [FromQuery] int categoryNumber,
        [FromServices] IStatisticsRepository repository)
        => Ok(await repository.GetCategoryBuyers(categoryNumber));

    [HttpGet("category-revenue")]
    public async Task<IActionResult> CategoryRevenue(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromServices] IStatisticsRepository repository)
        => Ok(await repository.GetCategoryRevenue(from, to));

    [HttpGet("cashiers-sold-all-promos")]
    public async Task<IActionResult> CashiersSoldAllPromos(
        [FromServices] IStatisticsRepository repository)
        => Ok(await repository.GetCashiersWhoSoldAllPromos());
}
