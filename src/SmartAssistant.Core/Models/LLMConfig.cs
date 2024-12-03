// <copyright file="ModelSettings.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Configuration for a specific language model.
    /// </summary>
    public class LLMConfig
    {
        /// <summary>
        /// Gets or sets LLM Type.
        /// </summary>
        public LLMType Type { get; set; }

        /// <summary>
        /// Gets or sets the API key for the language model.
        /// </summary>
        public string? LLMApiKey { get; set; }

        /// <summary>
        /// Gets or sets the model identifier.
        /// </summary>
        public string? ModelId { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of tokens for the model.
        /// </summary>
        public int MaxTokens { get; set; }

        /// <summary>
        /// Gets or sets the temperature for response generation.
        /// </summary>
        public float Temperature { get; set; }

        /// <summary>
        /// Gets or sets the base URL for API requests.
        /// </summary>
        public string? BaseUrl { get; set; }
    }
}
