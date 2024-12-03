using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using SmartAssistant.Tests.Common;

namespace SmartAssistant.Tests
{
    public class ExampleTests : TestBase
    {
        [Fact]
        public void TestHelper_GenerateRandomString_ShouldReturnCorrectLength()
        {
            // Arrange
            const int expectedLength = 15;

            // Act
            var result = TestHelper.GenerateRandomString(expectedLength);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Length.Should().Be(expectedLength);
        }

        [Fact]
        public async Task TestHelper_TempFile_ShouldCreateAndCleanup()
        {
            // Arrange
            var content = "Test Content";

            // Act
            var tempFile = await TestHelper.CreateTempTestFileAsync(content);

            // Assert
            File.Exists(tempFile).Should().BeTrue();
            var fileContent = await File.ReadAllTextAsync(tempFile);
            fileContent.Should().Be(content);

            // Cleanup
            TestHelper.CleanupTempFile(tempFile);
            File.Exists(tempFile).Should().BeFalse();
        }
    }
}
