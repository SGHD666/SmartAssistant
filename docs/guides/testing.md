# Testing Guide

## Overview

SmartAssistant uses a comprehensive testing strategy including unit tests, integration tests, and UI tests. We use:
- xUnit as the testing framework
- Moq for mocking
- FluentAssertions for assertions

## Test Structure

```
tests/
├── SmartAssistant.Tests/
│   ├── Core/               # Core service tests
│   ├── UI/                 # UI component tests
│   └── Integration/        # Integration tests
```

## Writing Tests

### Unit Test Structure
```csharp
public class ServiceTests
{
    // Arrange - Setup
    private readonly IService _service;
    private readonly Mock<IDependency> _mockDependency;

    public ServiceTests()
    {
        _mockDependency = new Mock<IDependency>();
        _service = new Service(_mockDependency.Object);
    }

    [Fact]
    public async Task MethodName_Scenario_ExpectedBehavior()
    {
        // Arrange
        _mockDependency.Setup(x => x.Method()).Returns(Task.FromResult(value));

        // Act
        var result = await _service.Method();

        // Assert
        result.Should().Be(expectedValue);
        _mockDependency.Verify(x => x.Method(), Times.Once);
    }
}
```

### Test Naming Convention
```
MethodName_Scenario_ExpectedBehavior

Examples:
- Execute_ValidInput_ReturnsSuccess
- Convert_NullInput_ThrowsArgumentException
- Process_EmptyList_ReturnsEmptyResult
```

## Test Categories

### 1. Unit Tests
- Test individual components in isolation
- Mock all dependencies
- Fast execution
- High coverage

### 2. Integration Tests
- Test component interactions
- Use real dependencies
- Database integration
- API integration

### 3. UI Tests
- View model tests
- Converter tests
- User interaction tests

## Code Coverage

### Requirements
- Minimum 80% code coverage
- Critical paths require 100% coverage
- Document uncovered code

### Running Coverage
```bash
# Run tests with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# Generate coverage report
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

## Mocking Guidelines

### Do Mock
- External services
- Database connections
- File system operations
- Network calls

### Don't Mock
- Value objects
- Static utility methods
- Framework classes

## Best Practices

1. **Test Independence**
   - Each test should be independent
   - Clean up test data
   - Reset mocks between tests

2. **Arrange-Act-Assert**
   - Clear separation of test phases
   - Single action per test
   - Specific assertions

3. **Test Data**
   - Use meaningful test data
   - Avoid magic numbers
   - Create test data helpers

4. **Error Cases**
   - Test edge cases
   - Verify error handling
   - Check exception details

## Example Tests

### Service Test
```csharp
[Fact]
public async Task ExecuteTask_ValidInput_ReturnsSuccess()
{
    // Arrange
    var input = "test command";
    _mockLLMService
        .Setup(x => x.ProcessAsync(input))
        .ReturnsAsync(new LLMResponse { Success = true });

    // Act
    var result = await _service.ExecuteAsync(input);

    // Assert
    result.Should().NotBeNull();
    result.Success.Should().BeTrue();
}
```

### Converter Test
```csharp
[Theory]
[InlineData(true, "You")]
[InlineData(false, "Assistant")]
public void Convert_BooleanInput_ReturnsExpectedString(bool input, string expected)
{
    // Arrange
    var converter = new MessageRoleConverter();

    // Act
    var result = converter.Convert(input, typeof(string), null, null);

    // Assert
    result.Should().Be(expected);
}
```

## CI Integration

### Test Workflow
1. Run unit tests
2. Run integration tests
3. Generate coverage report
4. Verify coverage thresholds
5. Upload test results

### Configuration
```yaml
test:
  stage: test
  script:
    - dotnet test
    - dotnet test /p:CollectCoverage=true
    - reportgenerator
  coverage: '/Line coverage: \d+\.\d+%/'
```

## Related Documentation
- [Development Guidelines](./development-guidelines.md)
- [CI/CD Setup](./cicd-setup.md)
- [Code Style Guide](./code-style.md)
