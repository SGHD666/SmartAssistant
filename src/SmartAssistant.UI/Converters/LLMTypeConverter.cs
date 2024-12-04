// <copyright file="LLMTypeConverter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.UI.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Avalonia.Data.Converters;
    using SmartAssistant.Core.Models;

    /// <summary>
    /// Converts between LLMType enum values and their display strings.
    /// </summary>
    public class LLMTypeConverter : IValueConverter
    {
        private static readonly Dictionary<string, LLMType> DisplayToEnum = new()
        {
            { "GPT-3.5", LLMType.OpenAI_GPT35 },
            { "GPT-4", LLMType.OpenAI_GPT4 },
            { "Claude", LLMType.Claude },
            { "QianWen", LLMType.QianWen },
        };

        private static readonly Dictionary<LLMType, string> EnumToDisplay = new()
        {
            { LLMType.OpenAI_GPT35, "GPT-3.5" },
            { LLMType.OpenAI_GPT4, "GPT-4" },
            { LLMType.Claude, "Claude" },
            { LLMType.QianWen, "QianWen" },
        };

        /// <summary>
        /// Converts an LLMType enum value to its display string representation.
        /// </summary>
        /// <param name="value">The LLMType enum value to convert.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The display string for the LLMType, or null if conversion fails.</returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            if (value is LLMType enumValue && EnumToDisplay.TryGetValue(enumValue, out var display))
            {
                return display;
            }

            return null;
        }

        /// <summary>
        /// Converts a display string back to its corresponding LLMType enum value.
        /// </summary>
        /// <param name="value">The display string to convert.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The LLMType enum value, or null if conversion fails.</returns>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            if (value is string display && DisplayToEnum.TryGetValue(display, out var enumValue))
            {
                return enumValue;
            }

            return null;
        }
    }
}
