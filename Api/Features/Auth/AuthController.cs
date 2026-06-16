using Api.Interfaces.Auth;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private const string AuthCookieName = "AuthToken";

    private void SetAuthCookie(string token)
    {
        Response.Cookies.Append(AuthCookieName, token, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Secure = Request.IsHttps,
            Expires = DateTimeOffset.UtcNow.AddHours(1),
        });
    }

    [HttpPost]
    [Route("employee/register")]
    public async Task<IActionResult> Register(
        [FromBody] CreateEmployeeRequest request, [FromServices] IValidator<CreateEmployeeRequest> validator, [FromServices] IAuthService authService)
    {
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var response = await authService.RegisterEmployee(request);

        if (response.IsFailure)
            return BadRequest(response.Errors);

        SetAuthCookie(response.Data!);
        return Ok(new { token = response.Data });
    }

    [HttpPost]
    [Route("employee/login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        [FromServices] IValidator<LoginRequest> validator,
        [FromServices] IAuthService authService)
    {
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var response = await authService.AuthorizeEmployee(request);

        if (response.IsFailure)
            return Unauthorized(response.Errors);

        SetAuthCookie(response.Data!);
        return Ok(new { token = response.Data });
    }

    [HttpPost]
    [Route("employee/logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(AuthCookieName);
        return NoContent();
    }

    [HttpGet]
    [Route("employee/test")]
    [Authorize]
    public IActionResult Test()
    {
        return Ok("Success");
    }
}
