namespace ProyectoRAG.Application.Interfaces;

public record EmbeddingRequest(string Text);
public record EmbeddingResponse(double[] Embedding);
