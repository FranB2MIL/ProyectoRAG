namespace ProyectoRAG.Application.Interfaces;

public interface IChatService
{
    Task<string> AskAsync(string question, IEnumerable<string> context);
}