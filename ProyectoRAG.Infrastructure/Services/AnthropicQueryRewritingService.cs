using Anthropic.SDK;
using Anthropic.SDK.Messaging;
using ProyectoRAG.Application.Interfaces;

namespace ProyectoRAG.Infrastructure.Services;

public class AnthropicQueryRewritingService : IQueryRewritingService
{
    private readonly AnthropicClient _client;

    public AnthropicQueryRewritingService(string apiKey)
    {
        _client = new AnthropicClient(apiKey);
    }

    public async Task<IEnumerable<string>> GenerateAlternativeQueriesAsync(string originalQuery)
    {
        var response = await _client.Messages.GetClaudeMessageAsync(new MessageParameters
        {
            Model = "claude-haiku-4-5",
            MaxTokens = 300,
            System = new List<SystemMessage> { new SystemMessage("""
                You are a search query optimizer for a Destiny lore database.
                Given a question, generate 3 alternative search queries that would help find relevant information.
                Each query should approach the topic from a different angle.
                Respond with ONLY the 3 queries, one per line, no numbering, no explanation.
                """) },
            Messages = new List<Message>
            {
                new Message(RoleType.User, originalQuery)
            }
        });

        var text = response.Content.OfType<TextContent>().First().Text;
        return text.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                   .Select(q => q.Trim())
                   .Where(q => !string.IsNullOrWhiteSpace(q))
                   .Take(3);
    }
}