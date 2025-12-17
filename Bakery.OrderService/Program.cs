using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Aspire ServiceDefaults
builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // Scalar UI disponibile a /scalar/v1
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Aspire default endpoints
app.MapDefaultEndpoints();

app.Run();
