using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using FluentAssertions;
using SmartAssistant.UI.ViewModels;
using SmartAssistant.UI.Views;
using Xunit;

namespace SmartAssistant.Tests.UI.Controls
{
    public class MainWindowTests : UITestBase
    {
        private MainWindow _mainWindow;
        private MainWindowViewModel _viewModel;

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            await OnUIThread(() =>
            {
                _mainWindow = new MainWindow();
                _viewModel = new MainWindowViewModel();
                _mainWindow.DataContext = _viewModel;
            });
        }

        [Fact]
        public async Task MainWindow_InitialState_ShouldBeCorrect()
        {
            // Act - 确保在UI线程上访问控件
            await OnUIThread(() =>
            {
                // Assert
                _mainWindow.Should().NotBeNull();
                _viewModel.Should().NotBeNull();
                _mainWindow.DataContext.Should().Be(_viewModel);
            });
        }

        [Fact]
        public async Task MainWindow_Title_ShouldBeSet()
        {
            // Arrange
            var expectedTitle = "Smart Assistant";

            // Act
            await OnUIThread(() =>
            {
                // Assert
                _mainWindow.Title.Should().Be(expectedTitle);
            });
        }

        [Fact]
        public async Task ViewModel_Commands_ShouldBeInitialized()
        {
            // Act & Assert
            await OnUIThread(() =>
            {
                _viewModel.StartCommand.Should().NotBeNull();
                _viewModel.StopCommand.Should().NotBeNull();
                _viewModel.SettingsCommand.Should().NotBeNull();
            });
        }
    }
}
