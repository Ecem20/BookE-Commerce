using OrdersAPI.Models;
using OrdersAPI.Models.Dto;
using Microsoft.AspNetCore.Diagnostics;

namespace OrdersAPI
{
    public class CustomExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var errorMessage = exception.Message;

            var generalErrorResponse = new ResponseDto
            {
                Result = null,
                IsSuccess = false,
                Message = errorMessage,
                TotalPages = 0
            };
            await httpContext.Response.WriteAsJsonAsync(generalErrorResponse, cancellationToken);
            return true;
        }
    }
}