using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppPromotora.Api.Logger
{
    public class MiddlewareDeExcecaoGlobal
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogger<MiddlewareDeExcecaoGlobal> _logger;
        public MiddlewareDeExcecaoGlobal(RequestDelegate requestDelegate, ILogger<MiddlewareDeExcecaoGlobal> logger)
        {
            _requestDelegate = requestDelegate;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {

            try
            {
                await _requestDelegate(httpContext);

            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {

            _logger.LogError(exception, "MiddlewareDeExcecaoGlobal");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                origem = exception.Source,
                mensagem = exception.Message,
                dataHora = PegaHoraBrasilia()
            }));

        }

        private static DateTime PegaHoraBrasilia() => TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
    }
}
