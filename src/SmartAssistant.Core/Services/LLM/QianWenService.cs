// <copyright file="QianWenService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
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
        private readonly HttpClient? httpClient;
        private readonly ILogger<QianWenService>? logger;
        private readonly bool isConfigured;

        /// <summary>
        /// Initializes a new instance of the <see cref="QianWenService"/> class.
        /// </summary>
        /// <param name="settings">Configuration settings for the QianWen language model service.</param>
        /// <param name="logger">Logger instance for logging messages and events in the QianWen service.</param>
        public QianWenService(IOptions<ModelSettings> settings, ILogger<QianWenService> logger)
        {
            this.logger = logger;
            var modelSettings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            this.Config = modelSettings.ModelConfigs!["QianWen"];

            if (string.IsNullOrEmpty(this.Config.BaseUrl))
            {
                this.logger.LogWarning("BaseUrl is not configured for QianWen service");
                this.isConfigured = false;
                return;
            }

            if (string.IsNullOrEmpty(this.Config.LLMApiKey))
            {
                this.logger.LogWarning("API key is not configured for QianWen service");
                this.isConfigured = false;
                return;
            }

            this.httpClient = new HttpClient
            {
                BaseAddress = new Uri(this.Config.BaseUrl),
            };
            this.httpClient.DefaultRequestHeaders.Add("Authorization", this.Config.LLMApiKey);
            this.isConfigured = true;
        }

        /// <inheritdoc/>
        public LLMConfig Config { get; }

        /// <inheritdoc/>
        public async Task<string> GenerateResponseAsync(string prompt)
        {
            if (!this.isConfigured)
            {
                this.logger!.LogWarning("QianWen service is not properly configured. Returning empty response.");
                return string.Empty;
            }

            try
            {
                var requestBody = new
                {
                    model = this.Config.ModelId,
                    prompt,
                    max_tokens = this.Config.MaxTokens,
                    temperature = this.Config.Temperature,
                };

                var response = await this.httpClient!.PostAsync(
                    "v1/chat/completions",
                    new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json"));

                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseBody);
                return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                this.logger!.LogError(ex, "Error generating response from QianWen API");
                return string.Empty;
            }
        }

        /// <inheritdoc/>
        public async Task<string> AnalyzeIntentAsync(string userInput)
        {
            if (!this.isConfigured)
            {
                this.logger!.LogWarning("QianWen service is not properly configured. Returning empty response.");
                return string.Empty;
            }

            try
            {
                var prompt = $"分析用户输入的意图并提供简短描述: {userInput}";
                return await this.GenerateResponseAsync(prompt);
            }
            catch (Exception ex)
            {
                this.logger!.LogError(ex, "Error analyzing intent");
                return string.Empty;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ValidateTaskAsync(string task)
        {
            if (!this.isConfigured)
            {
                this.logger!.LogWarning("QianWen service is not properly configured. Returning false.");
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
                this.logger!.LogError(ex, "Error validating task");
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> AnalyzeTaskAsync(string command)
        {
            if (!this.isConfigured)
            {
                this.logger!.LogWarning("QianWen service is not properly configured. Returning empty task list.");
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
                this.logger!.LogError(ex, "Error analyzing task");
                return Array.Empty<string>();
            }
        }
    }
}
