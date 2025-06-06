using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoMLDemo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IMLService, MLService>();

builder.Logging.AddConsole();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();

app.UseMetricServer();
app.UseHttpMetrics();

Console.WriteLine("\uD83D\uDE80 API ML.NET iniciada!");
Console.WriteLine("\uD83D\uDCCA M\u00e9tricas: http://localhost:5000/metrics");
Console.WriteLine("\uD83D\uDD0D Swagger: http://localhost:5000/swagger");
Console.WriteLine("\u2764\uFE0F  Health: http://localhost:5000/api/prediction/health");
Console.WriteLine("\n\uD83D\uDCCB Exemplo de uso:");
Console.WriteLine("POST http://localhost:5000/api/prediction/predict");
Console.WriteLine("Content-Type: application/json");
Console.WriteLine("{\n  \"userId\": 1,\n  \"movieId\": 10\n}");

await app.RunAsync();
