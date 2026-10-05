using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TodoApp.BLL.Exceptions;

namespace TodoApp.API.Infrastructure.ExceptionHandling;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetailsService)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            ConflictException conflictException =>
                (
                    StatusCodes.Status409Conflict,
                    "Conflict",
                    conflictException.Message
                ),

            ValidationException validationException =>
                (
                    StatusCodes.Status400BadRequest,
                    "Validation failed",
                    validationException.Message
                ),
            
            EntityNotFoundException entityNotFoundException =>
                (
                    StatusCodes.Status404NotFound,
                    "Not found",
                    entityNotFoundException.Message
                ),

            UnauthorizedAccessException =>
                (
                    StatusCodes.Status401Unauthorized,
                    "Unauthorized",
                    "Authentication is required to access this resource."
                ),

            _ =>
                (
                    StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred.",
                    "An unexpected error occurred while processing the request."
                )
        };

        logger.LogError(
            exception,
            "An exception occurred while processing {Method} {Path}",
            httpContext.Request.Method,
            httpContext.Request.Path);

        httpContext.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = title,
                    Detail = detail
                }
            });
    }
}