// <copyright file="IconConverter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.UI.Converters
{
    using System;
    using System.Globalization;
    using Avalonia.Data.Converters;
    using Material.Icons;

    /// <summary>
    /// Converts boolean values to Material Design icons for chat messages.
    /// </summary>
    public class IconConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a Material Design icon. True represents user messages (person icon),
        /// while False represents assistant messages (robot icon).
        /// </summary>
        /// <param name="value">The boolean value indicating if the message is from the user.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A MaterialIconKind - Person for user messages, Robot for assistant messages.</returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            bool isUser = value is bool b && b;
            return isUser ? MaterialIconKind.Person : MaterialIconKind.Robot;
        }

        /// <summary>
        /// Converts a Material Design icon back to a boolean value. Not implemented.
        /// </summary>
        /// <param name="value">The icon to convert.</param>
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
