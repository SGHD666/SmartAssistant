// <copyright file="OpenAIService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core.Services.LLM
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using OpenAI_API;
    using SmartAssistant.Core.Models;

    /// <summary>
    /// Service for interacting with OpenAI's language models.
    /// Implements the ILanguageModelService interface to provide GPT-3.5 and GPT-4 capabilities.
    /// </summary>
    public partial class OpenAIService : ILanguageModelService
    {
        private readonly ILogger<OpenAIService> logger;
        private readonly OpenAIServiceConfig config;
        private readonly OpenAIAPI api;
        private readonly RateLimiter rateLimiter;
        private bool isInitialized = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAIService"/> class.
        /// </summary>
        /// <param name="config">The OpenAI service configuration.</param>
        /// <param name="logger">The logger instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when config or logger is null.</exception>
        public OpenAIService(
            IOptions<OpenAIServiceConfig> config,
            ILogger<OpenAIService> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.config = config?.Value ?? throw new ArgumentNullException(nameof(config));
            this.api = new OpenAIAPI(new APIAuthentication(this.config.ApiKey));
            this.rateLimiter = new RateLimiter();
        }

        /// <summary>
        /// Gets the current LLM configuration.
        /// </summary>
        public LLMConfig Config => new() { ModelId = this.config.ModelId, LLMApiKey = this.config.ApiKey };

        /// <summary>
        /// Generates a response from the OpenAI model based on the provided prompt.
        /// </summary>
        /// <param name="prompt">The input prompt to send to the model.</param>
        /// <returns>A task representing the asynchronous operation, containing the generated response.</returns>
        public async Task<string> GenerateResponseAsync(string prompt)
        {
            var modelId = this.config.ModelId ?? "gpt-3.5-turbo";
            this.logger.LogInformation("Generating response using model {ModelId}", modelId);

            try
            {
                return await this.rateLimiter.ExecuteWithRateLimitingAsync(modelId, async () =>
                {
                    if (!this.isInitialized)
                    {
                        this.Initialize();
                    }

                    var chat = this.api.Chat.CreateConversation();
                    chat.AppendUserInput(prompt);

                    string response = await chat.GetResponseFromChatbotAsync();
                    this.logger.LogDebug("Successfully generated response");
                    return response;
                });
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("rate limit", StringComparison.OrdinalIgnoreCase))
            {
                this.logger.LogWarning(ex, "Rate limit exceeded via HttpRequestException");
                throw this.HandleRateLimit(ex);
            }
            catch (Exception ex) when (ex.Message.Contains("rate limit", StringComparison.OrdinalIgnoreCase))
            {
                this.logger.LogWarning(ex, "Rate limit exceeded via general exception");
                throw this.HandleRateLimit(ex);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error generating response");
                throw;
            }
        }

        /// <summary>
        /// Analyzes the intent of user input using the OpenAI model.
        /// </summary>
        /// <param name="userInput">The user input to analyze.</param>
        /// <returns>A task representing the asynchronous operation, containing the analyzed intent.</returns>
        public async Task<string> AnalyzeIntentAsync(string userInput)
        {
            return await this.rateLimiter.ExecuteWithRateLimitingAsync(this.config.ModelId!, async () =>
            {
                try
                {
                    if (!this.isInitialized)
                    {
                        this.Initialize();
                    }

                    var chat = this.api.Chat.CreateConversation();
                    chat.AppendSystemMessage("Analyze the user's intent from their input. Provide a brief description of what they want to do.");
                    chat.AppendUserInput(userInput);
                    return await chat.GetResponseFromChatbotAsync();
                }
                catch (Exception ex) when (ex.Message.Contains("rate limit"))
                {
                    throw this.HandleRateLimit(ex);
                }
            });
        }

        /// <summary>
        /// Validates if a given task is safe and appropriate to execute.
        /// </summary>
        /// <param name="task">The task description to validate.</param>
        /// <returns>A task representing the asynchronous operation, containing true if the task is valid, false otherwise.</returns>
        public async Task<bool> ValidateTaskAsync(string task)
        {
            return await this.rateLimiter.ExecuteWithRateLimitingAsync(this.config.ModelId!, async () =>
            {
                try
                {
                    if (!this.isInitialized)
                    {
                        this.Initialize();
                    }

                    var chat = this.api.Chat.CreateConversation();
                    chat.AppendSystemMessage("Validate if the given task is safe and appropriate to execute. Respond with 'true' or 'false'.");
                    chat.AppendUserInput(task);
                    var response = await chat.GetResponseFromChatbotAsync();
                    return bool.TryParse(response.Trim().ToLower(), out bool result) && result;
                }
                catch (Exception ex) when (ex.Message.Contains("rate limit"))
                {
                    throw this.HandleRateLimit(ex);
                }
            });
        }

        /// <summary>
        /// Analyzes the given command to determine required tasks.
        /// </summary>
        /// <param name="command">The command to analyze.</param>
        /// <returns>A collection of identified tasks from the command.</returns>
        /// <inheritdoc/>
        public Task<IEnumerable<string>> AnalyzeTaskAsync(string command)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles rate limit exceptions from the OpenAI API.
        /// </summary>
        /// <param name="ex">The exception that caused the rate limit.</param>
        /// <returns>A new <see cref="RateLimitExceededException"/> with appropriate retry time.</returns>
        private RateLimitExceededException HandleRateLimit(Exception ex)
        {
            this.logger.LogWarning(ex, "OpenAI rate limit exceeded");

            // Extract wait time from error message, default to 1 hour
            var retryAfter = TimeSpan.FromHours(1);

            // Try to extract specific wait time from error message
            if (ex.Message.Contains("try again in", StringComparison.OrdinalIgnoreCase))
            {
                var match = this.MyRegex().Match(ex.Message);

                if (match.Success)
                {
                    var value = int.Parse(match.Groups[1].Value);
                    var unit = match.Groups[2].Value.ToLower();

                    retryAfter = unit switch
                    {
                        "minute" or "minutes" => TimeSpan.FromMinutes(value),
                        "hour" or "hours" => TimeSpan.FromHours(value),
                        "second" or "seconds" => TimeSpan.FromSeconds(value),
                        _ => TimeSpan.FromHours(1),
                    };

                    this.logger.LogInformation("Extracted retry time: {RetryTime} {Unit}", value, unit);
                }
            }

            // Update rate limiter counter
            var modelId = this.config.ModelId ?? "gpt-3.5-turbo";
            this.rateLimiter.SetModelLimit(modelId, 1); // Temporarily set limit to 1 to force wait
            this.logger.LogInformation("Set temporary rate limit of 1 request for model {ModelId}", modelId);

            return new RateLimitExceededException(
                $"OpenAI rate limit exceeded. Please try again in {retryAfter.TotalMinutes:F0} minutes.",
                retryAfter,
                ex);
        }

        private void Initialize()
        {
            this.isInitialized = true;
        }

        /// <summary>
        /// Generates a regex pattern for extracting retry time from OpenAI rate limit messages.
        /// </summary>
        /// <returns>A compiled regex pattern for matching retry time information.</returns>
        [GeneratedRegex(@"try again in (?:about )?(\d+) (\w+)", RegexOptions.IgnoreCase, "zh-CN")]
        private partial Regex MyRegex();
    }
}
