using Avalonia.Media;
using SmartAssistant.UI.Common;
using SmartAssistant.UI.Converters;
using System;
using Xunit;

namespace SmartAssistant.Tests.UI.Converters
{
    public class MessageForegroundConverterTests
    {
        private readonly MessageForegroundConverter _converter = new();

        [Theory]
        [InlineData(MessageType.Error, "Red")]
        [InlineData(MessageType.System, "Gray")]
        [InlineData(MessageType.User, "Black")]
        [InlineData(MessageType.Assistant, "Black")]
        public void Convert_ShouldReturnCorrectColor(MessageType type, string expectedColor)
        {
            // Act
            var result = _converter.Convert(type, typeof(IBrush), null, null) as SolidColorBrush;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedColor, result.Color.ToString());
        }

        [Fact]
        public void Convert_WithNullValue_ShouldReturnBlack()
        {
            // Act
            var result = _converter.Convert(null, typeof(IBrush), null, null) as SolidColorBrush;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Black", result.Color.ToString());
        }

        [Fact]
        public void ConvertBack_ShouldThrowNotImplementedException()
        {
            // Assert
            Assert.Throws<NotImplementedException>(() =>
                _converter.ConvertBack(null, typeof(MessageType), null, null));
        }
    }
}
