using Infrastructure.AI.Interfaces;
using OllamaSharp;
using OllamaSharp.Models.Chat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;

namespace Infrastructure.AI.Chat
{
    public class LoggingAgentChat : IAgentChat
    {
        private readonly OllamaApiClient _ollama;

        public LoggingAgentChat(OllamaApiClient ollama)
        {
            _ollama = ollama;
        }

        private readonly static string LoggingAgentSysPrompt = File.ReadAllText(
            Path.Combine(AppContext.BaseDirectory, "Resources", "LoggingAgentSysPrompt.txt"));

        public async Task<string?> Chat(Guid applicationId, object? messagesContext)
        {
            string prompt = CreateFormatedPrompt(messagesContext, applicationId);

            var schemaObject = new
            {
                IsFinished = false,
                Message = (string?)null,
                AskUser = false,
                HasError = false
            };

            JsonNode schema = JsonSerializerOptions.Default
                .GetJsonSchemaAsNode(schemaObject.GetType());

            var request = new ChatRequest
            {
                Model = "gemma3",
                Stream = false,
                Format = schema,
                Messages =
                [
                    new Message(ChatRole.System, LoggingAgentSysPrompt),
                    new Message(ChatRole.User, prompt)
                ]
            };

            var response = new StringBuilder();

            await foreach (var chunk in _ollama.ChatAsync(request))
            {
                if(chunk is not null)
                     response.Append(chunk.Message?.Content);
            }

            return response.ToString();
        }

        private string CreateFormatedPrompt(object? messagesContext, Guid applicationId)
        {
            string messagesContextAsString = "There is no previous context yet";

            if (messagesContext is not null)
                messagesContextAsString = "The Chat Context:" + JsonSerializer.Serialize(messagesContext);

            string prompt = $"The APPLICATION_ID={applicationId},{messagesContextAsString}";

            return prompt;
        }
    }
}
