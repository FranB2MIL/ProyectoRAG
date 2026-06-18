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

    public DocumentsController(
        IDocumentRepository repository,
        IEmbeddingService embeddingService)
    {
        _repository = repository;
        _embeddingService = embeddingService;
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
}