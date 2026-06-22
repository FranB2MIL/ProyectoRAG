using ProyectoRAG.Application.Interfaces;
using ProyectoRAG.Infrastructure.Repositories;
using ProyectoRAG.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",  // Vite default
                "http://localhost:3000"   // CRA fallback
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddHttpClient<IEmbeddingService, VoyageEmbeddingService>(client =>
{
    var apiKey = builder.Configuration["VoyageAI:ApiKey"];
    Console.WriteLine($"API Key loaded: {(string.IsNullOrEmpty(apiKey) ? "NOT FOUND" : "OK")}");
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
});
var anthropicApiKey = builder.Configuration["Anthropic:ApiKey"];
builder.Services.AddSingleton<IChatService>(
    new AnthropicChatService(anthropicApiKey!));

builder.Services.AddScoped<IDocxReader, OpenXmlDocxReader>();
builder.Services.AddSingleton<IQueryRewritingService>(
    new AnthropicQueryRewritingService(anthropicApiKey!));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSingleton<IDocumentRepository>(
    new DocumentRepository(connectionString!));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.MapControllers();

app.Run();