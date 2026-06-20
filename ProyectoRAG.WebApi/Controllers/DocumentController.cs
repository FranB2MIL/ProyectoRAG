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
    private readonly IExcelReader _excelReader;
    public DocumentsController(
        IDocumentRepository repository,
        IEmbeddingService embeddingService,
        IChatService chatService,
        IExcelReader excelReader)
    {
        _repository = repository;
        _embeddingService = embeddingService;
        _chatService = chatService;
        _excelReader = excelReader;
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

    [HttpPost("import-excel")]
    public async Task<IActionResult> ImportExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        using var stream = file.OpenReadStream();
        var vehicles = _excelReader.ReadVehicles(stream).ToList();

        var texts = vehicles.Select(v => v.ToEmbeddingText()).ToArray();
        var embeddings = await _embeddingService.GenerateEmbeddingsAsync(texts);

        for (int i = 0; i < vehicles.Count; i++)
        {
            await _repository.SaveDocumentAsync(texts[i], embeddings[i]);
        }

        return Ok(new { message = $"{vehicles.Count} vehicles indexed successfully" });
    }
}