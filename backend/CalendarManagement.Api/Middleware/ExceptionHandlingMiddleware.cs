using System.Text.Json;
using System.Text.Json.Serialization;
using CalendarManagement.Api.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics;

namespace CalendarManagement.Api.Middleware;

/// <summary>
/// Converts unhandled exceptions into RFC 7807 shaped problem details responses.
/// </summary>
public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (DomainValidationException exception)
        {
            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status400BadRequest,
                "Validation failed.",
                exception.Message,
                exception.Errors);
        }
        catch (NotFoundException exception)
        {
            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status404NotFound,
                "Resource not found.",
                exception.Message,
                null);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception while processing {Method} {Path}", context.Request.Method, context.Request.Path);
            var statusCode = StatusCodes.Status500InternalServerError;
            var detail = _environment.IsDevelopment()
                ? $"An unexpected error occurred: {exception.Message}"
                : "An unexpected error occurred while processing the request.";

            await WriteProblemDetailsAsync(
                context,
                statusCode,
                "Request failed.",
                detail,
                null);
        }
    }

    private static Task WriteProblemDetailsAsync(
        HttpContext context,
        int statusCode,
        string title,
        string? detail,
        IReadOnlyDictionary<string, string[]>? errors)
    {
        var problemDetails = new ProblemDetailsResponse
        {
            Type = "https://tools.ietf.org/spec/rfc7807",
            Title = title,
            Status = statusCode,
            Detail = detail,
            Instance = context.Request.Path.Value,
            TraceId = context.TraceIdentifier,
            Errors = errors
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        return context.Response.WriteAsJsonAsync(problemDetails, options);
    }

    private sealed class ProblemDetailsResponse
    {
        public string Type { get; init; } = "https://tools.ietf.org/spec/rfc7807";

        public string Title { get; init; } = string.Empty;

        public int Status { get; init; }

        public string? Detail { get; init; }

        public string? Instance { get; init; }

        public string TraceId { get; init; } = string.Empty;

        public IReadOnlyDictionary<string, string[]>? Errors { get; init; }
    }
}
