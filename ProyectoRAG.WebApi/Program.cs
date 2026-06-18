using ProyectoRAG.Application.Interfaces;
using ProyectoRAG.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

builder.Services.AddHttpClient<IEmbeddingService, VoyageEmbeddingService>(client =>
{
    var apiKey = builder.Configuration["VoyageAI:ApiKey"];
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
});


app.Run();

