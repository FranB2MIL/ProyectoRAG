namespace ProyectoRAG.WebApi.Models;

public record IndexRequest(string Content);
public record SearchRequest(string Query, int TopK = 6);