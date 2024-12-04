# Code Style Guide

## General Principles

1. **Clarity First**
   - Write code for humans to read
   - Prioritize clarity over cleverness
   - Use meaningful names

2. **Consistency**
   - Follow established patterns
   - Use consistent naming
   - Maintain consistent formatting

3. **SOLID Principles**
   - Single Responsibility
   - Open/Closed
   - Liskov Substitution
   - Interface Segregation
   - Dependency Inversion

## Naming Conventions

### Classes and Types
```csharp
public class TaskExecutionService    // PascalCase for types
public interface ITaskExecutor       // 'I' prefix for interfaces
public enum TaskStatus              // PascalCase for enums
```

### Variables and Fields
```csharp
private readonly ILogger _logger;    // '_' prefix for private fields
public string UserId { get; set; }   // PascalCase for properties
var taskResult = Execute();          // camelCase for local variables
```

### Methods and Events
```csharp
public async Task ExecuteAsync()     // PascalCase for methods
public event EventHandler<T> TaskCompleted;  // PascalCase for events
```

## File Organization

### File Structure
```csharp
// 1. File header with copyright
// 2. Namespace declarations
// 3. Using statements
// 4. Type declarations

using System;
using System.Threading.Tasks;

namespace SmartAssistant.Core
{
    public class ExampleClass
    {
        // 1. Private fields
        // 2. Constructors
        // 3. Properties
        // 4. Public methods
        // 5. Private methods
    }
}
```

### Code Organization
```csharp
public class ServiceClass
{
    // Fields
    private readonly ILogger _logger;
    
    // Constructors
    public ServiceClass(ILogger logger)
    {
        _logger = logger;
    }
    
    // Properties
    public bool IsEnabled { get; set; }
    
    // Public Methods
    public async Task ExecuteAsync()
    {
        // Implementation
    }
    
    // Private Methods
    private void Initialize()
    {
        // Implementation
    }
}
```

## Formatting

### Indentation and Spacing
```csharp
// Use 4 spaces for indentation
public class Example
{
    public void Method()
    {
        if (condition)
        {
            // Code
        }
    }
}

// Space after keywords
if (condition)
foreach (var item in items)

// Space around operators
var sum = a + b;
```

### Line Breaks
```csharp
// Break long method chains
var result = items
    .Where(x => x.IsValid)
    .Select(x => x.Name)
    .ToList();

// Break long parameter lists
public void Method(
    string parameter1,
    string parameter2,
    string parameter3)
{
    // Implementation
}
```

## Documentation

### XML Documentation
```csharp
/// <summary>
/// Executes the specified task asynchronously.
/// </summary>
/// <param name="input">The task input.</param>
/// <returns>A task representing the execution result.</returns>
/// <exception cref="ArgumentNullException">Thrown when input is null.</exception>
public async Task<Result> ExecuteAsync(string input)
{
    // Implementation
}
```

### Comments
```csharp
// Use comments to explain why, not what
public void Method()
{
    // Temporary workaround for issue #123
    // TODO: Remove after implementing proper fix
    var temp = Process();
}
```

## Best Practices

### Async/Await
```csharp
// Do: Use async/await consistently
public async Task<Result> ExecuteAsync()
{
    await Task.Delay(100);
    return result;
}

// Don't: Mix async styles
public Task<Result> ExecuteAsync()
{
    return Task.FromResult(result);
}
```

### Null Handling
```csharp
// Use null-conditional operator
var name = user?.Name;

// Use null-coalescing operator
var displayName = user?.Name ?? "Unknown";

// Use nullable reference types
public string? OptionalProperty { get; set; }
```

### Exception Handling
```csharp
try
{
    await ExecuteAsync();
}
catch (SpecificException ex)
{
    _logger.LogError(ex, "Specific error occurred");
    throw;
}
```

## XAML Style

### Element Formatting
```xaml
<Grid Margin="10">
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="*"/>
    </Grid.RowDefinitions>
    
    <TextBlock Text="{Binding Title}"
               FontSize="16"
               Margin="5"/>
</Grid>
```

### Resource Organization
```xaml
<Application.Resources>
    <!-- Colors -->
    <Color x:Key="PrimaryColor">#FF4A90E2</Color>
    
    <!-- Brushes -->
    <SolidColorBrush x:Key="PrimaryBrush" 
                     Color="{StaticResource PrimaryColor}"/>
    
    <!-- Styles -->
    <Style x:Key="HeaderStyle" TargetType="TextBlock">
        <Setter Property="FontSize" Value="24"/>
    </Style>
</Application.Resources>
```

## Related Documentation
- [Development Guidelines](./development-guidelines.md)
- [Testing Guide](./testing.md)
- [Architecture Overview](../architecture/system-architecture.md)
