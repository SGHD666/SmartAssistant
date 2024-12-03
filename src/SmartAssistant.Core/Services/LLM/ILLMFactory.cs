// <copyright file="ILLMFactory.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core.Services.LLM
{
    using SmartAssistant.Core.Models;

    /// <summary>
    /// Factory interface for creating language model service instances.
    /// </summary>
    public interface ILLMFactory
    {
        /// <summary>
        /// Creates a language model service instance of the specified type.
        /// </summary>
        /// <param name="type">The type of language model service to create.</param>
        /// <returns>An instance of <see cref="ILanguageModelService"/> for the specified model type.</returns>
        ILanguageModelService CreateService(LLMType type);
    }
}
