using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace SmartAssistant.UI.Converters
{
    public class MessageRoleConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isUser)
            {
                return isUser ? "You" : "Assistant";
            }
            return "Unknown";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
