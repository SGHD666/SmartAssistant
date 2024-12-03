using SmartAssistant.UI.Common;
using Xunit;

namespace SmartAssistant.Tests.Common
{
    public class MessageTypeTests
    {
        [Fact]
        public void MessageType_HasCorrectValues()
        {
            // Assert
            Assert.Equal(0, (int)MessageType.User);
            Assert.Equal(1, (int)MessageType.Assistant);
            Assert.Equal(2, (int)MessageType.System);
            Assert.Equal(3, (int)MessageType.Error);
        }

        [Theory]
        [InlineData(MessageType.User)]
        [InlineData(MessageType.Assistant)]
        [InlineData(MessageType.System)]
        [InlineData(MessageType.Error)]
        public void MessageType_CanBeUsedInSwitch(MessageType messageType)
        {
            // Act
            var description = GetMessageTypeDescription(messageType);

            // Assert
            Assert.NotNull(description);
            Assert.NotEmpty(description);
        }

        private string GetMessageTypeDescription(MessageType messageType)
        {
            return messageType switch
            {
                MessageType.User => "User message",
                MessageType.Assistant => "Assistant message",
                MessageType.System => "System message",
                MessageType.Error => "Error message",
                _ => string.Empty
            };
        }

        [Fact]
        public void MessageType_ToString_ReturnsCorrectString()
        {
            // Assert
            Assert.Equal("User", MessageType.User.ToString());
            Assert.Equal("Assistant", MessageType.Assistant.ToString());
            Assert.Equal("System", MessageType.System.ToString());
            Assert.Equal("Error", MessageType.Error.ToString());
        }
    }
}
