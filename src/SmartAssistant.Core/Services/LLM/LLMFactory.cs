// <copyright file="LLMFactory.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core.Services.LLM
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;
    using SmartAssistant.Core.Models;

    /// <summary>
    /// Factory class for creating language model service instances.
    /// Implements the ILLMFactory interface to provide service creation functionality.
    /// </summary>
    public class LLMFactory : ILLMFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEnumerable<ILanguageModelService> _languageModelServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="LLMFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider for dependency injection.</param>
        /// <param name="languageModelServices">The collection of available language model services.</param>
        public LLMFactory(IServiceProvider serviceProvider, IEnumerable<ILanguageModelService> languageModelServices)
        {
            this._serviceProvider = serviceProvider;
            this._languageModelServices = languageModelServices;
        }

        /// <summary>
        /// Creates a language model service instance of the specified type.
        /// </summary>
        /// <param name="type">The type of language model service to create.</param>
        /// <returns>An instance of <see cref="ILanguageModelService"/> for the specified model type.</returns>
        /// <exception cref="ArgumentException">Thrown when no service is found for the specified type.</exception>
        public ILanguageModelService CreateService(LLMType type)
        {
            try
            {
                Console.WriteLine($"Creating service for type: {type}");

                // 根据类型创建对应的服务
                ILanguageModelService? service = type switch
                {
                    LLMType.OpenAI_GPT35 or LLMType.OpenAI_GPT4 
                        => ActivatorUtilities.CreateInstance<OpenAIService>(_serviceProvider),
                    LLMType.Claude 
                        => ActivatorUtilities.CreateInstance<ClaudeService>(_serviceProvider),
                    LLMType.QianWen 
                        => ActivatorUtilities.CreateInstance<QianWenService>(_serviceProvider),
                    _ => throw new ArgumentException($"Unsupported model type: {type}")
                };

                if (service == null)
                {
                    throw new ArgumentException($"Failed to create service for model type: {type}");
                }

                Console.WriteLine($"Successfully created service for type: {type}");
                return service;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create service for type {type}: {ex.Message}");
                throw;
            }
        }
    }
}
