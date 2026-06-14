using EntityFramework.Exceptions.Common;
using FluentValidation;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace ICMS.API.Handlers
{
    /// <summary>
    /// Singleton exception handler that resolves the scoped ILocalizer
    /// per-request via IServiceScopeFactory to avoid captive dependency issues.
    /// </summary>
    public class GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IServiceScopeFactory scopeFactory) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Exception occurred: {Message}", exception.Message);



            // Resolve ILocalizer from a request-lifetime scope. 
            // The localizer will return strings based on the request's current culture (Accept-Language).
            await using var scope = scopeFactory.CreateAsyncScope();
            var localizer = scope.ServiceProvider.GetRequiredService<ILocalizer>();

            // Map Exceptions to Status Codes & Messages
            var (statusCode, titleKey, detail) = exception switch
            {
                ValidationException =>
                    (StatusCodes.Status400BadRequest, "ValidationError", localizer["ValidationError"]),

                UniqueConstraintException =>
                    (StatusCodes.Status409Conflict, "Conflict", localizer["Conflict"]),

                Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException =>
                    (StatusCodes.Status409Conflict, "Conflict", localizer["ConcurrencyError"]),

                ReferenceConstraintException =>
                    (StatusCodes.Status400BadRequest, "RelatedData", localizer["RelatedData"]),

                NotFoundException notFoundEx =>
                    (StatusCodes.Status404NotFound, "NotFound", localizer[notFoundEx.MessageKey, notFoundEx.Args]),

                UnauthorizedException authEx =>
                    (StatusCodes.Status401Unauthorized, "Unauthorized", localizer[authEx.MessageKey, authEx.Args]),

                DomainException domEx =>
                    (StatusCodes.Status400BadRequest, "DomainError", localizer[domEx.MessageKey, domEx.Args]),

                _ =>
                    (StatusCodes.Status500InternalServerError, "ServerError", localizer["ServerError"])
            };

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = localizer[titleKey],
                Detail = detail,
                Instance = httpContext.Request.Path
            };

            // If it's a validation error, include the field-level error messages
            if (exception is ValidationException fluentEx)
            {
                problemDetails.Extensions["errors"] = fluentEx.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());
            }

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
