using System.Data.Common;

namespace Api.Configuration;

public class DbExceptionMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (DbException ex) when (ex.SqlState == "23503")
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Неможливо видалити запис: на нього посилаються інші дані."
            });
        }
    }
}

public static class DbExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseDbExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<DbExceptionMiddleware>();
}
