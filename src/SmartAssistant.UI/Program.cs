using Avalonia;
using Avalonia.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Avalonia.Controls.ApplicationLifetimes;

namespace SmartAssistant.UI
{
    internal class Program
    {
        private static ILogger<Program>? _logger;

        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                // 创建配置
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                // 从配置文件获取日志设置
                var logPath = configuration.GetValue<string>("Logging:File:Path") ??
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "smartassistant.log");
                var fileSizeLimit = configuration.GetValue<long?>("Logging:File:FileSizeLimitBytes") ?? (10L * 1024 * 1024);
                var maxRollingFiles = configuration.GetValue("Logging:File:MaxRollingFiles", 7);

                // 确保日志目录存在
                Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

                // 配置 Serilog
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration) // 从配置文件读取基本设置
                    .Enrich.FromLogContext()
                    .Enrich.WithThreadId()
                    .Enrich.WithEnvironmentName()
                    .WriteTo.Console()
                    .WriteTo.File(logPath,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: maxRollingFiles,
                        fileSizeLimitBytes: fileSizeLimit,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger();

                // 创建日志工厂
                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddSerilog(Log.Logger);
                });
                _logger = loggerFactory.CreateLogger<Program>();

                _logger.LogInformation("Application starting at {Time}", DateTime.Now);
                _logger.LogInformation("Log file created at: {LogPath}", logPath);

                _logger.LogDebug("Building Avalonia app...");
                var builder = BuildAvaloniaApp();
                
                _logger.LogDebug("Starting with classic desktop lifetime...");
                builder.StartWithClassicDesktopLifetime(args);

                // 注册应用程序退出事件
                AppDomain.CurrentDomain.ProcessExit += (s, e) =>
                {
                    _logger?.LogInformation("Application shutting down...");
                    Log.CloseAndFlush();
                };

                // 注册主窗口关闭事件
               if (Application.Current is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    desktop.Exit += (s, e) =>
                    {
                        _logger?.LogInformation("Main window closed, shutting down application...");
                        Log.CloseAndFlush();
                        desktop.Shutdown();
                    };
                }
                
                _logger.LogInformation("Application started successfully");
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Application error");
                    if (ex.InnerException != null)
                    {
                        _logger.LogError(ex.InnerException, "Inner exception details");
                    }
                }
                Log.CloseAndFlush();
                throw;
            }
        }

        public static AppBuilder BuildAvaloniaApp()
        {
            _logger?.LogDebug("Configuring AppBuilder...");
            try
            {
                _logger?.LogDebug("Building Avalonia app...");
                return AppBuilder.Configure<App>()
                    .UsePlatformDetect()
                    .LogToTrace();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error configuring AppBuilder");
                throw;
            }
        }
    }
}
