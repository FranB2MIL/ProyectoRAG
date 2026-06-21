using Microsoft.AspNetCore.Mvc;
using ProyectoRAG.Application.Interfaces;
using ProyectoRAG.WebApi.Models;

namespace ProyectoRAG.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentRepository _repository;
    private readonly IEmbeddingService _embeddingService;
    private readonly IChatService _chatService;
    private readonly IDocxReader _docxReader;

    public DocumentsController(
        IDocumentRepository repository,
        IEmbeddingService embeddingService,
        IChatService chatService,
        IDocxReader docxReader)
    {
        _repository = repository;
        _embeddingService = embeddingService;
        _chatService = chatService;
        _docxReader = docxReader;
    }

    [HttpPost("index")]
    public async Task<IActionResult> Index([FromBody] IndexRequest request)
    {
        var embedding = await _embeddingService.GenerateEmbeddingAsync(request.Content);
        await _repository.SaveDocumentAsync(request.Content, embedding);
        return Ok(new { message = "Document indexed successfully" });
    }

    [HttpPost("search")]
    public async Task<ActionResult<IEnumerable<string>>> Search([FromBody] SearchRequest request)
    {
        var embedding = await _embeddingService.GenerateEmbeddingAsync(request.Query);
        var results = await _repository.SearchSimilarAsync(embedding, request.TopK);
        return Ok(results);
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] SearchRequest request)
    {
        var embedding = await _embeddingService.GenerateEmbeddingAsync(request.Query);
        var relevantChunks = await _repository.SearchSimilarAsync(embedding, request.TopK);
        var answer = await _chatService.AskAsync(request.Query, relevantChunks);
        return Ok(new { answer });
    }

    [HttpPost("import-docx")]
    public async Task<IActionResult> ImportDocx(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        using var stream = file.OpenReadStream();
        var entries = _docxReader.ReadLoreEntries(stream).ToList();

        const int batchSize = 20;
        int totalIndexed = 0;

        for (int i = 0; i < entries.Count; i += batchSize)
        {
            var batch = entries.Skip(i).Take(batchSize).ToList();
            var texts = batch.Select(e => e.ToEmbeddingText()).ToArray();
            var embeddings = await _embeddingService.GenerateEmbeddingsAsync(texts);

            for (int j = 0; j < batch.Count; j++)
            {
                await _repository.SaveDocumentAsync(texts[j], embeddings[j]);
            }

            totalIndexed += batch.Count;
            Console.WriteLine($"Indexed batch {i / batchSize + 1}: {totalIndexed}/{entries.Count} entries");

            if (i + batchSize < entries.Count)
                await Task.Delay(8000); // avoid hitting Voyage AI rate limit between batches
        }

        return Ok(new { message = $"{totalIndexed} lore entries indexed successfully" });
    }
}