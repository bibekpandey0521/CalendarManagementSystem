using CalendarManagement.Api.Modules.AI.Models;

namespace CalendarManagement.Api.Modules.AI.Interfaces;

public interface IAIProvider
{
    Task<string> GetCompletionAsync(string systemPrompt, List<AIChatMessage> messages, string? toolDefinitions = null, CancellationToken cancellationToken = default);
    string ProviderName { get; }
}

public interface IAITool
{
    string Name { get; }
    string Description { get; }
    Task<AIToolResult> ExecuteAsync(Dictionary<string, object> parameters, CancellationToken cancellationToken = default);
}

public interface IAIAssistantService
{
    Task<AIChatResponse> ProcessMessageAsync(AIChatRequest request, CancellationToken cancellationToken = default);
    List<AIChatMessage> GetConversationHistory(string conversationId);
    void ClearConversation(string conversationId);
}
