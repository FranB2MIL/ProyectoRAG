namespace ProyectoRAG.Domain.Entities;

public class LoreEntry
{
    public string Section { get; set; } = string.Empty;
    public string SubTopic { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public string ToEmbeddingText()
    {
        var subTopicText = string.IsNullOrWhiteSpace(SubTopic) ? "" : $" — {SubTopic}";
        return $"[{Section}{subTopicText}]\n{Content}";
    }
}