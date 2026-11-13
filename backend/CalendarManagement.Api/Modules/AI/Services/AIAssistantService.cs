using CalendarManagement.Api.Interfaces;
using CalendarManagement.Api.Modules.AI.Interfaces;
using CalendarManagement.Api.Modules.AI.Models;
using CalendarManagement.Api.Modules.AI.Services;
using CalendarManagement.Api.Modules.AI.Tools;
using CalendarManagement.Api.Services.Email;
using Microsoft.Extensions.Logging;

namespace CalendarManagement.Api.Modules.AI.Services;

public class AIAssistantService : IAIAssistantService
{
    private readonly IAIProvider _aiProvider;
    private readonly ILogger<AIAssistantService> _logger;
    private readonly Dictionary<string, IAITool> _tools;
    private readonly Dictionary<string, List<AIChatMessage>> _conversations = new();

    public AIAssistantService(
        IAIProvider aiProvider,
        IEnumerable<IAITool> tools,
        ILogger<AIAssistantService> logger)
    {
        _aiProvider = aiProvider;
        _logger = logger;
        _tools = tools.ToDictionary(t => t.Name, t => t);
    }

    public async Task<AIChatResponse> ProcessMessageAsync(AIChatRequest request, CancellationToken cancellationToken = default)
    {
        var conversationId = request.ConversationId ?? Guid.NewGuid().ToString();
        if (!_conversations.TryGetValue(conversationId, out var history))
        {
            history = new List<AIChatMessage>();
            _conversations[conversationId] = history;
        }

        // Add user message
        history.Add(new AIChatMessage { Role = "user", Content = request.Message });

        // Build tool definitions for the AI
        var toolDefinitions = BuildToolDefinitions();

        // Get AI response
        var systemPrompt = BuildSystemPrompt();
        var aiResponse = await _aiProvider.GetCompletionAsync(systemPrompt, history, toolDefinitions, cancellationToken);

        // Check if AI wants to call a tool
        var toolCall = ParseToolCall(aiResponse);
        if (toolCall != null)
        {
            var toolResult = await ExecuteToolAsync(toolCall.Value.ToolName, toolCall.Value.Parameters, cancellationToken);
            if (toolResult.Success)
            {
                // Get a follow-up response with the tool result
                history.Add(new AIChatMessage { Role = "assistant", Content = $"I called {toolCall.Value.ToolName} and got the result." });
                history.Add(new AIChatMessage { Role = "tool", Content = System.Text.Json.JsonSerializer.Serialize(toolResult.Result) });
                
                var followUpResponse = await _aiProvider.GetCompletionAsync(systemPrompt, history, null, cancellationToken);
                history.Add(new AIChatMessage { Role = "assistant", Content = followUpResponse });
                
                return new AIChatResponse
                {
                    Message = followUpResponse,
                    ConversationId = conversationId,
                    History = history.ToList()
                };
            }
            else
            {
                var errorMsg = $"I tried to use {toolCall.Value.ToolName} but encountered an error: {toolResult.Error}";
                history.Add(new AIChatMessage { Role = "assistant", Content = errorMsg });
                return new AIChatResponse
                {
                    Message = errorMsg,
                    ConversationId = conversationId,
                    History = history.ToList()
                };
            }
        }

        // No tool call - just return the AI response
        history.Add(new AIChatMessage { Role = "assistant", Content = aiResponse });
        return new AIChatResponse
        {
            Message = aiResponse,
            ConversationId = conversationId,
            History = history.ToList()
        };
    }

    public List<AIChatMessage> GetConversationHistory(string conversationId)
    {
        return _conversations.TryGetValue(conversationId, out var history) ? history : new List<AIChatMessage>();
    }

    public void ClearConversation(string conversationId)
    {
        _conversations.Remove(conversationId);
    }

    private string BuildSystemPrompt()
    {
        return @"You are an AI assistant for the Calendar Management System. You can help users with:

1. Calendar Events: View, create, update, and delete events and meetings
2. Dashboard: Get summary statistics about events and meetings
3. Meetings: View upcoming and past meetings

Available tools:
- GetUpcomingEvents: Get upcoming calendar events
- GetPastEvents: Get past calendar events
- GetDashboardSummary: Get dashboard statistics
- GetEventById: Get details of a specific event by ID
- CreateEvent: Create a new event or meeting
- DeleteEvent: Delete an event by ID

When users ask about events or meetings, use the appropriate tools to get real data.
Always be helpful and provide clear, concise responses.
If you're unsure about something, ask for clarification.
For actions that modify data (create, delete), confirm with the user first.";
    }

    private string BuildToolDefinitions()
    {
        var definitions = _tools.Values.Select(t => new
        {
            name = t.Name,
            description = t.Description
        });
        return System.Text.Json.JsonSerializer.Serialize(definitions);
    }

    private (string ToolName, Dictionary<string, object> Parameters)? ParseToolCall(string response)
    {
        // Simple parsing - look for tool call patterns
        // In a real implementation, this would use function calling API
        var toolNames = _tools.Keys.ToList();
        foreach (var toolName in toolNames)
        {
            if (response.Contains($"[{toolName}]") || response.Contains($"use {toolName}"))
            {
                return (toolName, new Dictionary<string, object>());
            }
        }
        return null;
    }

    private async Task<AIToolResult> ExecuteToolAsync(string toolName, Dictionary<string, object> parameters, CancellationToken cancellationToken)
    {
        if (_tools.TryGetValue(toolName, out var tool))
        {
            return await tool.ExecuteAsync(parameters, cancellationToken);
        }
        return new AIToolResult
        {
            ToolName = toolName,
            Success = false,
            Error = $"Tool '{toolName}' not found."
        };
    }
}
