using Npgsql;
using Pgvector;
using ProyectoRAG.Application.Interfaces;

namespace ProyectoRAG.Infrastructure.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public DocumentRepository(string connectionString)
    {
        var builder = new NpgsqlDataSourceBuilder(connectionString);
        builder.UseVector();
        _dataSource = builder.Build();
    }

    public async Task SaveDocumentAsync(string content, double[] embedding)
    {
        var vector = new Vector(embedding.Select(d => (float)d).ToArray());

        await using var cmd = _dataSource.CreateCommand(
            "INSERT INTO documents (content, embedding) VALUES (@content, @embedding)");
        cmd.Parameters.AddWithValue("content", content);
        cmd.Parameters.AddWithValue("embedding", vector);

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<IEnumerable<string>> SearchSimilarAsync(double[] queryEmbedding, int topK = 3)
    {
        var vector = new Vector(queryEmbedding.Select(d => (float)d).ToArray());

        await using var cmd = _dataSource.CreateCommand(
            @"SELECT content FROM documents 
              ORDER BY embedding <=> @embedding 
              LIMIT @topK");
        cmd.Parameters.AddWithValue("embedding", vector);
        cmd.Parameters.AddWithValue("topK", topK);

        var results = new List<string>();
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            results.Add(reader.GetString(0));

        return results;
    }
}