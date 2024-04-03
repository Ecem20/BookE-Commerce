using BookAPI.Models;
using BookAPI.Models.Dto;
using Microsoft.AspNetCore.Diagnostics;

namespace BookAPI
{
    public class CustomExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var errorMessage = exception.Message;
            if (errorMessage == "Invalid category ID")
            {
                var errorResponse = new ResponseDto
                {
                    Result = null,
                    IsSuccess = false,
                    Message = "Invalid category ID",
                    TotalPages = 0
                };
                await httpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken);
                return true;
            }

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