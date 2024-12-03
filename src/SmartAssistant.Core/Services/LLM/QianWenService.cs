// <copyright file="QianWenService.cs" company="Codeium">
// Copyright (c) Codeium. All rights reserved.
// </copyright>

namespace SmartAssistant.Core.Services.LLM
{
    using System.Collections.Generic;
    using System.Text;
    using System.Text.Json;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using SmartAssistant.Core.Models;

    /// <summary>
    /// Service implementation for interacting with the QianWen language model.
    /// Provides functionality for text generation and language model interactions.
    /// </summary>
    public class QianWenService : ILanguageModelService
    {
        private readonly HttpClient httpClient;
        private readonly LLMConfig config;
        private readonly ILogger<QianWenService> _logger;
        private readonly bool _isConfigured;

        /// <summary>
        /// Initializes a new instance of the <see cref="QianWenService"/> class.
        /// </summary>
        /// <param name="settings">Configuration settings for the QianWen language model service.</param>
        /// <param name="logger">Logger instance for logging messages and events in the QianWen service.</param>
        public QianWenService(IOptions<ModelSettings> settings, ILogger<QianWenService> logger)
        {
            this._logger = logger;
            var modelSettings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            this.config = modelSettings.ModelConfigs["QianWen"];

            if (string.IsNullOrEmpty(this.config.BaseUrl))
            {
                this._logger.LogWarning("BaseUrl is not configured for QianWen service");
                this._isConfigured = false;
                return;
            }

            if (string.IsNullOrEmpty(this.config.LLMApiKey))
            {
                this._logger.LogWarning("API key is not configured for QianWen service");
                this._isConfigured = false;
                return;
            }

            this.httpClient = new HttpClient
            {
                BaseAddress = new Uri(this.config.BaseUrl),
            };
            this.httpClient.DefaultRequestHeaders.Add("Authorization", this.config.LLMApiKey);
            this._isConfigured = true;
        }

        public LLMConfig Config => this.config;

        public async Task<string> GenerateResponseAsync(string prompt)
        {
            if (!this._isConfigured)
            {
                this._logger.LogWarning("QianWen service is not properly configured. Returning empty response.");
                return string.Empty;
            }

            try
            {
                var requestBody = new
                {
                    model = this.config.ModelId,
                    prompt = prompt,
                    max_tokens = this.config.MaxTokens,
                    temperature = this.config.Temperature,
                };

                var response = await this.httpClient.PostAsync(
                    "v1/chat/completions",
                    new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json"));

                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseBody);
                return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error generating response from QianWen API");
                return string.Empty;
            }
        }

        public async Task<string> AnalyzeIntentAsync(string userInput)
        {
            if (!this._isConfigured)
            {
                this._logger.LogWarning("QianWen service is not properly configured. Returning empty response.");
                return string.Empty;
            }

            try
            {
                var prompt = $"分析用户输入的意图并提供简短描述: {userInput}";
                return await this.GenerateResponseAsync(prompt);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error analyzing intent");
                return string.Empty;
            }
        }

        public async Task<bool> ValidateTaskAsync(string task)
        {
            if (!this._isConfigured)
            {
                this._logger.LogWarning("QianWen service is not properly configured. Returning false.");
                return false;
            }

            try
            {
                var prompt = $"这个任务是否可以执行？只需回答'true'或'false': {task}";
                var response = await this.GenerateResponseAsync(prompt);
                return bool.TryParse(response.Trim().ToLower(), out bool result) && result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error validating task");
                return false;
            }
        }

        public async Task<IEnumerable<string>> AnalyzeTaskAsync(string command)
        {
            if (!this._isConfigured)
            {
                this._logger.LogWarning("QianWen service is not properly configured. Returning empty task list.");
                return Array.Empty<string>();
            }

            try
            {
                var prompt = @$"将这个命令分解成具体的可执行任务。每个任务单独一行输出:
命令: {command}
任务:";
                var response = await this.GenerateResponseAsync(prompt);
                return response.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error analyzing task");
                return Array.Empty<string>();
            }
        }
    }
}
