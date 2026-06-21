using System.Net.Http.Json;
using System.Text.Json;
using ProyectoRAG.Application.Interfaces;

namespace ProyectoRAG.Infrastructure.Services;

public class VoyageEmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;

    public VoyageEmbeddingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<double[]> GenerateEmbeddingAsync(string text)
    {
        var results = await GenerateEmbeddingsAsync(new[] { text });
        return results[0];
    }

    public async Task<double[][]> GenerateEmbeddingsAsync(string[] texts)
{
    var body = new { input = texts, model = "voyage-3" };

    const int maxRetries = 5;
    for (int attempt = 0; attempt <= maxRetries; attempt++)
    {
        var response = await _httpClient.PostAsJsonAsync("https://api.voyageai.com/v1/embeddings", body);

        if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            if (attempt == maxRetries)
                response.EnsureSuccessStatusCode(); // give up and throw after final attempt

            var waitSeconds = 30 * (attempt + 1); // 30s, 60s, 90s, 120s, 150s
            Console.WriteLine($"Rate limited by Voyage AI. Waiting {waitSeconds}s before retry {attempt + 1}/{maxRetries}...");
            await Task.Delay(TimeSpan.FromSeconds(waitSeconds));
            continue;
        }

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        return result.GetProperty("data")
            .EnumerateArray()
            .Select(e => e.GetProperty("embedding")
                .EnumerateArray()
                .Select(v => v.GetDouble())
                .ToArray())
            .ToArray();
    }

    throw new InvalidOperationException("Unreachable");
}
}