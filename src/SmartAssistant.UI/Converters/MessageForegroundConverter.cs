// <copyright file="MessageForegroundConverter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.UI.Converters
{
    using System;
    using System.Globalization;
    using Avalonia.Data.Converters;
    using Avalonia.Media;
    using SmartAssistant.UI.Common;

    /// <summary>
    /// Converts message type to foreground color
    /// </summary>
    public class MessageForegroundConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is MessageType type)
            {
                return type switch
                {
                    MessageType.Error => new SolidColorBrush(Colors.Red),
                    MessageType.System => new SolidColorBrush(Colors.Gray),
                    _ => new SolidColorBrush(Colors.Black),
                };
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
