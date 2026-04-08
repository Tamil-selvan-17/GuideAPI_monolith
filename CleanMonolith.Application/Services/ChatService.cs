using CleanMonolith.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace CleanMonolith.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IAIService _aiService;

        public ChatService(IAIService aiService)
        {
            _aiService = aiService;
        }

        public async Task<string> GetReplyAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return "Please enter a valid message.";

            // Simple prompt (can enhance later)
            var prompt = $@"
                            You are a helpful assistant.

                            User:
                            {message}
                            ";

            return await _aiService.GetResponse(prompt);
        }
    }
}
