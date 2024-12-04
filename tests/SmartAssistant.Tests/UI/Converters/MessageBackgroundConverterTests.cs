using System;
using Avalonia.Media;
using SmartAssistant.UI.Converters;
using Xunit;

namespace SmartAssistant.Tests.UI.Converters
{
    public class MessageBackgroundConverterTests
    {
        private readonly MessageBackgroundConverter _converter;

        public MessageBackgroundConverterTests()
        {
            _converter = new MessageBackgroundConverter();
        }

        [Fact]
        public void Convert_UserMessage_ShouldReturnLightGray()
        {
            var result = _converter.Convert(true, typeof(IBrush), null, null) as SolidColorBrush;
            
            Assert.NotNull(result);
            Assert.Equal(Color.FromRgb(230, 230, 230), result.Color);
        }

        [Fact]
        public void Convert_AssistantMessage_ShouldReturnLightBlue()
        {
            var result = _converter.Convert(false, typeof(IBrush), null, null) as SolidColorBrush;
            
            Assert.NotNull(result);
            Assert.Equal(Color.FromRgb(200, 220, 255), result.Color);
        }

        [Fact]
        public void Convert_InvalidInput_ShouldReturnTransparent()
        {
            var result = _converter.Convert("invalid", typeof(IBrush), null, null) as SolidColorBrush;
            
            Assert.NotNull(result);
            Assert.Equal(Colors.Transparent, result.Color);
        }

        [Fact]
        public void ConvertBack_ShouldThrowNotImplementedException()
        {
            Assert.Throws<NotImplementedException>(() => 
                _converter.ConvertBack(new SolidColorBrush(Colors.Red), typeof(bool), null, null));
        }
    }
}
