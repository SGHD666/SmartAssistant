# Design Patterns | 设计模式

This document outlines the key design patterns used in SmartAssistant and their implementation.

本文档概述了 SmartAssistant 中使用的关键设计模式及其实现。

## MVVM Pattern | MVVM 模式

### Overview | 概述
The Model-View-ViewModel pattern is central to our UI architecture.

MVVM（模型-视图-视图模型）模式是我们 UI 架构的核心。

### Implementation | 实现
```csharp
// ViewModel Example | ViewModel 示例
public class MainWindowViewModel : ViewModelBase
{
    private string _input;
    public string Input 
    {
        get => _input;
        set => this.RaiseAndSetIfChanged(ref _input, value);
    }
}
```

### Usage Guidelines | 使用指南
- ViewModels should inherit from `ViewModelBase`
  ViewModel 应继承自 `ViewModelBase`
- Use observable properties for UI binding
  使用可观察属性进行 UI 绑定
- Implement commands for user actions
  为用户操作实现命令

## Dependency Injection | 依赖注入

### Overview | 概述
We use Microsoft.Extensions.DependencyInjection for IoC container.

我们使用 Microsoft.Extensions.DependencyInjection 作为 IoC 容器。

### Registration | 注册
```csharp
services.AddSingleton<ITaskExecutionService, TaskExecutionService>();
services.AddScoped<ILLMService, OpenAIService>();
services.AddTransient<IAutomationService, AutomationService>();
```

### Best Practices | 最佳实践
- Register interfaces, not concrete types
  注册接口而不是具体类型
- Use appropriate lifetimes (Singleton, Scoped, Transient)
  使用适当的生命周期（单例、作用域、瞬态）
- Avoid service locator pattern
  避免使用服务定位器模式

## Repository Pattern | 仓储模式

### Overview | 概述
Used for data access abstraction and centralization.

用于数据访问抽象和集中化。

### Implementation | 实现
```csharp
public interface IRepository<T>
{
    Task<T> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(string id);
}
```

### Usage | 使用方法
- Create specific repositories for each entity type
  为每个实体类型创建特定的仓储
- Implement caching where appropriate
  在适当的地方实现缓存
- Use unit of work pattern for transactions
  使用工作单元模式处理事务

## Command Pattern | 命令模式

### Overview | 概述
Encapsulates operations as objects for execution, undo/redo support.

将操作封装为对象以支持执行、撤销/重做功能。

### Implementation | 实现
```csharp
public interface ICommand
{
    Task ExecuteAsync();
    Task UndoAsync();
    bool CanExecute();
}
```

### Use Cases | 使用场景
- User interface actions
  用户界面操作
- Task execution system
  任务执行系统
- Operation history management
  操作历史管理

## Observer Pattern | 观察者模式

### Overview | 概述
Used for event handling and state management.

用于事件处理和状态管理。

### Implementation | 实现
```csharp
public interface IObservable<T>
{
    IDisposable Subscribe(IObserver<T> observer);
}
```

### Applications | 应用场景
- Event handling system
  事件处理系统
- State change notifications
  状态变更通知
- Real-time updates
  实时更新

## Factory Pattern | 工厂模式

### Overview | 概述
Creates objects without exposing instantiation logic.

创建对象而不暴露实例化逻辑。

### Implementation | 实现
```csharp
public interface ILLMServiceFactory
{
    ILLMService Create(LLMType type);
}
```

### Usage | 使用方法
- LLM service creation
  LLM 服务创建
- Command instantiation
  命令实例化
- View model creation
  视图模型创建

## Strategy Pattern | 策略模式

### Overview | 概述
Defines a family of algorithms and makes them interchangeable.

定义一组算法并使它们可互换。

### Implementation | 实现
```csharp
public interface ITaskStrategy
{
    Task<ExecutionResult> ExecuteAsync(TaskContext context);
}
```

### Applications | 应用场景
- Task execution strategies
  任务执行策略
- LLM provider selection
  LLM 提供者选择
- UI rendering approaches
  UI 渲染方法

## Decorator Pattern | 装饰者模式

### Overview | 概述
Adds behavior to objects dynamically.

动态地为对象添加行为。

### Implementation | 实现
```csharp
public class LoggingServiceDecorator : IService
{
    private readonly IService _service;
    private readonly ILogger _logger;

    public LoggingServiceDecorator(IService service, ILogger logger)
    {
        _service = service;
        _logger = logger;
    }
}
```

### Use Cases | 使用场景
- Logging
  日志记录
- Caching
  缓存
- Validation
  验证

## Best Practices | 最佳实践

1. **Pattern Selection**
   - Choose patterns based on specific needs
   - Avoid over-engineering
   - Consider maintenance implications
   选择模式基于具体需求
   避免过度工程化
   考虑维护影响

2. **Implementation**
   - Follow SOLID principles
   - Keep implementations simple
   - Document pattern usage
   遵循 SOLID 原则
   保持实现简单
   文档化模式使用

3. **Testing**
   - Unit test each pattern implementation
   - Mock dependencies appropriately
   - Test edge cases
   单元测试每个模式实现
   适当地模拟依赖
   测试边缘情况

## Related Documentation | 相关文档

- [System Architecture](./system-architecture.md)
- [Development Guidelines](../guides/development-guidelines.md)
- [Code Style Guide](../guides/code-style.md)
