// <copyright file="IconColorConverter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.UI.Converters
{
    using System;
    using System.Globalization;
    using Avalonia.Data.Converters;
    using Avalonia.Media;

    /// <summary>
    /// Converts boolean values to color brushes for chat message icons.
    /// </summary>
    public class IconColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a color brush. True represents user messages (warm blue),
        /// while False represents assistant messages (tech cyan).
        /// </summary>
        /// <param name="value">The boolean value indicating if the message is from the user.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A SolidColorBrush - blue for user messages, cyan for assistant messages.</returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            bool isUser = value is bool b && b;

            // User messages use warm blue, assistant messages use tech cyan
            return isUser ? new SolidColorBrush(Color.Parse("#4A90E2")) : new SolidColorBrush(Color.Parse("#00B8D4"));
        }

        /// <summary>
        /// Converts a color brush back to a boolean value. Not implemented.
        /// </summary>
        /// <param name="value">The color brush to convert.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>This method is not implemented and throws NotImplementedException.</returns>
        /// <exception cref="NotImplementedException">This conversion is not supported.</exception>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
