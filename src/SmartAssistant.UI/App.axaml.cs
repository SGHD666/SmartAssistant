// <copyright file="App.axaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.UI;

using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartAssistant.Core;
using SmartAssistant.UI.ViewModels;
using SmartAssistant.UI.Views;
using SmartAssistant.UI.Common;
using SmartAssistant.Core.Services;

public partial class App : Application
{
    private System.IServiceProvider? _serviceProvider;
    private readonly ILogger<App> _logger;

    public App()
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
        });
        _logger = loggerFactory.CreateLogger<App>();
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        
        var services = new ServiceCollection();
        
        // 加载配置
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        _logger.LogInformation("Current BaseDirectory: {BasePath}", basePath);
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        // 验证配置
        var smartAssistantSection = configuration.GetSection("SmartAssistant");
        if (!smartAssistantSection.Exists())
        {
            var configPath = Path.Combine(basePath, "appsettings.json");
            _logger.LogError("SmartAssistant configuration not found at: {ConfigPath}", configPath);
            throw new InvalidOperationException(
                $"SmartAssistant configuration section not found in {configPath}");
        }

        _logger.LogInformation("Configuration loaded successfully");
        ConfigureServices(services, configuration);
        _serviceProvider = services.BuildServiceProvider();

        // 预初始化核心服务
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var modelManager = scope.ServiceProvider.GetRequiredService<SmartAssistant.Core.Services.LLM.ModelManager>();
            _logger.LogInformation("Core services pre-initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to pre-initialize core services");
            throw;
        }
    }

    private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // 添加日志
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
            builder.AddConfiguration(configuration.GetSection("Logging"));
        });

        // 添加配置
        services.AddSingleton<IConfiguration>(configuration);

        // 添加Python运行时服务（确保最先初始化）
        var pythonPath = configuration.GetValue<string>("SmartAssistant:PythonPath") ?? "c:\\python39\\python.exe";
        services.AddSingleton<IPythonRuntimeService>(sp => 
            new PythonRuntimeService(pythonPath, sp.GetRequiredService<ILogger<PythonRuntimeService>>()));

        // 预初始化Python运行时
        var serviceProvider = services.BuildServiceProvider();
        try
        {
            var pythonService = serviceProvider.GetRequiredService<IPythonRuntimeService>();
            pythonService.EnsureInitialized();
            _logger.LogInformation("Python runtime initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Python runtime");
            throw;
        }

        // 添加所有其他 SmartAssistant 服务
        services.AddSmartAssistantCore(configuration);

        // 添加视图模型和窗口
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<MainWindow>();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        _logger.LogDebug("Framework initialization completed");
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _logger.LogInformation("Creating main window...");
            try 
            {
                // 使用 GetRequiredService 而不是 GetService，这样如果服务未注册会立即抛出异常
                var mainWindow = _serviceProvider?.GetRequiredService<MainWindow>()
                    ?? throw new InvalidOperationException("Service provider is not initialized");
                
                var viewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();
                
                mainWindow.DataContext = viewModel;
                _logger.LogDebug("MainWindow DataContext set to MainWindowViewModel");

                desktop.MainWindow = mainWindow;
                _logger.LogInformation("Main window created and set as desktop.MainWindow");

                // 注册应用程序退出事件
                desktop.Exit += (sender, args) =>
                {
                    try
                    {
                        var pythonService = _serviceProvider?.GetService<IPythonRuntimeService>();
                        if (pythonService != null)
                        {
                            pythonService.Shutdown();
                            _logger.LogInformation("Python runtime shut down successfully");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error shutting down Python runtime");
                    }
                };

                // 显示窗口
                mainWindow.Show();
                _logger.LogInformation("Main window shown");

                // 在主线程上初始化服务，以避免线程问题
                Dispatcher.UIThread.Post(async () =>
                {
                    try
                    {
                        await viewModel.InitializeAsync();
                        _logger.LogInformation("ViewModel initialization completed successfully");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error during ViewModel initialization");
                        // 在UI线程上显示错误消息
                        await Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            viewModel.Messages.Add(new ChatMessage
                            {
                                Content = $"初始化失败: {ex.Message}",
                                Type = Common.MessageType.Error,
                                Timestamp = DateTime.Now
                            });
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize application");
                throw;
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}
