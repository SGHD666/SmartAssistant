// <copyright file="SmartAssistantCoreExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using SmartAssistant.Core.Controllers;
    using SmartAssistant.Core.Models;
    using SmartAssistant.Core.Services;
    using SmartAssistant.Core.Services.LLM;

/// <summary>
/// SmartAssistantCoreExtensions.
/// </summary>
    public static class SmartAssistantCoreExtensions
    {
        /// <summary>
        /// Adds the SmartAssistant core services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddSmartAssistantCore(this IServiceCollection services, IConfiguration configuration)
        {
            // 配置服务
            services.Configure<ModelSettings>(configuration.GetSection("SmartAssistant"));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<ModelSettings>>().Value);

            // 注册服务
            services.AddSingleton<ModelManager>();
            services.AddSingleton<ILLMFactory, LLMFactory>();
            services.AddSingleton<ITaskExecutionService, TaskExecutionService>();

            // Register language model services
            services.AddSingleton<OpenAIService>();
            services.AddSingleton<ClaudeService>();
            services.AddSingleton<QianWenService>();
            services.AddSingleton<ILanguageModelService, OpenAIService>();
            services.AddSingleton<ILanguageModelService, ClaudeService>();
            services.AddSingleton<ILanguageModelService, QianWenService>();

            services.AddSingleton<IAutomationService, AutomationService>();

            // Python 脚本执行器
            var pythonPath = configuration.GetValue<string>("PythonPath") ?? "python";
            var scriptsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts");
            services.AddSingleton<IPythonScriptExecutor>(sp => 
                new PythonScriptExecutor(pythonPath, scriptsDirectory, sp.GetRequiredService<ILogger<PythonScriptExecutor>>()));

            // 控制器
            services.AddSingleton<AssistantController>();

            return services;
        }
    }
}
