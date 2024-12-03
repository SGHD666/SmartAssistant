namespace SmartAssistant.Core.Models
{
    /// <summary>
    /// Configuration settings for the OpenAI service.
    /// </summary>
    public class OpenAIServiceConfig
    {
        /// <summary>
        /// Gets or sets the API key for OpenAI service.
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the model ID (e.g., "gpt-3.5-turbo", "gpt-4").
        /// </summary>
        public string? ModelId { get; set; }

        /// <summary>
        /// Gets or sets the temperature for response generation (0.0 to 1.0).
        /// Higher values make output more random, lower values more deterministic.
        /// </summary>
        public float? Temperature { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of tokens to generate.
        /// </summary>
        public int? MaxTokens { get; set; }
    }
}
