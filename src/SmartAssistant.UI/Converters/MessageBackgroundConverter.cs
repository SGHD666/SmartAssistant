// <copyright file="MessageBackgroundConverter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.UI.Converters
{
    using System;
    using System.Globalization;
    using Avalonia.Data.Converters;
    using Avalonia.Media;

/// <summary>
/// Converts message type to background color
/// </summary>
    public class MessageBackgroundConverter : IValueConverter
    {
        /// <summary>
        /// Converts message type to background color.
        /// </summary>
        /// <returns>The background color.</returns>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            if (value is bool isUser)
            {
                return isUser
                    ? new SolidColorBrush(Color.FromRgb(230, 230, 230)) // Light gray for user messages
                    : new SolidColorBrush(Color.FromRgb(200, 220, 255));  // Light blue for assistant messages
            }

            return new SolidColorBrush(Colors.Transparent);
        }

        /// <summary>
        /// Converts background color to message type.
        /// </summary>
        /// <returns>The message type.</returns>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            throw new NotImplementedException();
        }
    }
}
