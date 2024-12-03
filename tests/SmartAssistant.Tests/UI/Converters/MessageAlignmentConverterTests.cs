using System;
using Avalonia.Layout;
using SmartAssistant.UI.Converters;
using Xunit;

namespace SmartAssistant.Tests.UI.Converters
{
    public class MessageAlignmentConverterTests
    {
        private readonly MessageAlignmentConverter _converter;

        public MessageAlignmentConverterTests()
        {
            _converter = new MessageAlignmentConverter();
        }

        [Theory]
        [InlineData(true, HorizontalAlignment.Right)]
        [InlineData(false, HorizontalAlignment.Left)]
        public void Convert_ShouldReturnCorrectAlignment(bool isUser, HorizontalAlignment expected)
        {
            var result = _converter.Convert(isUser, typeof(HorizontalAlignment), null, null);
            
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Convert_WithInvalidInput_ShouldReturnLeftAlignment()
        {
            var result = _converter.Convert("invalid", typeof(HorizontalAlignment), null, null);
            
            Assert.Equal(HorizontalAlignment.Left, result);
        }

        [Fact]
        public void ConvertBack_ShouldThrowNotImplementedException()
        {
            Assert.Throws<NotImplementedException>(() => 
                _converter.ConvertBack(HorizontalAlignment.Left, typeof(bool), null, null));
        }
    }
}
