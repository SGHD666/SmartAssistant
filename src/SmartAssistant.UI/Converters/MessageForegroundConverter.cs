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
    /// Converts MessageType enum values to appropriate foreground colors for message display.
    /// </summary>
    public class MessageForegroundConverter : IValueConverter
    {
        /// <summary>
        /// Converts a MessageType value to a SolidColorBrush.
        /// Error messages are displayed in red, system messages in gray, and default messages in black.
        /// </summary>
        /// <param name="value">The MessageType value to convert.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A SolidColorBrush with the appropriate color for the message type.</returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
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

        /// <summary>
        /// Converts a SolidColorBrush back to a MessageType value. Not implemented.
        /// </summary>
        /// <param name="value">The color brush to convert.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>This method is not implemented and throws NotImplementedException.</returns>
        /// <exception cref="NotImplementedException">This conversion is not supported.</exception>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            throw new NotImplementedException();
        }
    }
}
