using ExchangeRateClientService.Clients;
using ExchangeRateClientService.Dtos;
using ExchangeRateClientService.Services;
using ExchangeRateClientService.Utils;

// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.
// // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();

// app.Run();

//Test code for api request
const string API_KEY = "c511651c1c924ddc9b621f261b3ee649";
string url = $"https://openexchangerates.org/api/latest.json?app_id={API_KEY}&base=USD&prettyprint=true&show_alternative=false";

var httpClient = new HttpClient();
var apiRequestWrapper = new APIRequestWrapper(httpClient);
var exchangeApiClient = new ExchangeAPIClient(apiRequestWrapper, API_KEY);

var data = await exchangeApiClient.GetExchangeRateDataAsync();

if (data != null)
{
    Console.WriteLine($"Timestamp: {data.Timestamp}");
    Console.WriteLine($"Rate: 1USD->{data.Rates["KRW"]}₩");

    var eur = CurrencyConvertor.ConvertToKRW(data, "EUR");
    Console.WriteLine($"Rate: 1EUR->{eur}₩");
}
else
{
    Console.WriteLine("API Result is NULL");
}
//test code ends