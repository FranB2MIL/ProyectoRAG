using AspNetCoreRateLimit;
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

// Rate limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false;
    options.HttpStatusCode = 429;
    options.RealIpHeader = "X-Real-IP";
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "POST:/api/documents/ask",
            Period = "1m",
            Limit = 5
        },
        new RateLimitRule
        {
            Endpoint = "POST:/api/documents/ask",
            Period = "1h",
            Limit = 30
        }
    };
});
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddInMemoryRateLimiting();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseIpRateLimiting();

// app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.MapControllers();

app.Run();