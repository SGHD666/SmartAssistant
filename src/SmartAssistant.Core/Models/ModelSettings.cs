// <copyright file="ModelSettings.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core.Models
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Configuration settings for language models used in the SmartAssistant.
    /// </summary>
    public class ModelSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelSettings"/> class.
        /// </summary>
        [SetsRequiredMembers]
        public ModelSettings()
        {
            this.ModelConfigs = new Dictionary<string, LLMConfig>();
            this.CurrentModel = LLMType.QianWen; // Default model
        }

        /// <summary>
        /// Gets or sets the collection of available language model configurations.
        /// </summary>
        public Dictionary<string, LLMConfig>? ModelConfigs { get; set; }

        /// <summary>
        /// Gets or sets the type of the current language model.
        /// </summary>
        public required LLMType CurrentModel { get; set; }

        /// <summary>
        /// Gets or sets the API key for accessing language model services.
        /// </summary>
        public required string? ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the path to the Python executable.
        /// </summary>
        public required string? PythonPath { get; set; }

        /// <summary>
        /// Gets or sets the base path for the language model.
        /// </summary>
        public required string? BasePath { get; set; }
    }
}
