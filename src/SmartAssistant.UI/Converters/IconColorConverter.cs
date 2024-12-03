using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace SmartAssistant.UI.Converters
{
    public class IconColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isUser = value is bool b && b;
            // 用户图标使用温暖的蓝色，助手图标使用科技感的青色
            return isUser ? new SolidColorBrush(Color.Parse("#4A90E2")) : new SolidColorBrush(Color.Parse("#00B8D4"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
