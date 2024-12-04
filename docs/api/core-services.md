# Core Services API Documentation | 核心服务 API 文档

## Task Execution Service | 任务执行服务

The Task Execution Service is responsible for managing and executing user tasks.

任务执行服务负责管理和执行用户任务。

### ITaskExecutionService

```csharp
public interface ITaskExecutionService
{
    Task<ExecutionResult> ExecuteAsync(string input, CancellationToken cancellationToken = default);
    Task<ExecutionStatus> GetStatusAsync(string taskId);
    Task CancelAsync(string taskId);
}
```

#### Methods | 方法

##### ExecuteAsync
Executes a task based on user input.
根据用户输入执行任务。

**Parameters | 参数:**
- `input`: The user's task input | 用户的任务输入
- `cancellationToken`: Optional cancellation token | 可选的取消令牌

**Returns | 返回:**
- `ExecutionResult`: Result of the task execution | 任务执行结果

##### GetStatusAsync
Gets the current status of a task.
获取任务的当前状态。

**Parameters | 参数:**
- `taskId`: ID of the task | 任务ID

**Returns | 返回:**
- `ExecutionStatus`: Current status of the task | 任务的当前状态

##### CancelAsync
Cancels a running task.
取消正在运行的任务。

**Parameters | 参数:**
- `taskId`: ID of the task to cancel | 要取消的任务ID

## LLM Services | LLM 服务

Services for interacting with various Language Learning Models.

LLM 服务处理与语言模型的交互。

### ILLMService

```csharp
public interface ILLMService
{
    Task<LLMResponse> ProcessAsync(string input);
    Task<bool> IsAvailableAsync();
    LLMType Type { get; }
}
```

#### Methods | 方法

##### ProcessAsync
Processes user input through the LLM.
使用语言模型处理用户输入。

**Parameters | 参数:**
- `input`: User input to process | 要处理的用户输入

**Returns | 返回:**
- `LLMResponse`: Response from the LLM | LLM 的响应

##### IsAvailableAsync
Checks if the LLM service is available.
检查 LLM 服务是否可用。

**Returns | 返回:**
- `bool`: True if the service is available | 如果服务可用则为 true

#### Properties | 属性

##### Type
Gets the type of LLM service.
获取 LLM 服务类型。

**Type | 类型:**
- `LLMType`: Type of the LLM service | LLM 服务类型

## Automation Service | 自动化服务

Handles system automation and command execution.

自动化服务处理系统自动化任务。

### IAutomationService

```csharp
public interface IAutomationService
{
    Task<CommandResult> ExecuteCommandAsync(string command, string[] args);
    Task<bool> IsCommandAvailableAsync(string command);
    IEnumerable<string> GetAvailableCommands();
}
```

#### Methods | 方法

##### ExecuteCommandAsync
Executes a system command.
执行系统命令。

**Parameters | 参数:**
- `command`: Command to execute | 要执行的命令
- `args`: Command arguments | 命令参数

**Returns | 返回:**
- `CommandResult`: Result of the command execution | 命令执行结果

##### IsCommandAvailableAsync
Checks if a command is available.
检查命令是否可用。

**Parameters | 参数:**
- `command`: Command to check | 要检查的命令

**Returns | 返回:**
- `bool`: True if the command is available | 如果命令可用则为 true

##### GetAvailableCommands
Lists all available commands.
列出所有可用的命令。

**Returns | 返回:**
- `IEnumerable<string>`: Collection of available commands | 可用命令集合

## Data Types | 数据类型

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
    Pending,    // 待处理
    Running,    // 运行中
    Completed,  // 已完成
    Failed,     // 失败
    Cancelled   // 已取消
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

## Usage Examples | 使用示例

### Task Execution | 任务执行

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

### LLM Integration | LLM 集成

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

### Command Execution | 命令执行

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

## Error Handling | 错误处理

Services use a combination of exceptions and result objects for error handling:

### Common Exceptions | 公共异常

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

## Related Documentation | 相关文档
- [System Architecture](../architecture/system-architecture.md)
- [Development Guidelines](../guides/development-guidelines.md)
- [Testing Guide](../guides/testing.md)
