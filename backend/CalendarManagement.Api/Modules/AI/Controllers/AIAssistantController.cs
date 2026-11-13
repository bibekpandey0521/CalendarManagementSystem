using CalendarManagement.Api.Modules.AI.Interfaces;
using CalendarManagement.Api.Modules.AI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CalendarManagement.Api.Modules.AI.Controllers;

[ApiController]
[Route("api/ai")]
public sealed class AIAssistantController : ControllerBase
{
    private readonly IAIAssistantService _aiService;

    public AIAssistantController(IAIAssistantService aiService)
    {
        _aiService = aiService;
    }

    /// <summary>
    /// Sends a message to the AI assistant and gets a response.
    /// </summary>
    [HttpPost("chat")]
    [ProducesResponseType(typeof(AIChatResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AIChatResponse>> Chat(
        [FromBody] AIChatRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _aiService.ProcessMessageAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Gets the conversation history for a given conversation ID.
    /// </summary>
    [HttpGet("history/{conversationId}")]
    [ProducesResponseType(typeof(List<AIChatMessage>), StatusCodes.Status200OK)]
    public ActionResult<List<AIChatMessage>> GetHistory(string conversationId)
    {
        var history = _aiService.GetConversationHistory(conversationId);
        return Ok(history);
    }

    /// <summary>
    /// Clears the conversation history.
    /// </summary>
    [HttpDelete("history/{conversationId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public ActionResult ClearHistory(string conversationId)
    {
        _aiService.ClearConversation(conversationId);
        return NoContent();
    }
}
