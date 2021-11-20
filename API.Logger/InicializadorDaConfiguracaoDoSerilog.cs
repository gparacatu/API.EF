
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;

namespace AppPromotora.Api.Logger
{
    public static class InicializadorDaConfiguracaoDoSerilog
    {
        public static void AddSerilogConfig(this IServiceCollection services, string applicationName)
        {
            services.AddLogging();

            string nomeArquivoLog = $"Logs/{PegaHoraBrasilia().ToString("yyyyMMddHHmmss")}_{applicationName.Replace(".", "_")}.log";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Logger(lc => lc.Filter
                                .ByIncludingOnly(e => e.Level == LogEventLevel.Information
                                                || e.Level == LogEventLevel.Warning
                                                || e.Level == LogEventLevel.Debug
                                                || e.Level == LogEventLevel.Fatal
                                                || e.Level == LogEventLevel.Error))
                .WriteTo.Console()
                .WriteTo.Async(a => a.File(nomeArquivoLog), bufferSize: 500)
                .CreateLogger();
        }

        public static void AddSerilogConfig(this ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();
        }

        private static DateTime PegaHoraBrasilia() => TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
    }
}
