# SmartAssistant API Documentation

## Core Services

### Task Execution System

#### ITaskExecutionService

The main service interface for executing user tasks and commands.

```csharp
public interface ITaskExecutionService
{
    // Execute a task with the given input
    Task<ExecutionResult> ExecuteAsync(string input, CancellationToken cancellationToken = default);
    
    // Get the status of a running task
    Task<ExecutionStatus> GetStatusAsync(string taskId);
    
    // Cancel a running task
    Task CancelAsync(string taskId);
}
```

#### IAutomationService

Interface for performing automated actions on the system.

```csharp
public interface IAutomationService
{
    // Execute a system command
    Task<ExecutionResult> ExecuteCommandAsync(string command, CancellationToken cancellationToken = default);
    
    // Simulate keyboard input
    Task SendKeysAsync(string keys);
    
    // Simulate mouse clicks
    Task ClickAsync(int x, int y);
    
    // Get screen information
    Task<ScreenInfo> GetScreenInfoAsync();
}
```

### Python Integration

#### IPythonRuntimeService

Service for managing the Python runtime environment.

```csharp
public interface IPythonRuntimeService
{
    // Initialize the Python runtime
    Task InitializeAsync();
    
    // Check if Python runtime is available
    bool IsAvailable { get; }
    
    // Get Python version information
    string GetVersionInfo();
}
```

#### IPythonScriptExecutor

Service for executing Python scripts.

```csharp
public interface IPythonScriptExecutor
{
    // Execute a Python script
    Task<ExecutionResult> ExecuteScriptAsync(string script, CancellationToken cancellationToken = default);
    
    // Execute a Python script file
    Task<ExecutionResult> ExecuteScriptFileAsync(string filePath, CancellationToken cancellationToken = default);
}
```

### Language Models

#### ILanguageModelService

Base interface for all language model services.

```csharp
public interface ILanguageModelService
{
    // Generate response for user input
    Task<string> GenerateResponseAsync(string input, CancellationToken cancellationToken = default);
    
    // Get model information
    ModelInfo GetModelInfo();
}
```

Implementations:
- OpenAIService - Service for OpenAI GPT-3.5/4 models
- ClaudeService - Service for Anthropic's Claude models
- QianWenService - Service for QianWen models

## Models

### ExecutionResult

Represents the result of a task execution.

```csharp
public class ExecutionResult
{
    public string TaskId { get; set; }
    public bool Success { get; set; }
    public string Output { get; set; }
    public string Error { get; set; }
    public ExecutionStatus Status { get; set; }
}
```

### ExecutionStatus

Enum representing the status of a task.

```csharp
public enum ExecutionStatus
{
    NotStarted,
    Running,
    Completed,
    Failed,
    Cancelled
}
```

### ModelInfo

Information about a language model.

```csharp
public class ModelInfo
{
    public string Name { get; set; }
    public string Version { get; set; }
    public Dictionary<string, string> Capabilities { get; set; }
}
```

## UI Components

### Value Converters

SmartAssistant uses several value converters to transform data for UI display. All converters implement `IValueConverter` and follow a consistent pattern.

#### IconConverter

Converts boolean values to Material Design icons for chat messages.

```csharp
public class IconConverter : IValueConverter
{
    // True -> Person icon (user)
    // False -> Robot icon (assistant)
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture);
}
```

#### IconColorConverter

Converts boolean values to color brushes for chat message icons.

```csharp
public class IconColorConverter : IValueConverter
{
    // True -> Warm blue (#4A90E2) for user
    // False -> Tech cyan (#00B8D4) for assistant
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture);
}
```

#### LLMTypeConverter

Converts between LLMType enum values and their display strings.

```csharp
public class LLMTypeConverter : IValueConverter
{
    // Enum -> Display string mapping:
    // OpenAI_GPT35 -> "GPT-3.5"
    // OpenAI_GPT4 -> "GPT-4"
    // Claude -> "Claude"
    // QianWen -> "QianWen"
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture);
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture);
}
```

#### MessageAlignmentConverter

Converts boolean values to horizontal alignment values for chat message layout.

```csharp
public class MessageAlignmentConverter : IValueConverter
{
    // True -> HorizontalAlignment.Right (user messages)
    // False -> HorizontalAlignment.Left (assistant messages)
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture);
}
```

