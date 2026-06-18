namespace ProyectoRAG.Application.Interfaces
{
    public interface IEmbeddingService
    {
        Task<double[]> GenerateEmbeddingAsync(string text);
        Task<double[][]> GenerateEmbeddingsAsync(string[] texts);
    }
}