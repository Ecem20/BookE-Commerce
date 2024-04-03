using ShoppingCartAPI.Models;
using ShoppingCartAPI.Models.Dto;
using Microsoft.AspNetCore.Diagnostics;

namespace ShoppingCartAPI
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
            };
            await httpContext.Response.WriteAsJsonAsync(generalErrorResponse, cancellationToken);
            return true;
        }
    }
}