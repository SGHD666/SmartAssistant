// <copyright file="ClaudeService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core.Services.LLM
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using SmartAssistant.Core.Models;

    /// <summary>
    /// Service for interacting with Anthropic's Claude language model.
    /// Implements the ILanguageModelService interface to provide Claude AI capabilities.
    /// </summary>
    public class ClaudeService : ILanguageModelService
    {
        /// <summary>
        /// The HTTP client for making API requests.
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        /// The configuration settings for the Claude service.
        /// </summary>
        private readonly LLMConfig config;

        /// <summary>
        /// The rate limiter for limiting API requests.
        /// </summary>
        private readonly RateLimiter rateLimiter;

        /// <summary>
        /// The logger for logging warnings and errors.
        /// </summary>
        private readonly ILogger<ClaudeService> _logger;

        /// <summary>
        /// A flag indicating whether the service is properly configured.
        /// </summary>
        private readonly bool _isConfigured;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaudeService"/> class.
        /// </summary>
        /// <param name="settings">The model settings containing API configuration.</param>
        /// <param name="logger">The logger for logging warnings and errors.</param>
        /// <exception cref="ArgumentNullException">Thrown when settings is null.</exception>
        public ClaudeService(IOptions<ModelSettings> settings, ILogger<ClaudeService> logger)
        {
            this._logger = logger;
            var modelSettings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            this.config = modelSettings.ModelConfigs["Claude"] ?? throw new InvalidOperationException("Claude configuration not found in model settings");

            if (string.IsNullOrEmpty(this.config.BaseUrl))
            {
                this._logger.LogWarning("BaseUrl is not configured for Claude service");
                this._isConfigured = false;
                return;
            }

            if (string.IsNullOrEmpty(this.config.LLMApiKey))
            {
                this._logger.LogWarning("API key is not configured for Claude service");
                this._isConfigured = false;
                return;
            }

            this.httpClient = new HttpClient
            {
                BaseAddress = new Uri(this.config.BaseUrl),
                DefaultRequestHeaders =
                {
                    { "x-api-key", this.config.LLMApiKey },
                    { "anthropic-version", "2023-06-01" },
                },
            };

            this.rateLimiter = new RateLimiter();
            this._isConfigured = true;
        }

        /// <summary>
        /// Gets the current LLM configuration.
        /// </summary>
        public LLMConfig Config => this.config;

        /// <summary>
        /// Generates a response from the Claude model based on the provided prompt.
        /// </summary>
        /// <param name="prompt">The input prompt to send to the model.</param>
        /// <returns>A task representing the asynchronous operation, containing the generated response.</returns>
        public async Task<string> GenerateResponseAsync(string prompt)
        {
            if (!this._isConfigured)
            {
                this._logger.LogWarning("Claude service is not properly configured. Returning empty response.");
                return string.Empty;
            }

            return await this.rateLimiter.ExecuteWithRateLimitingAsync(this.config.ModelId ?? "claude", async () =>
            {
                var requestBody = new
                {
                    model = this.config.ModelId,
                    prompt = $"\n\nHuman: {prompt}\n\nAssistant:",
                    max_tokens_to_sample = this.config.MaxTokens,
                    temperature = this.config.Temperature,
                };

                var response = await this.httpClient.PostAsync(
                    "v1/complete",
                    new StringContent(
                        JsonSerializer.Serialize(requestBody),
                        Encoding.UTF8,
                        "application/json"));

                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseBody);
                return doc.RootElement.GetProperty("completion").GetString() ?? string.Empty;
            });
        }

        /// <summary>
        /// Analyzes the intent of user input using the Claude model.
        /// </summary>
        /// <param name="userInput">The user input to analyze.</param>
        /// <returns>A task representing the asynchronous operation, containing the analyzed intent.</returns>
        public async Task<string> AnalyzeIntentAsync(string userInput)
        {
            if (!this._isConfigured)
            {
                this._logger.LogWarning("Claude service is not properly configured. Returning empty response.");
                return string.Empty;
            }

            return await this.rateLimiter.ExecuteWithRateLimitingAsync(this.config.ModelId ?? "claude", async () =>
            {
                var prompt = $"Analyze the intent of this user input and provide a concise description: {userInput}";
                return await this.GenerateResponseAsync(prompt);
            });
        }

        /// <summary>
        /// Validates if a given task is safe and appropriate to execute.
        /// </summary>
        /// <param name="task">The task description to validate.</param>
        /// <returns>A task representing the asynchronous operation, containing true if the task is valid, false otherwise.</returns>
        public async Task<bool> ValidateTaskAsync(string task)
        {
            if (!this._isConfigured)
            {
                this._logger.LogWarning("Claude service is not properly configured. Returning false.");
                return false;
            }

            return await this.rateLimiter.ExecuteWithRateLimitingAsync(this.config.ModelId ?? "claude", async () =>
            {
                var prompt = $"Can you execute this task? Answer only with 'true' or 'false': {task}";
                var response = await this.GenerateResponseAsync(prompt);
                return bool.TryParse(response.Trim().ToLower(), out bool result) && result;
            });
        }

        /// <summary>
        /// Analyzes a task into specific executable tasks.
        /// </summary>
        /// <param name="command">The command to analyze.</param>
        /// <returns>A task representing the asynchronous operation, containing the analyzed tasks.</returns>
        public async Task<IEnumerable<string>> AnalyzeTaskAsync(string command)
        {
            if (!this._isConfigured)
            {
                this._logger.LogWarning("Claude service is not properly configured. Returning empty task list.");
                return Array.Empty<string>();
            }

            return await this.rateLimiter.ExecuteWithRateLimitingAsync(this.config.ModelId ?? "claude", async () =>
            {
                var prompt = @$"Break down this command into specific executable tasks. Format each task as a separate line:
Command: {command}
Tasks:";
                var response = await this.GenerateResponseAsync(prompt);
                return response.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            });
        }
    }
}
