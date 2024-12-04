using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using SmartAssistant.Core.Models;
using SmartAssistant.Core.Services.LLM;
using Xunit;

namespace SmartAssistant.Tests.Core.Services.LLM
{
    public class ModelManagerTests
    {
        private readonly ModelSettings _modelSettings;
        private readonly Mock<ILLMFactory> _mockFactory;
        private readonly ModelManager _modelManager;
        private readonly Mock<ILanguageModelService> _mockModelService;

        public ModelManagerTests()
        {
            _modelSettings = new ModelSettings
            {
                CurrentModel = LLMType.QianWen,
                ApiKey = "test-api-key",
                PythonPath = "/test/python/path",
                BasePath = "/test/base/path",
                ModelConfigs = new Dictionary<string, LLMConfig>
                {
                    { "QianWen", new LLMConfig { Type = LLMType.QianWen, ModelId = "qwen-max" } }
                }
            };

            _mockModelService = new Mock<ILanguageModelService>();
            _mockFactory = new Mock<ILLMFactory>();
            _mockFactory.Setup(f => f.CreateService(It.IsAny<LLMType>()))
                       .Returns(_mockModelService.Object);

            _modelManager = new ModelManager(_modelSettings, _mockFactory.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeWithSettings()
        {
            // Assert
            _modelManager.CurrentModel.Should().Be(LLMType.QianWen);
            _modelManager.Settings.Should().Be(_modelSettings);
        }

        [Fact]
        public void GetService_ShouldReturnCachedService()
        {
            // Act
            var service1 = _modelManager.GetService();
            var service2 = _modelManager.GetService();

            // Assert
            service1.Should().Be(service2);
            _mockFactory.Verify(f => f.CreateService(It.IsAny<LLMType>()), Times.Once);
        }

        [Fact]
        public void SwitchModel_ShouldUpdateCurrentModelAndCreateNewService()
        {
            // Arrange
            var newModelType = LLMType.OpenAI_GPT4;

            // Act
            _modelManager.SwitchModel(newModelType);

            // Assert
            _modelManager.CurrentModel.Should().Be(newModelType);
            _mockFactory.Verify(f => f.CreateService(newModelType), Times.Once);
        }

        [Fact]
        public void UpdateSettings_ShouldUpdateModelSettingsAndRecreateService()
        {
            // Arrange
            var newSettings = new ModelSettings
            {
                CurrentModel = LLMType.Claude,
                ApiKey = "new-api-key",
                ModelConfigs = new Dictionary<string, LLMConfig>
                {
                    { "Claude", new LLMConfig { Type = LLMType.Claude, ModelId = "claude-3" } }
                }
            };

            // Act
            _modelManager.UpdateSettings(newSettings);

            // Assert
            _modelManager.Settings.ApiKey.Should().Be(newSettings.ApiKey);
            _modelManager.Settings.CurrentModel.Should().Be(LLMType.Claude);
            _modelManager.Settings.ModelConfigs.Should().BeEquivalentTo(newSettings.ModelConfigs);
            _mockFactory.Verify(f => f.CreateService(LLMType.Claude), Times.Once);
        }
    }
}
