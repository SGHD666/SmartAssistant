using System;
using System.Threading.Tasks;
using FluentAssertions;
using SmartAssistant.UI.ViewModels;
using Xunit;

namespace SmartAssistant.Tests.UI.ViewModels
{
    public class MainWindowViewModelTests : ViewModelTestBase
    {
        private MainWindowViewModel _viewModel;

        public MainWindowViewModelTests()
        {
            _viewModel = new MainWindowViewModel();
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
