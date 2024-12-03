using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using SmartAssistant.Core.Models;

namespace SmartAssistant.UI.Converters
{
    public class LLMTypeConverter : IValueConverter
    {
        private static readonly Dictionary<string, LLMType> _displayToEnum = new()
        {
            { "GPT-3.5", LLMType.OpenAI_GPT35 },
            { "GPT-4", LLMType.OpenAI_GPT4 },
            { "Claude", LLMType.Claude },
            { "QianWen", LLMType.QianWen }
        };

        private static readonly Dictionary<LLMType, string> _enumToDisplay = new()
        {
            { LLMType.OpenAI_GPT35, "GPT-3.5" },
            { LLMType.OpenAI_GPT4, "GPT-4" },
            { LLMType.Claude, "Claude" },
            { LLMType.QianWen, "QianWen" }
        };

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is LLMType enumValue && _enumToDisplay.TryGetValue(enumValue, out var display))
            {
                return display;
            }
            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string display && _displayToEnum.TryGetValue(display, out var enumValue))
            {
                return enumValue;
            }
            return null;
        }
    }
}
