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
        var body = new { input = texts, model = "voyage-03" };
        var response = await _httpClient.PostAsJsonAsync("https://api.voyageai.com/v1/embeddings", body);
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
}