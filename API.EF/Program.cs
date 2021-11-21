//Pacotes para Instalar SeriLog
//dotnet add package serilog.aspnetcore
//dotnet add package serilog.sinks.seq
//dotnet add package serilog.expressions

using API.EF.Extensions;
using API.EF.Infra;
using API.EF.Models.DTOs.Mapping;
using API.EF.Repository.UOWR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Extensions.Logging;
using System.Text.Json.Serialization;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    //SeriLog
    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console()
        .ReadFrom.Configuration(ctx.Configuration));

    // Add services to the container.

    //Config AutoMapper
    var mappingConfig = new MapperConfiguration(mc =>
    {
        mc.AddProfile(new MappingProfile());
    });
    IMapper mapper = mappingConfig.CreateMapper();
    builder.Services.AddSingleton(mapper);

    //Unit Of Work injeção
    builder.Services.AddScoped<IUOW, UOW>();

    //DbContext Mysql
    var connectionString = builder.Configuration.GetConnectionString("Default");
    builder.Services.AddDbContext<IUoW>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

    builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        //Obsoleto -- options.JsonSerializerOptions.IgnoreNullValues = true;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
        //options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        //options.JsonSerializerOptions.PropertyNamingPolicy = null; // prevent camel case
    });

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "EF Api", Version = "v1" });
    });

    builder.Logging.AddProvider(new SerilogLoggerProvider(null, false));

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EF Api v1"));
    }

    //Method for Middleware Exception
    app.ConfigureExceptionHandler();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{

    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}

