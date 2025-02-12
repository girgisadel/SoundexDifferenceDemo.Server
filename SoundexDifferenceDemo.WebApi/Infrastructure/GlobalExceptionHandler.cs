using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SoundexDifferenceDemo.SharedKernel;
using System.Diagnostics;
using System.Text.Json;

namespace SoundexDifferenceDemo.WebApi.Infrastructure;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IWebHostEnvironment environment) : IExceptionHandler
{
    private static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        string correlationId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        logger.LogError(exception, "An Exception has occurred. CorrelationId: {CorrelationId}", correlationId);

        httpContext.Response.ContentType = "application/problem+json";

        switch (exception)
        {
            case ValidationException validationException:
                await HandleValidationExceptionAsync(httpContext, validationException, correlationId, cancellationToken);
                break;

            case BusinessException businessException:
                await HandleBusinessExceptionAsync(httpContext, businessException, correlationId, cancellationToken);
                break;

            default:
                await HandleUnknownExceptionAsync(httpContext, exception, correlationId, cancellationToken);
                break;
        }
        return true;
    }

    private async Task HandleValidationExceptionAsync(HttpContext httpContext, ValidationException exception, string correlationId, CancellationToken cancellationToken = default)
    {
        logger.LogError(exception, "Validation Exception.");

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        httpContext.Response.ContentType = "application/problem+json";

        var response = new ProblemDetails
        {
            Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/400",
            Title = "Validation Failed",
            Status = StatusCodes.Status400BadRequest,
            Detail = "One or more validation errors occurred.",
            Instance = httpContext.Request.Path,
            Extensions = { ["errors"] = exception.AsCodeDictionary(), ["correlationId"] = correlationId }
        };

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response, jsonSerializerOptions), cancellationToken);
    }

    private async Task HandleBusinessExceptionAsync(HttpContext httpContext, BusinessException exception, string correlationId, CancellationToken cancellationToken = default)
    {
        logger.LogError(exception, "Business Exception.");

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        httpContext.Response.ContentType = "application/problem+json";

        var response = new ProblemDetails
        {
            Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/400",
            Title = "Operation Failed",
            Status = StatusCodes.Status400BadRequest,
            Detail = "The requested operation could not be completed due to a failure.",
            Instance = httpContext.Request.Path,
            Extensions = { ["errors"] = exception.Errors.AsCodeDictionary(), ["correlationId"] = correlationId }
        };

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response, jsonSerializerOptions), cancellationToken);
    }

    private async Task HandleUnknownExceptionAsync(HttpContext httpContext, Exception exception, string correlationId, CancellationToken cancellationToken = default)
    {
        logger.LogError(exception, "Unknown Exception.");

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = new ProblemDetails
        {
            Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/500",
            Title = "Internal Server Error",
            Status = StatusCodes.Status500InternalServerError,
            Detail = environment.IsDevelopment() ? exception.ToString() : "An unexpected error occurred.",
            Instance = httpContext.Request.Path,
            Extensions = { ["correlationId"] = correlationId }
        };

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response, jsonSerializerOptions), cancellationToken);
    }
}
