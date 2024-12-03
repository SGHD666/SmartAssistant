// <copyright file="ILanguageModelService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core.Services.LLM
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SmartAssistant.Core.Models;

    /// <summary>
    /// Interface defining the contract for language model services.
    /// Provides methods for generating responses, analyzing intents, and validating tasks.
    /// </summary>
    public interface ILanguageModelService
    {
        /// <summary>
        /// Gets the configuration settings for the language model.
        /// </summary>
        LLMConfig Config { get; }

        /// <summary>
        /// Generates a response based on the provided prompt.
        /// </summary>
        /// <param name="prompt">The input prompt to generate a response for.</param>
        /// <returns>A task that represents the asynchronous operation, containing the generated response.</returns>
        Task<string> GenerateResponseAsync(string prompt);

        /// <summary>
        /// Analyzes the intent behind the provided user input.
        /// </summary>
        /// <param name="userInput">The user input to analyze.</param>
        /// <returns>A task that represents the asynchronous operation, containing the analyzed intent.</returns>
        Task<string> AnalyzeIntentAsync(string userInput);

        /// <summary>
        /// Analyzes a user command and breaks it down into executable subtasks.
        /// </summary>
        /// <param name="command">The command to analyze.</param>
        /// <returns>A list of subtask descriptions that can be executed by the automation service.</returns>
        Task<IEnumerable<string>> AnalyzeTaskAsync(string command);

        /// <summary>
        /// Validates whether a given task can be executed by the language model.
        /// </summary>
        /// <param name="task">The task to validate.</param>
        /// <returns>A task that represents the asynchronous operation, containing a boolean indicating whether the task is valid.</returns>
        Task<bool> ValidateTaskAsync(string task);
    }
}
