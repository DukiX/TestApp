using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestApp.ExceptionHandling
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                string message = "";

                switch (error)
                {
                    case ErrorException e:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;

                        message = JsonSerializer.Serialize(e.Error);
                        break;
                    default:
                        // unhandled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        message = "Unknown Error";
                        break;
                }

                var result = message;
                await response.WriteAsync(result);
            }
        }
    }
}
