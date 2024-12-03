// <copyright file="OpenAIService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core.Services.LLM
{
    using System;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using OpenAI_API;
    using SmartAssistant.Core.Models;
    using System.Collections.Generic;

    /// <summary>
    /// Service for interacting with OpenAI's language models.
    /// Implements the ILanguageModelService interface to provide GPT-3.5 and GPT-4 capabilities.
    /// </summary>
    public class OpenAIService : ILanguageModelService
    {
        private readonly ILogger<OpenAIService> _logger;
        private readonly OpenAIServiceConfig _config;
        private readonly OpenAIAPI _api;
        private readonly RateLimiter _rateLimiter;

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
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
            _api = new OpenAIAPI(new APIAuthentication(_config.ApiKey));
            _rateLimiter = new RateLimiter();
        }

        /// <summary>
        /// Gets the current LLM configuration.
        /// </summary>
        public LLMConfig Config => new LLMConfig { ModelId = _config.ModelId, LLMApiKey = _config.ApiKey };

        private RateLimitExceededException HandleRateLimit(Exception ex)
        {
            _logger.LogWarning(ex, "OpenAI rate limit exceeded");
            
            // Extract wait time from error message, default to 1 hour
            var retryAfter = TimeSpan.FromHours(1);

            // Try to extract specific wait time from error message
            if (ex.Message.Contains("try again in", StringComparison.OrdinalIgnoreCase))
            {
                var match = Regex.Match(
                    ex.Message, 
                    @"try again in (?:about )?(\d+) (\w+)",
                    RegexOptions.IgnoreCase);
                
                if (match.Success)
                {
                    var value = int.Parse(match.Groups[1].Value);
                    var unit = match.Groups[2].Value.ToLower();
                    
                    retryAfter = unit switch
                    {
                        "minute" or "minutes" => TimeSpan.FromMinutes(value),
                        "hour" or "hours" => TimeSpan.FromHours(value),
                        "second" or "seconds" => TimeSpan.FromSeconds(value),
                        _ => TimeSpan.FromHours(1)
                    };

                    _logger.LogInformation("Extracted retry time: {RetryTime} {Unit}", value, unit);
                }
            }

            // Update rate limiter counter
            var modelId = _config.ModelId ?? "gpt-3.5-turbo";
            _rateLimiter.SetModelLimit(modelId, 1); // Temporarily set limit to 1 to force wait
            _logger.LogInformation("Set temporary rate limit of 1 request for model {ModelId}", modelId);
            
            return new RateLimitExceededException(
                $"OpenAI rate limit exceeded. Please try again in {retryAfter.TotalMinutes:F0} minutes.", 
                retryAfter,
                ex);
        }

        /// <summary>
        /// Generates a response from the OpenAI model based on the provided prompt.
        /// </summary>
        /// <param name="prompt">The input prompt to send to the model.</param>
        /// <returns>A task representing the asynchronous operation, containing the generated response.</returns>
        public async Task<string> GenerateResponseAsync(string prompt)
        {
            var modelId = _config.ModelId ?? "gpt-3.5-turbo";
            _logger.LogInformation("Generating response using model {ModelId}", modelId);

            try
            {
                return await _rateLimiter.ExecuteWithRateLimitingAsync(modelId, async () =>
                {
                    var chat = _api.Chat.CreateConversation();
                    chat.AppendUserInput(prompt);
                    
                    string response = await chat.GetResponseFromChatbotAsync();
                    _logger.LogDebug("Successfully generated response");
                    return response;
                });
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("rate limit", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning(ex, "Rate limit exceeded via HttpRequestException");
                throw HandleRateLimit(ex);
            }
            catch (Exception ex) when (ex.Message.Contains("rate limit", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning(ex, "Rate limit exceeded via general exception");
                throw HandleRateLimit(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating response");
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
            return await _rateLimiter.ExecuteWithRateLimitingAsync(_config.ModelId, async () =>
            {
                try
                {
                    var chat = _api.Chat.CreateConversation();
                    chat.AppendSystemMessage("Analyze the user's intent from their input. Provide a brief description of what they want to do.");
                    chat.AppendUserInput(userInput);
                    return await chat.GetResponseFromChatbotAsync();
                }
                catch (Exception ex) when (ex.Message.Contains("rate limit"))
                {
                    throw HandleRateLimit(ex);
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
            return await _rateLimiter.ExecuteWithRateLimitingAsync(_config.ModelId, async () =>
            {
                try
                {
                    var chat = _api.Chat.CreateConversation();
                    chat.AppendSystemMessage("Validate if the given task is safe and appropriate to execute. Respond with 'true' or 'false'.");
                    chat.AppendUserInput(task);
                    var response = await chat.GetResponseFromChatbotAsync();
                    return bool.TryParse(response.Trim().ToLower(), out bool result) && result;
                }
                catch (Exception ex) when (ex.Message.Contains("rate limit"))
                {
                    throw HandleRateLimit(ex);
                }
            });
        }

        public Task<IEnumerable<string>> AnalyzeTaskAsync(string command)
        {
            throw new NotImplementedException();
        }
    }
}