#### MessageForegroundConverter

Converts MessageType enum values to appropriate foreground colors.

```csharp
public class MessageForegroundConverter : IValueConverter
{
    // MessageType.Error -> Red
    // MessageType.System -> Gray
    // Default -> Black
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture);
}
```

#### MessageRoleConverter

Converts boolean values to message role strings in the chat interface.

```csharp
public class MessageRoleConverter : IValueConverter
{
    // True -> "You" (user messages)
    // False -> "Assistant" (assistant messages)
    // Invalid -> "Unknown"
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture);
}
```

### Common Converter Patterns

All UI converters in SmartAssistant follow these patterns:

1. **Nullability**: All converters properly handle null values and use nullable reference types
2. **ConvertBack**: Most converters do not support back-conversion and throw `NotImplementedException`
3. **Default Values**: Converters provide sensible defaults when input is invalid
4. **Type Safety**: Converters use explicit type checking before conversion

### Usage Example

```xaml
<TextBlock Text="{Binding IsUserMessage, Converter={StaticResource MessageRoleConverter}}"
           Foreground="{Binding MessageType, Converter={StaticResource MessageForegroundConverter}}"
           HorizontalAlignment="{Binding IsUserMessage, Converter={StaticResource MessageAlignmentConverter}}"/>
```

## Usage Examples

### Task Execution

```csharp
// Execute a task
var result = await taskExecutionService.ExecuteAsync("open youtube");

// Check task status
var status = await taskExecutionService.GetStatusAsync(result.TaskId);

// Cancel task if needed
await taskExecutionService.CancelAsync(result.TaskId);
```

### Automation

```csharp
// Execute system command
var result = await automationService.ExecuteCommandAsync("notepad.exe");

// Simulate keyboard input
await automationService.SendKeysAsync("Hello World");

// Click at coordinates
await automationService.ClickAsync(100, 100);
```

### Language Model Integration

```csharp
// Generate response using language model
var response = await languageModelService.GenerateResponseAsync("What's the weather like?");

// Get model information
var modelInfo = languageModelService.GetModelInfo();
```

## Detailed Method Documentation

### TaskExecutionService

#### ExecuteAsync

```csharp
Task<ExecutionResult> ExecuteAsync(string input, CancellationToken cancellationToken = default)
```

Executes a user task based on natural language input.

**Parameters:**
- `input` (string): Natural language command from the user
- `cancellationToken` (CancellationToken): Token for cancellation support

**Returns:**
- `ExecutionResult`: Result of the task execution

**Exceptions:**
- `TaskExecutionException`: Thrown when task execution fails
- `ArgumentNullException`: Thrown when input is null or empty

**Example:**
```csharp
var result = await taskExecutionService.ExecuteAsync("open chrome and navigate to github.com");
if (result.Success)
{
    Console.WriteLine($"Task {result.TaskId} completed successfully");
}
```

#### GetStatusAsync

```csharp
Task<ExecutionStatus> GetStatusAsync(string taskId)
```

Gets the current status of a task.

**Parameters:**
- `taskId` (string): ID of the task to check

**Returns:**
- `ExecutionStatus`: Current status of the task

**Exceptions:**
- `TaskNotFoundException`: Thrown when task ID is not found

### AutomationService

#### ExecuteCommandAsync

```csharp
Task<ExecutionResult> ExecuteCommandAsync(string command, CancellationToken cancellationToken = default)
```

Executes a system command.

**Parameters:**
- `command` (string): Command to execute
- `cancellationToken` (CancellationToken): Token for cancellation support

**Returns:**
- `ExecutionResult`: Result of the command execution

**Security Note:**
This method performs input validation and sanitization to prevent command injection.

#### SendKeysAsync

```csharp
Task SendKeysAsync(string keys)
```

Simulates keyboard input.

**Parameters:**
- `keys` (string): Keys to send (supports special keys like {ENTER}, {TAB})

**Example:**
```csharp
// Type text and press Enter
await automationService.SendKeysAsync("Hello World{ENTER}");

// Press multiple special keys
await automationService.SendKeysAsync("{CTRL}c{CTRL}v");
```

### PythonRuntimeService

