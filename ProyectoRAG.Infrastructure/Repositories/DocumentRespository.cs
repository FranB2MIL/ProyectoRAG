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

    public async Task<IEnumerable<string>> SearchSimilarAsync(double[] queryEmbedding, string queryText, int topK = 3)
    {
        var vector = new Vector(queryEmbedding.Select(d => (float)d).ToArray());

        await using var cmd = _dataSource.CreateCommand(
            @"WITH semantic AS (
            SELECT id, content,
                   ROW_NUMBER() OVER (ORDER BY embedding <=> @embedding) AS rank
            FROM documents
            LIMIT 20
        ),
        lexical AS (
            SELECT id, content,
                   ROW_NUMBER() OVER (ORDER BY ts_rank(content_tsv, query) DESC) AS rank
            FROM documents, plainto_tsquery('english', @query) query
            WHERE content_tsv @@ query
            LIMIT 20
        ),
        combined AS (
            SELECT COALESCE(s.id, l.id) AS id,
                   COALESCE(s.content, l.content) AS content,
                   COALESCE(1.0 / (60 + s.rank), 0) + 
                   COALESCE(1.0 / (60 + l.rank), 0) AS rrf_score
            FROM semantic s
            FULL OUTER JOIN lexical l ON s.id = l.id
        )
        SELECT content
        FROM combined
        ORDER BY rrf_score DESC
        LIMIT @topK");

        cmd.Parameters.AddWithValue("embedding", vector);
        cmd.Parameters.AddWithValue("query", queryText);
        cmd.Parameters.AddWithValue("topK", topK);

        var results = new List<string>();
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            results.Add(reader.GetString(0));

        return results;
    }
}