using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using System.Net;

namespace API.EF.Extensions
{
    public static class ApiExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var guid = Guid.NewGuid().ToString();
                    if (contextFeature != null)
                    {
                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "An error was encountered. Contact technical support.",
                            Trace = guid

                        }.ToString());

                        Log.Error($"==============================ERROR EXCEPTION INIT | GUID: {guid} ==============================");
                        Log.Error("===Status Code: " + context.Response.StatusCode);
                        Log.Error("===Message: " + contextFeature.Error.Message);
                        Log.Error("===Trace: " + contextFeature.Error.StackTrace);
                        Log.Error($"==============================ERROR EXCEPTION FINAL | GUID: {guid} ==============================");

                    }
                }
                );
            }
            );
        }
    }
}
//Method for Middleware Exception
//app.ConfigureExceptionHandler();

//Recomendado gravar em tabela do banco o GUID / DataHora