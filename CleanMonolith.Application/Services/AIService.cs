using CleanMonolith.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace CleanMonolith.Application.Services
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public AIService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<string> GetResponse(string prompt)
        {
            var apiKey = _config["OpenAI:ApiKey"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var body = new
            {
                model = "gpt-4.1-mini",
                messages = new[]
                {
            new { role = "user", content = prompt }
        }
            };

            var response = await _httpClient.PostAsJsonAsync(
                "v1/chat/completions",
                body
            );

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return $"Error: {content}";

            dynamic result = System.Text.Json.JsonSerializer.Deserialize<dynamic>(content);

            return result?.GetProperty("choices")[0]
                         .GetProperty("message")
                         .GetProperty("content")
                         .ToString();
        }
    }
}
