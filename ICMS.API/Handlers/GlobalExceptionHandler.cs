using EntityFramework.Exceptions.Common;
using FluentValidation;
using ICMS.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using ICMS.Application.Interfaces.Services;

namespace ICMS.API.Handlers
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, ILocalizer localizer) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

            // Map Exceptions to Status Codes & Messages
            var (statusCode, titleKey, detail) = exception switch
            {
                ValidationException =>
                    (StatusCodes.Status400BadRequest, "ValidationError", localizer["ValidationError"]),

                UniqueConstraintException =>
                    (StatusCodes.Status409Conflict, "Conflict", localizer["Conflict"]),

                ReferenceConstraintException =>
                    (StatusCodes.Status400BadRequest, "RelatedData", localizer["RelatedData"]),

                DomainException domEx =>
                    (StatusCodes.Status400BadRequest, "DomainError", localizer[domEx.MessageKey, domEx.Args]),

                _ =>
                    (StatusCodes.Status500InternalServerError, "ServerError", localizer["ServerError"])
            };

            // Override specific domain exceptions with appropriate status codes if needed
            if (exception is NotFoundException notFoundEx)
            {
                statusCode = StatusCodes.Status404NotFound;
                titleKey = "NotFound";
                detail = localizer[notFoundEx.MessageKey, notFoundEx.Args];
            }

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = localizer[titleKey],
                Detail = detail,
                Instance = httpContext.Request.Path
            };

            // If it's a validation error, add the specific field errors
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
