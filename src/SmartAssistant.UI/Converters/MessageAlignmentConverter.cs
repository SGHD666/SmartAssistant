// <copyright file="MessageAlignmentConverter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.UI.Converters
{
    using System;
    using System.Globalization;
    using Avalonia.Data.Converters;
    using Avalonia.Layout;

    /// <summary>
    /// Converts boolean values to horizontal alignment values for chat message layout.
    /// </summary>
    public class MessageAlignmentConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a horizontal alignment. True represents user messages (right-aligned),
        /// while False represents assistant messages (left-aligned).
        /// </summary>
        /// <param name="value">The boolean value indicating if the message is from the user.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A HorizontalAlignment - Right for user messages, Left for assistant messages.</returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            if (value is bool isUser)
            {
                return isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left;
            }

            return HorizontalAlignment.Left;
        }

        /// <summary>
        /// Converts a horizontal alignment value back to a boolean value. Not implemented.
        /// </summary>
        /// <param name="value">The alignment to convert.</param>
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
