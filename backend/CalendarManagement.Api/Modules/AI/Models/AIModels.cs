namespace CalendarManagement.Api.Modules.AI.Models;

/// <summary>
/// Request payload for the AI assistant chat endpoint.
/// </summary>
public class AIChatRequest
{
    /// <summary>User message to send to the assistant.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Optional conversation identifier for continuity.</summary>
    public string? ConversationId { get; set; }

    /// <summary>Optional user identifier.</summary>
    public string? UserId { get; set; }
}

/// <summary>
/// Response payload returned by the AI assistant chat endpoint.
/// </summary>
public class AIChatResponse
{
    /// <summary>Assistant response message.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Conversation identifier.</summary>
    public string ConversationId { get; set; } = string.Empty;

    /// <summary>Conversation history.</summary>
    public List<AIChatMessage> History { get; set; } = new();

    /// <summary>Whether the assistant requires user confirmation.</summary>
    public bool RequiresConfirmation { get; set; }

    /// <summary>Pending action awaiting confirmation.</summary>
    public string? PendingAction { get; set; }
}

/// <summary>
/// A single message in an AI conversation.
/// </summary>
public class AIChatMessage
{
    /// <summary>Message role (user, assistant, system or tool).</summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>Message content.</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>Message timestamp (UTC).</summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Definition of an AI tool exposed to the assistant.
/// </summary>
public class AIToolDefinition
{
    /// <summary>Tool name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Tool description.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Tool parameters schema.</summary>
    public Dictionary<string, object> Parameters { get; set; } = new();
}

/// <summary>
/// Result returned by an AI tool execution.
/// </summary>
public class AIToolResult
{
    /// <summary>Name of the executed tool.</summary>
    public string ToolName { get; set; } = string.Empty;

    /// <summary>Tool result payload.</summary>
    public object? Result { get; set; }

    /// <summary>Whether execution succeeded.</summary>
    public bool Success { get; set; }

    /// <summary>Error message when execution fails.</summary>
    public string? Error { get; set; }
}