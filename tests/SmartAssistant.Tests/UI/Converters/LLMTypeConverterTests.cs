using System;
using SmartAssistant.Core.Models;
using SmartAssistant.UI.Converters;
using Xunit;

namespace SmartAssistant.Tests.UI.Converters
{
    public class LLMTypeConverterTests
    {
        private readonly LLMTypeConverter _converter;

        public LLMTypeConverterTests()
        {
            _converter = new LLMTypeConverter();
        }

        [Theory]
        [InlineData(LLMType.OpenAI_GPT35, "GPT-3.5")]
        [InlineData(LLMType.OpenAI_GPT4, "GPT-4")]
        [InlineData(LLMType.Claude, "Claude")]
        [InlineData(LLMType.QianWen, "QianWen")]
        public void Convert_ShouldReturnCorrectDisplayString(LLMType input, string expected)
        {
            var result = _converter.Convert(input, typeof(string), null, null);
            
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("GPT-3.5", LLMType.OpenAI_GPT35)]
        [InlineData("GPT-4", LLMType.OpenAI_GPT4)]
        [InlineData("Claude", LLMType.Claude)]
        [InlineData("QianWen", LLMType.QianWen)]
        public void ConvertBack_ShouldReturnCorrectEnumValue(string input, LLMType expected)
        {
            var result = _converter.ConvertBack(input, typeof(LLMType), null, null);
            Assert.NotNull(result);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("InvalidModel")]
        public void ConvertBackWithInvalidInputShouldReturnNull(string? input)
        {
            var result = _converter.ConvertBack(input, typeof(LLMType), null, null);
            Assert.Null(result);
        }

    }
}
