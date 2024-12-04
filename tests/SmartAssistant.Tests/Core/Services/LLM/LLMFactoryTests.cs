using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SmartAssistant.Core.Models;
using SmartAssistant.Core.Services.LLM;
using Xunit;

namespace SmartAssistant.Tests.Core.Services.LLM
{
    public class LLMFactoryTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEnumerable<ILanguageModelService> _languageModelServices;
        private readonly LLMFactory _factory;

        public LLMFactoryTests()
        {
            // 设置服务提供者
            var services = new ServiceCollection();
            services.AddTransient<OpenAIService>();
            services.AddTransient<ClaudeService>();
            services.AddTransient<QianWenService>();
            _serviceProvider = services.BuildServiceProvider();

            // 设置语言模型服务
            var mockQianWen = new Mock<ILanguageModelService>();
            var mockGPT = new Mock<ILanguageModelService>();
            var mockClaude = new Mock<ILanguageModelService>();

            _languageModelServices = new List<ILanguageModelService>
            {
                mockQianWen.Object,
                mockGPT.Object,
                mockClaude.Object
            };

            _factory = new LLMFactory(_serviceProvider, _languageModelServices);
        }

        [Theory]
        [InlineData(LLMType.QianWen, typeof(QianWenService))]
        [InlineData(LLMType.OpenAI_GPT4, typeof(OpenAIService))]
        [InlineData(LLMType.Claude, typeof(ClaudeService))]
        public void CreateService_ShouldReturnCorrectServiceType(LLMType modelType, Type expectedType)
        {
            // Act
            var service = _factory.CreateService(modelType);

            // Assert
            service.Should().NotBeNull();
            service.Should().BeOfType(expectedType);
        }

        [Fact]
        public void CreateService_WithInvalidType_ShouldThrowArgumentException()
        {
            // Arrange
            var invalidType = (LLMType)999;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _factory.CreateService(invalidType));
        }

        [Theory]
        [InlineData(LLMType.QianWen)]
        [InlineData(LLMType.OpenAI_GPT4)]
        [InlineData(LLMType.Claude)]
        public void CreateService_ShouldInitializeWithCorrectSettings(LLMType modelType)
        {
            // Act
            var service = _factory.CreateService(modelType);

            // Assert
            service.Should().NotBeNull();
        }
    }
}
