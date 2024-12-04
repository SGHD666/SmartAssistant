# Core Services API Documentation

## Task Execution Service

The Task Execution Service is responsible for managing and executing user tasks.

### ITaskExecutionService

```csharp
public interface ITaskExecutionService
{
    Task<ExecutionResult> ExecuteAsync(string input, CancellationToken cancellationToken = default);
    Task<ExecutionStatus> GetStatusAsync(string taskId);
    Task CancelAsync(string taskId);
}
```

#### Methods

##### ExecuteAsync
Executes a task based on user input.
- **Parameters**
  - `input`: User input string
  - `cancellationToken`: Optional cancellation token
- **Returns**: `ExecutionResult` containing task outcome

##### GetStatusAsync
Retrieves the current status of a task.
- **Parameters**
  - `taskId`: Unique identifier of the task
- **Returns**: Current `ExecutionStatus`

##### CancelAsync
Cancels a running task.
- **Parameters**
  - `taskId`: Unique identifier of the task to cancel

## LLM Services

Services for interacting with various Language Learning Models.

### ILLMService

```csharp
public interface ILLMService
{
    Task<LLMResponse> ProcessAsync(string input);
    Task<bool> IsAvailableAsync();
    LLMType Type { get; }
}
```

#### Methods

##### ProcessAsync
Processes user input through the LLM.
- **Parameters**
  - `input`: User input to process
- **Returns**: `LLMResponse` containing model's response

##### IsAvailableAsync
Checks if the LLM service is available.
- **Returns**: Boolean indicating availability

#### Properties

##### Type
Gets the type of LLM service.
- **Type**: `LLMType` enum

## Automation Service

Handles system automation and command execution.

### IAutomationService

```csharp
public interface IAutomationService
{
    Task<CommandResult> ExecuteCommandAsync(string command, string[] args);
    Task<bool> IsCommandAvailableAsync(string command);
    IEnumerable<string> GetAvailableCommands();
}
```

#### Methods

##### ExecuteCommandAsync
Executes a system command.
- **Parameters**
  - `command`: Command to execute
  - `args`: Command arguments
- **Returns**: `CommandResult` with execution outcome

##### IsCommandAvailableAsync
Checks if a command is available.
- **Parameters**
  - `command`: Command to check
- **Returns**: Boolean indicating availability

##### GetAvailableCommands
Lists all available commands.
- **Returns**: Collection of command names

## Data Types

### ExecutionResult

```csharp
public class ExecutionResult
{
    public string TaskId { get; set; }
    public bool Success { get; set; }
    public string? Output { get; set; }
    public string? Error { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}
```

### ExecutionStatus

```csharp
public enum ExecutionStatus
{
    Pending,
    Running,
    Completed,
    Failed,
    Cancelled
}
```

### LLMResponse

```csharp
public class LLMResponse
{
    public bool Success { get; set; }
    public string? Content { get; set; }
    public double? Confidence { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}
```

### CommandResult

```csharp
public class CommandResult
{
    public bool Success { get; set; }
    public string? Output { get; set; }
    public string? Error { get; set; }
    public int ExitCode { get; set; }
}
```

## Usage Examples

### Task Execution

```csharp
public class ExampleUsage
{
    private readonly ITaskExecutionService _taskService;

    public async Task RunExample()
    {
        var result = await _taskService.ExecuteAsync("process file data.txt");
        
        if (result.Success)
        {
            Console.WriteLine($"Task {result.TaskId} completed successfully");
        }
        else
        {
            Console.WriteLine($"Error: {result.Error}");
        }
    }
}
```

### LLM Integration

```csharp
public class LLMExample
{
    private readonly ILLMService _llmService;

    public async Task ProcessInput()
    {
        if (await _llmService.IsAvailableAsync())
        {
            var response = await _llmService.ProcessAsync("Analyze this text");
            Console.WriteLine($"Response: {response.Content}");
        }
    }
}
```

### Command Execution

```csharp
public class AutomationExample
{
    private readonly IAutomationService _automationService;

    public async Task RunCommand()
    {
        var result = await _automationService.ExecuteCommandAsync("git", 
            new[] { "status" });
        
        if (result.Success)
        {
            Console.WriteLine(result.Output);
        }
    }
}
```

## Error Handling

Services use a combination of exceptions and result objects for error handling:

### Common Exceptions

```csharp
public class TaskExecutionException : Exception
{
    public string TaskId { get; }
    public ExecutionStatus Status { get; }
}

public class LLMServiceException : Exception
{
    public LLMType ServiceType { get; }
}

public class AutomationException : Exception
{
    public string Command { get; }
    public int ExitCode { get; }
}
```

## Related Documentation
- [System Architecture](../architecture/system-architecture.md)
- [Development Guidelines](../guides/development-guidelines.md)
- [Testing Guide](../guides/testing.md)