#### InitializeAsync

```csharp
Task InitializeAsync()
```

Initializes the Python runtime environment.

**Behavior:**
- Checks Python installation
- Sets up virtual environment if needed
- Installs required packages
- Configures environment variables

**Example:**
```csharp
await pythonRuntimeService.InitializeAsync();
if (pythonRuntimeService.IsAvailable)
{
    var version = pythonRuntimeService.GetVersionInfo();
    Console.WriteLine($"Python {version} ready");
}
```

## Extended Usage Examples

### Complex Task Execution

```csharp
// Execute a complex task with multiple steps
public async Task ProcessDocumentAsync(string filePath)
{
    try
    {
        // 1. Open document
        var openResult = await taskExecutionService.ExecuteAsync($"open document {filePath}");
        if (!openResult.Success) throw new TaskExecutionException("Failed to open document");

        // 2. Extract text
        var extractResult = await pythonScriptExecutor.ExecuteScriptAsync(@"
            import pytesseract
            from PIL import Image
            text = pytesseract.image_to_string(Image.open('temp.png'))
            print(text)
        ");

        // 3. Process with language model
        var processResult = await languageModelService.GenerateResponseAsync(
            $"Summarize this text: {extractResult.Output}"
        );

        // 4. Save results
        await File.WriteAllTextAsync("summary.txt", processResult);
    }
    catch (Exception ex)
    {
        // Handle errors appropriately
        logger.LogError(ex, "Document processing failed");
        throw;
    }
}
```

### Automation Workflow

```csharp
// Automate a web workflow
public async Task AutomateWebWorkflowAsync()
{
    try
    {
        // 1. Open browser
        await automationService.ExecuteCommandAsync("chrome.exe");
        await Task.Delay(1000); // Wait for browser to open

        // 2. Navigate to URL
        await automationService.SendKeysAsync("https://example.com{ENTER}");
        await Task.Delay(2000); // Wait for page load

        // 3. Fill form
        var screenInfo = await automationService.GetScreenInfoAsync();
        await automationService.ClickAsync(screenInfo.Width / 2, 300);
        await automationService.SendKeysAsync("username{TAB}password{ENTER}");

        // 4. Wait for result
        await Task.Delay(1000);
        
        // 5. Capture result
        // Implementation depends on specific needs
    }
    catch (AutomationException ex)
    {
        logger.LogError(ex, "Web automation failed");
        throw;
    }
}
```

## Performance Best Practices

### 1. Resource Management

```csharp
// Use async/await properly
public async Task<ExecutionResult> ExecuteWithResourcesAsync()
{
    using var scope = serviceProvider.CreateScope();
    await using var resource = await GetResourceAsync();
    try
    {
        return await ProcessWithResourceAsync(resource);
    }
    finally
    {
        await CleanupAsync();
    }
}
```

### 2. Caching

```csharp
// Implement caching for frequently used data
public class CachedLanguageModelService : ILanguageModelService
{
    private readonly IMemoryCache _cache;
    private readonly ILanguageModelService _innerService;

    public async Task<string> GenerateResponseAsync(string input)
    {
        var cacheKey = $"response_{GetHash(input)}";
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetAbsoluteExpiration(TimeSpan.FromHours(1));
            return await _innerService.GenerateResponseAsync(input);
        });
    }
}
```

### 3. Batch Processing

```csharp
// Process multiple items efficiently
public async Task ProcessBatchAsync(IEnumerable<string> inputs)
{
    var batches = inputs.Chunk(100); // Process in batches of 100
    foreach (var batch in batches)
    {
        await Task.WhenAll(batch.Select(ProcessSingleAsync));
    }
}
```

### 4. Memory Management

- Use object pooling for frequently created objects
- Implement proper disposal patterns
- Monitor memory usage in long-running operations

### 5. Error Handling and Retry Logic

```csharp
// Implement retry logic for transient failures
public async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation)
{
    var retryPolicy = Policy<T>
        .Handle<HttpRequestException>()
        .Or<TimeoutException>()
        .WaitAndRetryAsync(3, retryAttempt => 
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    return await retryPolicy.ExecuteAsync(operation);
}
```

## Configuration Options

### appsettings.json

