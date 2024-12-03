using System.Threading.Tasks;
using Avalonia.Controls;
using FluentAssertions;
using SmartAssistant.Tests.UI.Helpers;
using SmartAssistant.UI.ViewModels;
using SmartAssistant.UI.Views;
using Xunit;

namespace SmartAssistant.Tests.UI.Controls
{
    public class SettingsDialogTests : UITestBase
    {
        private SettingsDialog _dialog;
        private SettingsViewModel _viewModel;

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            await OnUIThread(() =>
            {
                _dialog = new SettingsDialog();
                _viewModel = new SettingsViewModel();
                _dialog.DataContext = _viewModel;
            });
        }

        [Fact]
        public async Task SettingsDialog_SaveButton_ShouldUpdateSettings()
        {
            // Arrange
            Button saveButton = null;
            TextBox apiKeyInput = null;
            CheckBox autoStartCheckBox = null;

            await OnUIThread(() =>
            {
                saveButton = UserInteractionHelper.FindControl<Button>(_dialog, "SaveButton");
                apiKeyInput = UserInteractionHelper.FindControl<TextBox>(_dialog, "ApiKeyInput");
                autoStartCheckBox = UserInteractionHelper.FindControl<CheckBox>(_dialog, "AutoStartCheckBox");
            });

            // Act
            await OnUIThread(async () =>
            {
                await UserInteractionHelper.EnterText(apiKeyInput, "test-api-key");
                await UserInteractionHelper.ToggleCheckBox(autoStartCheckBox, true);
                await UserInteractionHelper.ClickButton(saveButton);
            });

            // Assert
            await OnUIThread(() =>
            {
                _viewModel.ApiKey.Should().Be("test-api-key");
                _viewModel.AutoStart.Should().BeTrue();
                _dialog.IsVisible.Should().BeFalse(); // 确认对话框已关闭
            });
        }

        [Fact]
        public async Task SettingsDialog_CancelButton_ShouldNotUpdateSettings()
        {
            // Arrange
            const string originalApiKey = "original-key";
            Button cancelButton = null;
            TextBox apiKeyInput = null;

            await OnUIThread(() =>
            {
                _viewModel.ApiKey = originalApiKey;
                cancelButton = UserInteractionHelper.FindControl<Button>(_dialog, "CancelButton");
                apiKeyInput = UserInteractionHelper.FindControl<TextBox>(_dialog, "ApiKeyInput");
            });

            // Act
            await OnUIThread(async () =>
            {
                await UserInteractionHelper.EnterText(apiKeyInput, "new-api-key");
                await UserInteractionHelper.ClickButton(cancelButton);
            });

            // Assert
            await OnUIThread(() =>
            {
                _viewModel.ApiKey.Should().Be(originalApiKey); // 确认设置未被更改
                _dialog.IsVisible.Should().BeFalse(); // 确认对话框已关闭
            });
        }
    }
}
