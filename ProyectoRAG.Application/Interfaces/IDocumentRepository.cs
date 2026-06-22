namespace ProyectoRAG.Application.Interfaces
{
    public interface IDocumentRepository
    {
        Task SaveDocumentAsync(string content, double[] embedding);
        Task<IEnumerable<string>> SearchSimilarAsync(double[] queryEmbedding,string queryText, int topK = 3);
    }
}