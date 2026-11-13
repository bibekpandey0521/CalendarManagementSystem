namespace CalendarManagement.Api.Modules.AI.Models;

public class AIChatRequest
{
    public string Message { get; set; } = string.Empty;
    public string? ConversationId { get; set; }
    public string? UserId { get; set; }
}

public class AIChatResponse
{
    public string Message { get; set; } = string.Empty;
    public string ConversationId { get; set; } = string.Empty;
    public List<AIChatMessage> History { get; set; } = new();
    public bool RequiresConfirmation { get; set; }
    public string? PendingAction { get; set; }
}

public class AIChatMessage
{
    public string Role { get; set; } = string.Empty; // user, assistant, system, tool
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class AIToolDefinition
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
}

public class AIToolResult
{
    public string ToolName { get; set; } = string.Empty;
    public object? Result { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
}
