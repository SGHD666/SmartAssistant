using SmartAssistant.UI.Converters;
using Material.Icons;
using Xunit;

namespace SmartAssistant.Tests.UI.Converters
{
    public class IconConverterTests
    {
        private readonly IconConverter _converter;

        public IconConverterTests()
        {
            _converter = new IconConverter();
        }

        [Fact]
        public void Convert_WhenUserIsTrue_ReturnsPerson()
        {
            // Arrange
            var value = true;

            // Act
            var result = _converter.Convert(value, null!, null!, null!);

            // Assert
            Assert.Equal(MaterialIconKind.Person, result);
        }

        [Fact]
        public void Convert_WhenUserIsFalse_ReturnsRobot()
        {
            // Arrange
            var value = false;

            // Act
            var result = _converter.Convert(value, null!, null!, null!);

            // Assert
            Assert.Equal(MaterialIconKind.Robot, result);
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
