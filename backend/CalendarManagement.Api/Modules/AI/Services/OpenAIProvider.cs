using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CalendarManagement.Api.Modules.AI.Interfaces;
using CalendarManagement.Api.Modules.AI.Models;
using Microsoft.Extensions.Logging;

namespace CalendarManagement.Api.Modules.AI.Services;

public class OpenAIProvider : IAIProvider
{
    private readonly ILogger<OpenAIProvider> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public OpenAIProvider(IConfiguration configuration, ILogger<OpenAIProvider> logger, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("OpenAI");
    }

    public string ProviderName => "OpenAI";

    public async Task<string> GetCompletionAsync(string systemPrompt, List<AIChatMessage> messages, string? toolDefinitions = null, CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration["OPENAI_API_KEY"] ?? _configuration["Ai:OpenAI:ApiKey"];
        var modelId = _configuration["OPENAI_MODEL"] ?? _configuration["Ai:OpenAI:Model"] ?? "gpt-4o-mini";
        var endpoint = _configuration["OPENAI_ENDPOINT"] ?? _configuration["Ai:OpenAI:Endpoint"] ?? "https://api.openai.com/v1/chat/completions";

        if (string.IsNullOrEmpty(apiKey))
        {
            return "I'm sorry, the AI service is not configured. Please set the OPENAI_API_KEY environment variable or configure it in appsettings.json.";
        }

        try
        {
            var requestMessages = new List<object>();
            requestMessages.Add(new { role = "system", content = systemPrompt });
            foreach (var msg in messages)
            {
                requestMessages.Add(new { role = msg.Role, content = msg.Content });
            }

            var requestBody = new
            {
                model = modelId,
                messages = requestMessages,
                temperature = 0.7,
                max_tokens = 1000
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("OpenAI API error: {StatusCode} - {Error}", response.StatusCode, errorContent);
                return $"AI service error: {response.StatusCode}";
            }

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(responseJson);
            var messageContent = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return messageContent ?? "I couldn't generate a response.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting AI completion");
            return $"Error: {ex.Message}";
        }
    }
}
