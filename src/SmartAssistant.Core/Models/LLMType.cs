// <copyright file="LLMType.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core.Models
{
    /// <summary>
    /// Represents the different types of Language Learning Models (LLM) supported by the system.
    /// </summary>
    public enum LLMType
    {
        /// <summary>
        /// OpenAI's GPT-3.5 model.
        /// </summary>
        OpenAI_GPT35 = 0,

        /// <summary>
        /// OpenAI's GPT-4 model.
        /// </summary>
        OpenAI_GPT4 = 1,

        /// <summary>
        /// Anthropic's Claude model.
        /// </summary>
        Claude = 2,

        /// <summary>
        /// Baidu's QianWen model.
        /// </summary>
        QianWen = 3,
    }
}
