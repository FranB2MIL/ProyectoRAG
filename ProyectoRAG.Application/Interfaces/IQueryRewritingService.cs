namespace ProyectoRAG.Application.Interfaces;

public interface IQueryRewritingService
{
    Task<IEnumerable<string>> GenerateAlternativeQueriesAsync(string originalQuery);
}