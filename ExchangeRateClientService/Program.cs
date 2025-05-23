using ExchangeRateClientService.Clients;
using ExchangeRateClientService.Dtos;
using ExchangeRateClientService.Services;
using ExchangeRateClientService.Utils;

using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Telemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

string serviceName = configuration["Logging:ServiceName"];
string serviceVersion = configuration["Logging:ServiceVersion"];

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
builder.Services.AddSingleton(provider =>
{
    var configuration = provider.GetService<IConfiguration>();
    var apiRequestWrapper = provider.GetService<APIRequestWrapper>();
    string token = "c511651c1c924ddc9b621f261b3ee649";
    return new ExchangeAPIClient(configuration, apiRequestWrapper, token);
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