```json
{
  "SmartAssistant": {
    "TaskExecution": {
      "MaxConcurrentTasks": 10,
      "DefaultTimeout": "00:05:00",
      "RetryAttempts": 3
    },
    "Automation": {
      "InputDelay": 100,
      "ScreenshotQuality": 90,
      "SafeMode": true
    },
    "Python": {
      "VirtualEnvPath": "./venv",
      "RequirementsFile": "./requirements.txt",
      "TimeoutSeconds": 30
    },
    "LanguageModel": {
      "DefaultModel": "gpt-4",
      "Temperature": 0.7,
      "MaxTokens": 2000,
      "CacheEnabled": true,
      "CacheDuration": "01:00:00"
    },
    "Logging": {
      "MinimumLevel": "Information",
      "FilePath": "logs/smartassistant.log",
      "RollingInterval": "Day"
    }
  }
}
```

### Environment Variables

```bash
# API Keys
SMARTASSISTANT_OPENAI_KEY=sk-...
SMARTASSISTANT_CLAUDE_KEY=sk-...
SMARTASSISTANT_QIANWEN_KEY=sk-...

# Feature Flags
SMARTASSISTANT_ENABLE_CACHE=true
SMARTASSISTANT_SAFE_MODE=true
SMARTASSISTANT_DEBUG=false

# Performance Settings
SMARTASSISTANT_MAX_THREADS=4
SMARTASSISTANT_MEMORY_LIMIT=1024
```

## API Versioning

### Version History

#### v0.1.0 (2024-01-31)
- Initial release
- Basic task execution
- Simple automation
- OpenAI integration

#### v0.1.1 (2024-02-07)
- Added documentation
- Improved error handling
- Enhanced Python integration

### Version Control

```csharp
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AssistantController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ExecutionResult>> ExecuteTask(
        [FromBody] TaskRequest request)
    {
        // Implementation
    }
}
```

### Breaking Changes

#### v0.1.0 → v0.1.1
- `ExecuteAsync` now returns `ExecutionResult` instead of `bool`
- Added required `CancellationToken` parameter to async methods
- Changed configuration structure in appsettings.json

### Deprecation Policy
- APIs are deprecated with one minor version notice
- Deprecated APIs remain functional for one major version
- Breaking changes only in major versions

### Migration Guides

#### Migrating to v0.1.1
```csharp
// Old code (v0.1.0)
var success = await service.ExecuteAsync("command");

// New code (v0.1.1)
var result = await service.ExecuteAsync("command", CancellationToken.None);
var success = result.Success;
```

## Security Guidelines

### 1. Input Validation

```csharp
public class InputValidator
{
    public static bool ValidateCommand(string command)
    {
        if (string.IsNullOrEmpty(command)) return false;
        
        // Check for dangerous patterns
        var dangerousPatterns = new[] { "&", "|", ">", "<", ";", "`" };
        return !dangerousPatterns.Any(p => command.Contains(p));
    }
}
```

### 2. Authentication

```csharp
public class AuthenticatedTaskExecutionService : ITaskExecutionService
{
    private readonly ITaskExecutionService _inner;
    private readonly IAuthenticationService _auth;

    public async Task<ExecutionResult> ExecuteAsync(string input, 
        CancellationToken cancellationToken = default)
    {
        if (!await _auth.ValidateTokenAsync())
        {
            throw new UnauthorizedException();
        }
        return await _inner.ExecuteAsync(input, cancellationToken);
    }
}
```

### 3. Rate Limiting

```csharp
public class RateLimitedLanguageModelService : ILanguageModelService
{
    private readonly ILanguageModelService _inner;
    private readonly RateLimiter _rateLimiter;

    public async Task<string> GenerateResponseAsync(string input)
    {
        await _rateLimiter.WaitAsync();
        try
        {
            return await _inner.GenerateResponseAsync(input);
        }
        finally
        {
            _rateLimiter.Release();
        }
    }
}
```

## Error Handling

All service methods may throw the following exceptions:

- `SmartAssistantException` - Base exception for all SmartAssistant errors
- `TaskExecutionException` - Error during task execution
- `AutomationException` - Error during automation operations
- `LanguageModelException` - Error when using language models

It's recommended to wrap service calls in try-catch blocks and handle these exceptions appropriately.
