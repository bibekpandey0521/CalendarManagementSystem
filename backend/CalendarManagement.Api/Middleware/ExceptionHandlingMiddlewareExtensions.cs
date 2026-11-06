using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics;

namespace CalendarManagement.Api.Middleware;

/// <summary>
/// Represents the next delegate in the request pipeline.
/// </summary>
public delegate Task NextDelegate(HttpContext context);

/// <summary>
/// Extension methods used to register the exception handling middleware.
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
