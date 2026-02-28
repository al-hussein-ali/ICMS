using EntityFramework.Exceptions.Common;
using FluentValidation;
using ICMS.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ICMS.API.Handlers
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

            // Map Exceptions to Status Codes & Messages
            var (statusCode, title, detail) = exception switch
            {
                // 1. FluentValidation Errors (400)
                ValidationException =>
                    (StatusCodes.Status400BadRequest, "Validation Error", "One or more validation failures occurred."),

                // 2. Database Unique Constraint (409)
                UniqueConstraintException =>
                    (StatusCodes.Status409Conflict, "Conflict Error", "This record already exists in the system."),

                // 3. Database Reference/Foreign Key Violation (400)
                ReferenceConstraintException =>
                    (StatusCodes.Status400BadRequest, "Related Data Error", "Cannot delete or update because this record is used elsewhere."),

                // 4. Custom Domain Exception (e.g. NotFound)
                NotFoundException =>
                    (StatusCodes.Status404NotFound, "Not Found", exception.Message),

                InvalidDoubleDoseException =>
                (StatusCodes.Status400BadRequest,"Double Dose","This individual has been already toke the dose."),

                // 5. Everything Else (500)
                _ =>
                    (StatusCodes.Status500InternalServerError, "Server Error", "An unexpected error occurred.")
            };

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
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
