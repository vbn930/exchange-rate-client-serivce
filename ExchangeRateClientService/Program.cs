using ExchangeRateClientService.Azure;
using ExchangeRateClientService.Clients;
using ExchangeRateClientService.Dtos;
using ExchangeRateClientService.Services;
using ExchangeRateClientService.Utils;

using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Telemetry.Trace;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

IConfiguration configuration = builder.Configuration;

string serviceName = configuration["Logging:ServiceName"];
string serviceVersion = configuration["Logging:ServiceVersion"];
string API_KEY = Environment.GetEnvironmentVariable("OPEN_EXCHANGE_RATES_API_KEY");

builder.Services.AddMemoryCache();
builder.Services.AddOpenTelemetry().WithTracing(tcb =>
{
    tcb
    .AddSource(serviceName)
    .SetResourceBuilder(
        ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
    .AddAspNetCoreInstrumentation()
    .AddJsonConsoleExporter();
});

builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton<APIRequestWrapper>();
builder.Services.AddSingleton<CosmosDbWrapper>();
builder.Services.AddSingleton<KeyVaultWapper>();
builder.Services.AddSingleton(provider =>
{
    var configuration = provider.GetService<IConfiguration>();
    var apiRequestWrapper = provider.GetService<APIRequestWrapper>();
    var cosmosDbWrapper = provider.GetService<CosmosDbWrapper>();
    return new ExchangeAPIClient(configuration, apiRequestWrapper, cosmosDbWrapper, API_KEY);
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();