using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = ".NIES API", Version = "v1" });
});

builder.Services.AddHealthChecks();

// Poort ophalen en gebruiken
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// Zet tijdelijk https redirect UIT tijdens debuggen
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHealthChecks("/");
app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", ".NIES API v1");
});

app.Run();