using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace SmartAssistant.UI.Converters
{
    public class MessageBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isUser)
            {
                return isUser 
                    ? new SolidColorBrush(Color.FromRgb(230, 230, 230)) // Light gray for user messages
                    : new SolidColorBrush(Color.FromRgb(200, 220, 255));  // Light blue for assistant messages
            }
            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
