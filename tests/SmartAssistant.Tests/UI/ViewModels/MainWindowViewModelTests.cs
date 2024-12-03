using System;
using System.Threading.Tasks;
using FluentAssertions;
using SmartAssistant.UI.ViewModels;
using Xunit;
using SmartAssistant.Core.Models;
using SmartAssistant.Core.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using SmartAssistant.Core.Services.LLM;
using SmartAssistant.Core.Models;
using SmartAssistant.Core.Services; // Added using statement for LLMType

namespace SmartAssistant.Tests.UI.ViewModels
{
    public class MainWindowViewModelTests : ViewModelTestBase
    {
        private readonly MainWindowViewModel _viewModel;
        private readonly Mock<ModelManager> _modelManager;
        private readonly Mock<AssistantController> _assistantController;
        private readonly Mock<ILogger<MainWindowViewModel>> _logger;
        private readonly ModelSettings _modelSettings;

        public MainWindowViewModelTests()
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
            var mockFactory = new Mock<ILLMFactory>();
            mockFactory.Setup(factory => factory.CreateService(It.IsAny<LLMType>())).Returns(new Mock<ILanguageModelService>().Object);
            _modelManager = new Mock<ModelManager>(_modelSettings, mockFactory.Object);
            var mockLanguageModel = new Mock<ILanguageModelService>();
            var mockTaskExecutor = new Mock<ITaskExecutionService>();
            var mockLogger = new Mock<ILogger<AssistantController>>();
            _assistantController = new Mock<AssistantController>(mockLanguageModel.Object, mockTaskExecutor.Object, mockLogger.Object);
            _logger = new Mock<ILogger<MainWindowViewModel>>();

            _viewModel = new MainWindowViewModel(
                _modelManager.Object,
                _modelSettings,
                _assistantController.Object,
                _logger.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Assert
            _viewModel.IsRunning.Should().BeFalse();
            _viewModel.StatusMessage.Should().NotBeNull();
            _viewModel.StartCommand.Should().NotBeNull();
            _viewModel.StopCommand.Should().NotBeNull();
        }

        [Fact]
        public void StartCommand_WhenExecuted_ShouldUpdateIsRunning()
        {
            // Arrange
            _viewModel.IsRunning.Should().BeFalse();

            // Act
            _viewModel.StartCommand.Execute(null);

            // Assert
            _viewModel.IsRunning.Should().BeTrue();
            _viewModel.StatusMessage.Should().Be("Assistant is running");
        }

        [Fact]
        public void StopCommand_WhenExecuted_ShouldUpdateIsRunning()
        {
            // Arrange
            _viewModel.StartCommand.Execute(null);
            _viewModel.IsRunning.Should().BeTrue();

            // Act
            _viewModel.StopCommand.Execute(null);

            // Assert
            _viewModel.IsRunning.Should().BeFalse();
            _viewModel.StatusMessage.Should().Be("Assistant is stopped");
        }

        [Fact]
        public void Commands_EnabledState_ShouldBeCorrect()
        {
            // Initial state
            _viewModel.StartCommand.CanExecute(null).Should().BeTrue();
            _viewModel.StopCommand.CanExecute(null).Should().BeFalse();

            // After start
            _viewModel.StartCommand.Execute(null);
            _viewModel.StartCommand.CanExecute(null).Should().BeFalse();
            _viewModel.StopCommand.CanExecute(null).Should().BeTrue();

            // After stop
            _viewModel.StopCommand.Execute(null);
            _viewModel.StartCommand.CanExecute(null).Should().BeTrue();
            _viewModel.StopCommand.CanExecute(null).Should().BeFalse();
        }
    }
}
