using Avalonia.Data.Converters;
using Material.Icons;
using System;
using System.Globalization;

namespace SmartAssistant.UI.Converters
{
    public class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isUser = value is bool b && b;
            return isUser ? MaterialIconKind.Person : MaterialIconKind.Robot;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
