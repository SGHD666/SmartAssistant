// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using SmartAssistant.Core.Controllers;

    /// <summary>
    /// The main program class for the SmartAssistant Core application.
    /// This class contains the entry point for the application and sets up the core services.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main entry point for the SmartAssistant Core application.
        /// Configures and initializes the application's services and dependencies.
        /// </summary>
        /// <param name="args">Command-line arguments passed to the application.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation of the application.</returns>
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var services = new ServiceCollection();
            services.AddSmartAssistantCore(configuration);
            // 添加日志服务
            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddDebug();  // 添加 Debug 输出
                logging.AddConsole(); // 添加控制台输出
                logging.SetMinimumLevel(LogLevel.Debug); // 设置最小日志级别为 Debug
            });
            var serviceProvider = services.BuildServiceProvider();

            try
            {
                var assistant = serviceProvider.GetRequiredService<AssistantController>();
                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("智能助手已准备就绪。输入 'exit' 退出。");
                Console.WriteLine("智能助手已准备就绪。输入 'exit' 退出。");
                while (true)
                {
                    Console.Write("\n请输入您的指令:");
                    var input = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        continue;
                    }
                    if (input.ToLower() == "exit") 
                    {
                        break;
                    }

                    logger.LogDebug("收到用户指令: {Input}", input);
                    var response = await assistant.ProcessUserInputAsync(input);
                    logger.LogDebug("助手响应: {Response}", response);
                    Console.WriteLine($"\n助手: {response}");
                }
            }
            catch (Exception ex)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "应用程序发生错误");
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
