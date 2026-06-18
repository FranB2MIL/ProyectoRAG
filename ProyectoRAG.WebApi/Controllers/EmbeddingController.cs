using Microsoft.AspNetCore.Mvc;
using ProyectoRAG.Application.Interfaces;

namespace ProyectoRAG.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmbeddingController : ControllerBase
{
    private readonly IEmbeddingService _embeddingService;

    public EmbeddingController(IEmbeddingService embeddingService)
    {
        _embeddingService = embeddingService;
    }

    [HttpPost]
    public async Task<ActionResult<EmbeddingResponse>> GenerateEmbedding(EmbeddingRequest request)
    {
        var embedding = await _embeddingService.GenerateEmbeddingAsync(request.Text);
        return Ok(new EmbeddingResponse(embedding));
    }
}