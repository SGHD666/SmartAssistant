// <copyright file="AssistantController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using SmartAssistant.Core.Services;
    using SmartAssistant.Core.Services.LLM;

    /// <summary>
    /// Controller class for handling user interactions with the smart assistant.
    /// Coordinates between language model services and task execution.
    /// </summary>
    public class AssistantController
    {
        private readonly ILanguageModelService _languageModel;
        private readonly ITaskExecutionService _taskExecutor;
        private readonly ILogger<AssistantController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssistantController"/> class.
        /// </summary>
        /// <param name="languageModel">The language model service for processing user input.</param>
        /// <param name="taskExecutor">The task execution service for performing actions.</param>
        /// <param name="logger">The logger for recording controller operations.</param>
        public AssistantController(
            ILanguageModelService languageModel,
            ITaskExecutionService taskExecutor,
            ILogger<AssistantController> logger)
        {
            this._languageModel = languageModel ?? throw new ArgumentNullException(nameof(languageModel));
            this._taskExecutor = taskExecutor ?? throw new ArgumentNullException(nameof(taskExecutor));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Processes user input and executes appropriate actions.
        /// </summary>
        /// <param name="userInput">The user input to process.</param>
        /// <returns>A task representing the asynchronous operation, containing the response message.</returns>
        public async Task<string> ProcessUserInputAsync(string userInput)
        {
            try
            {
                this._logger.LogDebug("Processing user input: {Input}", userInput);

                // 如果是命令格式，直接执行
                if (userInput.StartsWith("COMMAND:", StringComparison.OrdinalIgnoreCase))
                {
                    this._logger.LogDebug("Executing command: {Command}", userInput[8..].Trim());
                    var commandSuccess = await this._taskExecutor.ExecuteTaskAsync(userInput);
                    var commandResult = commandSuccess ? "命令已成功执行。" : "执行命令时出现错误。";
                    this._logger.LogDebug("Command execution result: {Result}", commandResult);
                    return commandResult;
                }

                // 先尝试获取直接对话响应
                var response = await this._languageModel.GenerateResponseAsync(userInput);
                if (!string.IsNullOrEmpty(response))
                {
                    this._logger.LogDebug("Got direct response from language model");
                    return response;
                }

                // 如果没有直接响应，再尝试执行任务
                this._logger.LogDebug("No direct response, trying to execute task");
                var intent = await this._languageModel.AnalyzeIntentAsync(userInput);
                this._logger.LogInformation("Analyzed intent: {Intent}", intent);

                var isValid = await this._languageModel.ValidateTaskAsync(intent);
                this._logger.LogDebug("Task validation result: {IsValid}", isValid);
                if (!isValid)
                {
                    return "抱歉，我无法执行这个任务。";
                }

                var taskSuccess = await this._taskExecutor.ExecuteTaskAsync(intent);
                var result = taskSuccess ? "任务已成功执行。" : "执行任务时出现错误。";
                this._logger.LogDebug("Task execution result: {Result}", result);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error processing user input: {Input}", userInput);
                return $"处理请求时出现错误: {ex.Message}";
            }
        }

        /// <summary>
        /// Gets a response from the language model for the given user input.
        /// </summary>
        /// <param name="userInput">The user's input message.</param>
        /// <returns>The assistant's response.</returns>
        public async Task<string> GetResponseAsync(string userInput)
        {
            try
            {
                _logger.LogDebug("Getting response for user input: {Input}", userInput);
                var response = await _languageModel.GenerateResponseAsync(userInput);
                _logger.LogDebug("Generated response: {Response}", response);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting response for user input: {Input}", userInput);
                throw;
            }
        }
    }
}
