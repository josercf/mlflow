using Prometheus;
using AutoMLDemo.Services;
using AutoMLDemo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Configurar URL para sempre usar porta 5000
builder.WebHost.UseUrls("http://0.0.0.0:5000");

// Configurar serviços
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IMLService, MLService>();

// Configurar logging
builder.Logging.AddConsole();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.MapControllers();

// Configurar métricas Prometheus
app.UseMetricServer(); // Endpoint /metrics
app.UseHttpMetrics();  // Métricas HTTP automáticas

// Obter a porta configurada dinamicamente
var urls = builder.Configuration.GetValue<string>("ASPNETCORE_URLS") ?? "http://localhost:5000";
var port = urls.Contains(":") ? urls.Split(':').Last() : "5000";
var baseUrl = $"http://localhost:{port}";

Console.WriteLine("🚀 API ML.NET iniciada!");
Console.WriteLine($"📊 Métricas: {baseUrl}/metrics");
Console.WriteLine($"🔍 Swagger: {baseUrl}/swagger");
Console.WriteLine($"❤️  Health: {baseUrl}/api/prediction/health");
Console.WriteLine("\n📋 Exemplo de uso:");
Console.WriteLine($"POST {baseUrl}/api/prediction/predict");
Console.WriteLine("Content-Type: application/json");
Console.WriteLine("{\n  \"userId\": 1,\n  \"movieId\": 10\n}");

await app.RunAsync();
