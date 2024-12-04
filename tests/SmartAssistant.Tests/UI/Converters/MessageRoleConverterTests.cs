using SmartAssistant.UI.Converters;
using System;
using Xunit;

namespace SmartAssistant.Tests.UI.Converters
{
    public class MessageRoleConverterTests
    {
        private readonly MessageRoleConverter _converter = new();

        [Theory]
        [InlineData(true, "You")]
        [InlineData(false, "Assistant")]
        public void Convert_ShouldReturnCorrectRole(bool isUser, string expectedRole)
        {
            // Act
            var result = _converter.Convert(isUser, typeof(string), null, null) as string;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedRole, result);
        }

        [Fact]
        public void Convert_WithNullValue_ShouldReturnUnknown()
        {
            // Act
            var result = _converter.Convert(null, typeof(string), null, null) as string;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Unknown", result);
        }

        [Fact]
        public void ConvertBack_ShouldThrowNotImplementedException()
        {
            // Assert
            Assert.Throws<NotImplementedException>(() =>
                _converter.ConvertBack(null, typeof(bool), null, null));
        }
    }
}
