using Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MetalProcessingSolution.Extensions
{
    public class CustomExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var isValidation = exception is AppValidationException;
            var isNotFound = exception is NotFoundException;

            var statusCode = isValidation ? StatusCodes.Status400BadRequest
                             : isNotFound ? StatusCodes.Status404NotFound
                             : StatusCodes.Status500InternalServerError;

            var title = isValidation ? "Ошибка данных" : isNotFound ? "Не найдено" : "Ошибка сервера завода";

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = exception.Message,
                Instance = httpContext.Request.Path
            }, cancellationToken);

            return true;
        }
    }
}
