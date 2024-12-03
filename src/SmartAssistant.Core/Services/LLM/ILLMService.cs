// <copyright file="ILLMService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core.Services.LLM
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for interacting with Language Learning Models (LLMs).
    /// </summary>
    public interface ILLMService
    {
        /// <summary>
        /// Generates a response from the language model based on the provided prompt.
        /// </summary>
        /// <param name="prompt">The input prompt to send to the language model.</param>
        /// <returns>A task representing the asynchronous operation, containing the generated response.</returns>
        Task<string> GenerateResponseAsync(string prompt);
    }
}
