// <copyright file="MessageRoleConverter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.UI.Converters
{
    using System;
    using System.Globalization;
    using Avalonia.Data.Converters;

    /// <summary>
    /// Converts boolean values to message role strings in the chat interface.
    /// True represents user messages ("You"), while False represents assistant messages ("Assistant").
    /// </summary>
    public class MessageRoleConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a role string.
        /// </summary>
        /// <param name="value">The boolean value indicating if the message is from the user.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// "You" for user messages (true),
        /// "Assistant" for assistant messages (false),
        /// "Unknown" if the input cannot be converted.
        /// </returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            // Explicitly cast and check the value
            bool? userFlag = value as bool?;
            if (userFlag.HasValue)
            {
                return userFlag.Value ? "You" : "Assistant";
            }

            return "Unknown";
        }

        /// <summary>
        /// Converts a role string back to a boolean value. Not implemented.
        /// </summary>
        /// <param name="value">The role string to convert.</param>
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
