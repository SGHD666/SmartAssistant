// <copyright file="ModelManager.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core.Services.LLM
{
    using System;
    using System.Threading.Tasks;
    using Polly;
    using Polly.Retry;
    using SmartAssistant.Core.Models;

    /// <summary>
    /// Manages language model services and handles model switching and response generation.
    /// </summary>
    public class ModelManager
    {
        /// <summary>
        /// Lock object for thread-safe model switching.
        /// </summary>
        private readonly object switchLock = new();

        /// <summary>
        /// Factory for creating language model service instances.
        /// </summary>
        private readonly ILLMFactory factory;

        /// <summary>
        /// Settings for configuring language models.
        /// </summary>
        private readonly ModelSettings modelSettings;

        /// <summary>
        /// Retry policy for rate limit handling.
        /// </summary>
        private readonly AsyncRetryPolicy<string> retryPolicy;

        /// <summary>
        /// Currently active language model service.
        /// </summary>
        private volatile ILanguageModelService? currentService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelManager"/> class.
        /// </summary>
        /// <param name="modelSettings">The settings for configuring language models.</param>
        /// <param name="factory">The factory for creating language model service instances.</param>
        /// <exception cref="ArgumentNullException">Thrown when factory or modelSettings is null.</exception>
        public ModelManager(
            ModelSettings modelSettings,
            ILLMFactory factory)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.modelSettings = modelSettings ?? throw new ArgumentNullException(nameof(modelSettings));
            Console.WriteLine($"ModelManager initialized with settings:");
            Console.WriteLine($"  CurrentModel: {this.modelSettings.CurrentModel}");
            Console.WriteLine($"  ModelConfigs: {(this.modelSettings.ModelConfigs == null ? "null" : $"{this.modelSettings.ModelConfigs.Count} items")}");

            if (this.modelSettings.ModelConfigs != null)
            {
                Console.WriteLine("Available models:");
                foreach (var (key, config) in this.modelSettings.ModelConfigs)
                {
                    Console.WriteLine($"  {key}: Type={config.Type}");
                }
            }

            // Configure retry policy for rate limits
            this.retryPolicy = Policy<string>
                .Handle<HttpRequestException>(ex => ex.Message.Contains("rate limit"))
                .Or<Exception>(ex => ex.Message.Contains("rate limit"))
                .WaitAndRetryAsync(
                    3, // Number of retries
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Exponential backoff
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"Rate limit hit - Waiting {timeSpan} before retry. Attempt {retryCount}");
                    });

            this.ValidateConfiguration();
            this.InitializeService();
        }

        /// <summary>
        /// Gets the current model settings.
        /// </summary>
        public ModelSettings Settings => this.modelSettings;

        /// <summary>
        /// Gets the current model type.
        /// </summary>
        public LLMType CurrentModel => this.modelSettings.CurrentModel;

        /// <summary>
        /// Gets the current language model service.
        /// </summary>
        /// <returns>The current language model service.</returns>
        public ILanguageModelService GetService()
        {
            this.EnsureServiceInitialized();
            return this.currentService!;
        }

        /// <summary>
        /// Generates a response to the given prompt using the current language model.
        /// </summary>
        /// <param name="prompt">The prompt to generate a response for.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">Thrown when prompt is empty.</exception>
        public async Task<string> GenerateResponseAsync(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                throw new ArgumentException("Prompt cannot be empty", nameof(prompt));
            }

            this.EnsureServiceInitialized();

            return await this.retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var service = this.currentService;
                    return service == null
                        ? throw new InvalidOperationException("Language model service is not initialized")
                        : await service.GenerateResponseAsync(prompt);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error generating response: {ex.Message}", ex);
                }
            });
        }

        /// <summary>
        /// Switches to the specified language model.
        /// </summary>
        /// <param name="modelType">The type of the language model to switch to.</param>
        /// <exception cref="ArgumentException">Thrown when configuration not found for model type.</exception>
        public void SwitchModel(LLMType modelType)
        {
            try
            {
                Console.WriteLine($"Switching to model: {modelType}");
                Console.WriteLine($"Available configs: {string.Join(", ", this.modelSettings.ModelConfigs!.Select(c => $"{c.Key}:{c.Value.Type}"))}");
                var config = this.modelSettings.ModelConfigs!.Values
                    .FirstOrDefault(c => c.Type == modelType);

                if (config == null)
                {
                    throw new ArgumentException($"Configuration not found for model type {modelType}");
                }

                Console.WriteLine($"Found config: Type={config.Type}, Model={config.ModelId}");
                lock (this.switchLock)
                {
                    this.currentService = this.factory.CreateService(modelType);
                    if (this.currentService == null)
                    {
                        throw new InvalidOperationException($"Failed to create service for model type {modelType}");
                    }
                    this.modelSettings.CurrentModel = modelType;
                }

                Console.WriteLine($"Successfully switched to model: {modelType}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to switch to model {modelType}: {ex.Message}");
                throw new InvalidOperationException($"Failed to switch to model {modelType}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Updates the model settings and reinitializes the service.
        /// </summary>
        /// <param name="newSettings">The new model settings to apply.</param>
        public void UpdateSettings(ModelSettings newSettings)
        {
            ArgumentNullException.ThrowIfNull(newSettings);

            lock (this.switchLock)
            {
                // Update the model settings
                this.modelSettings.ApiKey = newSettings.ApiKey;
                this.modelSettings.CurrentModel = newSettings.CurrentModel;
                this.modelSettings.ModelConfigs = newSettings.ModelConfigs;
                this.modelSettings.PythonPath = newSettings.PythonPath;
                this.modelSettings.BasePath = newSettings.BasePath;

                // Reinitialize the service with the new settings
                this.currentService = this.factory.CreateService(this.modelSettings.CurrentModel);
            }
        }

        /// <summary>
        /// Initializes the language model service.
        /// </summary>
        private void InitializeService()
        {
            try
            {
                Console.WriteLine($"Initializing service for model type: {this.modelSettings.CurrentModel}");
                var config = this.modelSettings.ModelConfigs!.Values
                    .FirstOrDefault(c => c.Type == this.modelSettings.CurrentModel);

                if (config == null)
                {
                    throw new InvalidOperationException(
                        $"Configuration not found for model type {this.modelSettings.CurrentModel}. " +
                        $"Available types: {string.Join(", ", this.modelSettings.ModelConfigs.Values.Select(c => c.Type))}");
                }

                Console.WriteLine($"Creating service with config: Type={config.Type}, Model={config.ModelId}");
                this.currentService = this.factory.CreateService(this.modelSettings.CurrentModel);

                if (this.currentService == null)
                {
                    throw new InvalidOperationException(
                        $"Failed to create service for model type {this.modelSettings.CurrentModel}");
                }

                Console.WriteLine("Service initialized successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize service: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Ensures the language model service is initialized.
        /// </summary>
        private void EnsureServiceInitialized()
        {
            if (this.currentService == null)
            {
                lock (this.switchLock)
                {
                    if (this.currentService == null)
                    {
                        this.InitializeService();
                    }
                }
            }
        }

        /// <summary>
        /// Validates the language model configuration.
        /// </summary>
        private void ValidateConfiguration()
        {
            try
            {
                Console.WriteLine("\nValidating configuration...");

                if (this.modelSettings.ModelConfigs == null)
                {
                    throw new InvalidOperationException("ModelConfigs is null");
                }

                if (this.modelSettings.ModelConfigs.Count == 0)
                {
                    throw new InvalidOperationException("ModelConfigs is empty");
                }

                var currentModelExists = this.modelSettings.ModelConfigs.Values
                    .Any(c => c.Type == this.modelSettings.CurrentModel);

                if (!currentModelExists)
                {
                    var availableTypes = string.Join(
                        ", ",
                        this.modelSettings.ModelConfigs.Values.Select(c => c.Type.ToString()));
                    throw new InvalidOperationException(
                        $"Current model {this.modelSettings.CurrentModel} not found in configuration. " +
                        $"Available types: {availableTypes}");
                }

                Console.WriteLine("Configuration validation successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Configuration validation failed: {ex.Message}");
                throw;
            }
        }
    }
}
