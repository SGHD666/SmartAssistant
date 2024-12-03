using SmartAssistant.UI.Converters;
using Avalonia.Media;
using Xunit;

namespace SmartAssistant.Tests.UI.Converters
{
    public class IconColorConverterTests
    {
        private readonly IconColorConverter _converter;

        public IconColorConverterTests()
        {
            _converter = new IconColorConverter();
        }

        [Fact]
        public void Convert_WhenUserIsTrue_ReturnsUserColor()
        {
            // Arrange
            var value = true;
            var expectedColor = Color.Parse("#4A90E2");

            // Act
            var result = _converter.Convert(value, null!, null!, null!) as SolidColorBrush;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedColor, result.Color);
        }

        [Fact]
        public void Convert_WhenUserIsFalse_ReturnsAssistantColor()
        {
            // Arrange
            var value = false;
            var expectedColor = Color.Parse("#00B8D4");

            // Act
            var result = _converter.Convert(value, null!, null!, null!) as SolidColorBrush;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedColor, result.Color);
        }

        [Fact]
        public void ConvertBack_ThrowsNotImplementedException()
        {
            // Assert
            Assert.Throws<NotImplementedException>(() =>
                _converter.ConvertBack(null!, null!, null!, null!));
        }
    }
}
