using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using FluentAssertions;
using SmartAssistant.Tests.UI.Helpers;
using SmartAssistant.UI.Controls;
using Xunit;

namespace SmartAssistant.Tests.UI.Controls.CustomControls
{
    public class ChatInputBoxTests : UITestBase
    {
        private ChatInputBox _chatInputBox;

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            await OnUIThread(() =>
            {
                _chatInputBox = new ChatInputBox();
            });
        }

        [Fact]
        public async Task ChatInputBox_EnterText_ShouldUpdateText()
        {
            // Arrange
            const string testText = "Hello, Assistant!";
            TextBox inputBox = null;

            await OnUIThread(() =>
            {
                inputBox = UserInteractionHelper.FindControl<TextBox>(_chatInputBox, "InputTextBox");
            });

            // Act
            await OnUIThread(async () =>
            {
                await UserInteractionHelper.EnterText(inputBox, testText);
            });

            // Assert
            await OnUIThread(() =>
            {
                _chatInputBox.Text.Should().Be(testText);
            });
        }

        [Fact]
        public async Task ChatInputBox_SendButton_ShouldRaiseEvent()
        {
            // Arrange
            const string testMessage = "Test message";
            Button sendButton = null;
            TextBox inputBox = null;
            string receivedMessage = null;

            await OnUIThread(() =>
            {
                sendButton = UserInteractionHelper.FindControl<Button>(_chatInputBox, "SendButton");
                inputBox = UserInteractionHelper.FindControl<TextBox>(_chatInputBox, "InputTextBox");
                
                _chatInputBox.MessageSent += (s, e) => receivedMessage = e.Message;
            });

            // Act
            await OnUIThread(async () =>
            {
                await UserInteractionHelper.EnterText(inputBox, testMessage);
                await UserInteractionHelper.ClickButton(sendButton);
            });

            // Assert
            await OnUIThread(() =>
            {
                receivedMessage.Should().Be(testMessage);
                inputBox.Text.Should().BeEmpty(); // 发送后应清空输入框
            });
        }

        [Fact]
        public async Task ChatInputBox_EmptyText_SendButtonShouldBeDisabled()
        {
            // Arrange
            Button sendButton = null;

            await OnUIThread(() =>
            {
                sendButton = UserInteractionHelper.FindControl<Button>(_chatInputBox, "SendButton");
            });

            // Assert
            await OnUIThread(() =>
            {
                sendButton.IsEnabled.Should().BeFalse();
            });
        }

        [Fact]
        public async Task ChatInputBox_KeyboardShortcut_ShouldSendMessage()
        {
            // Arrange
            const string testMessage = "Test message";
            TextBox inputBox = null;
            string receivedMessage = null;

            await OnUIThread(() =>
            {
                inputBox = UserInteractionHelper.FindControl<TextBox>(_chatInputBox, "InputTextBox");
                _chatInputBox.MessageSent += (s, e) => receivedMessage = e.Message;
            });

            // Act
            await OnUIThread(async () =>
            {
                await UserInteractionHelper.EnterText(inputBox, testMessage);
                
                var keyEvent = new KeyEventArgs
                {
                    Key = Key.Enter,
                    KeyModifiers = KeyModifiers.Control
                };
                await inputBox.RaiseEventAsync(InputElement.KeyDownEvent, keyEvent);
            });

            // Assert
            await OnUIThread(() =>
            {
                receivedMessage.Should().Be(testMessage);
                inputBox.Text.Should().BeEmpty();
            });
        }
    }
}
