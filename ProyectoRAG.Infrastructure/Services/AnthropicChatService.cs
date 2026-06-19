using Anthropic.SDK;
using Anthropic.SDK.Messaging;
using ProyectoRAG.Application.Interfaces;

namespace ProyectoRAG.Infrastructure.Services;

public class AnthropicChatService : IChatService
{
    private readonly AnthropicClient _client;
    public AnthropicChatService(string apiKey)
    {
        _client = new AnthropicClient(apiKey);
    }

    public async Task<string> AskAsync(string question, IEnumerable<string> context)
    {
        var contextText = string.Join("\n\n", context);
        var systemPrompt = $"""
            Answer questions based ONLY on the provided context.
            If the answer is not in the context, say you dont have that information.
            Context:
            {contextText}
            """;
        var response = await _client.Messages.GetClaudeMessageAsync(new MessageParameters
        {
            Model = "claude-sonnet-4-6",
            MaxTokens = 1024,
            System = new List<SystemMessage> { new SystemMessage(systemPrompt) },
            Messages = new List<Message>
            {
                new Message(RoleType.User, question)
            }
        });
        return response.Content.OfType<TextContent>().First().Text;
    }
}