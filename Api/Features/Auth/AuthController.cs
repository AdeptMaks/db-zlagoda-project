using System.Runtime.CompilerServices;
using Api.Interfaces.Auth;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    [HttpPost]
    [Route("employee/register")]
    public async Task<IActionResult> Register(
        [FromBody] CreateEmployeeRequest request, [FromServices] IValidator<CreateEmployeeRequest> validator, [FromServices] IAuthService authService)
    {
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var response = await authService.CreateEmployee(request);

        if (response.IsFailure)
            return BadRequest(response.Errors);

        return Ok(response.Data);
    }

    [HttpPost]
    [Route("employee/login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request, [FromServices] IValidator<LoginRequest> validator)
    {
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        // TODO: should be jwt here
        return Ok("");
    }
}