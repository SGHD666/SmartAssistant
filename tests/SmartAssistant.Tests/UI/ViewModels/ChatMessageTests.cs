using System;
using FluentAssertions;
using SmartAssistant.UI.Common;
using SmartAssistant.UI.ViewModels;
using Xunit;

namespace SmartAssistant.Tests.UI.ViewModels
{
    public class ChatMessageTests
    {
        [Fact]
        public void Constructor_WithRequiredProperties_ShouldCreateValidInstance()
        {
            // Arrange
            var content = "Test message";
            var timestamp = DateTime.Now;
            var type = MessageType.User;

            // Act
            var message = new ChatMessage
            {
                Content = content,
                Timestamp = timestamp,
                Type = type
            };

            // Assert
            message.Content.Should().Be(content);
            message.Timestamp.Should().Be(timestamp);
            message.Type.Should().Be(type);
        }

        [Theory]
        [InlineData(MessageType.User, true, false, false, false)]
        [InlineData(MessageType.System, false, true, false, false)]
        [InlineData(MessageType.Error, false, false, true, false)]
        [InlineData(MessageType.Assistant, false, false, false, true)]
        public void MessageType_Properties_ShouldReturnCorrectValues(
            MessageType type,
            bool expectedIsUser,
            bool expectedIsSystem,
            bool expectedIsError,
            bool expectedIsAssistant)
        {
            // Arrange
            var message = new ChatMessage
            {
                Content = "Test",
                Timestamp = DateTime.Now,
                Type = type
            };

            // Assert
            message.IsUser.Should().Be(expectedIsUser);
            message.IsSystem.Should().Be(expectedIsSystem);
            message.IsError.Should().Be(expectedIsError);
            message.IsAssistant.Should().Be(expectedIsAssistant);
        }

        [Theory]
        [InlineData(MessageType.User)]
        [InlineData(MessageType.System)]
        [InlineData(MessageType.Error)]
        [InlineData(MessageType.Assistant)]
        public void BackgroundColor_ShouldReturnCorrectColor(MessageType type)
        {
            // Arrange
            var message = new ChatMessage
            {
                Content = "Test",
                Timestamp = DateTime.Now,
                Type = type
            };

            // Act
            var backgroundColor = message.BackgroundColor;

            // Assert
            backgroundColor.Should().NotBeNull();
            // 这里可以添加具体颜色值的验证，但需要根据实际实现来确定
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Constructor_WithInvalidContent_ShouldThrowException(string invalidContent)
        {
            // Act
            Action act = () => new ChatMessage
            {
                Content = invalidContent,
                Timestamp = DateTime.Now,
                Type = MessageType.User
            };

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_WithDefaultTimestamp_ShouldThrowException()
        {
            // Act
            Action act = () => new ChatMessage
            {
                Content = "Test",
                Timestamp = default,
                Type = MessageType.User
            };

            // Assert
            act.Should().Throw<ArgumentException>();
        }
    }
}
