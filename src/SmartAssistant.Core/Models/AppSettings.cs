// <copyright file="AppSettings.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Configuration settings for the SmartAssistant application.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Gets or sets the current selected model.
        /// </summary>
        public string CurrentModel { get; set; } = "OpenAI_GPT4";

        /// <summary>
        /// Gets or sets the API key for the application.
        /// </summary>
        public string? ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the path to the Python executable.
        /// </summary>
        public string? PythonPath { get; set; }

        /// <summary>
        /// Gets or sets the base path for the SmartAssistant.
        /// </summary>
        public string? BasePath { get; set; }

        /// <summary>
        /// Gets or sets the path to web drivers.
        /// </summary>
        public string? DriverPath { get; set; }

        /// <summary>
        /// Gets or sets the configurations for different language models.
        /// </summary>
        public Dictionary<string, LLMConfig>? ModelConfigs { get; set; }
    }
}
