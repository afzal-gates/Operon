using Operon.Application.Extensions;
using Operon.Application.Common.Exceptions;
using Operon.Application.Common.ViewModels;
using System.Net;
using System.Text.Json;

namespace Operon.Api.Middlewares
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                FailResponseViewModel failResponse = new();

                if (ex.GetType().IsSubclassOf(typeof(BusinessExceptionBase)))
                {
                    failResponse = (ex as BusinessExceptionBase).EmitResult(ex?.Message);
                    response.StatusCode = (int)failResponse.Status.Code;
                }
                else if (ex.GetType().IsSubclassOf(typeof(CustomExceptionBase)))
                {
                    failResponse = (ex as CustomExceptionBase).EmitResult();
                    response.StatusCode = (int)failResponse.Status.Code;
                }
                else
                {
                    response.StatusCode = StatusCodes.Status500InternalServerError;

                    string message = ex.GetExceptionDetails();
                    failResponse = new FailResponseViewModel
                    {
                        Data = null,
                        Status = new StatusViewModel
                        {
                            Message = message,
                            Code = HttpStatusCode.InternalServerError
                        },
                        Exceptions = new string[] { ex.GetType().Name }
                    };
                }

                var serializedResponse = JsonSerializer.Serialize(failResponse);
                _logger.LogError(serializedResponse);
                await context.Response.WriteAsync(serializedResponse);
            }
        }
    }

}
