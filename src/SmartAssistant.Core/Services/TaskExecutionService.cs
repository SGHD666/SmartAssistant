// <copyright file="TaskExecutionService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core.Services
{
    using System.Collections.Concurrent;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using SmartAssistant.Core.Models;
    using SmartAssistant.Core.Services.LLM;
    using Newtonsoft.Json;

    /// <summary>
    /// Service responsible for managing and executing tasks within the SmartAssistant system.
    /// Handles task lifecycle, execution, and history tracking.
    /// </summary>
    public class TaskExecutionService : ITaskExecutionService
    {
        private readonly ConcurrentDictionary<string, TaskDefinition> _activeTasks;
        private readonly ConcurrentDictionary<string, TaskDefinition> _taskHistory;
        private readonly ILanguageModelService _languageModel;
        private readonly IAutomationService _automationService;
        private readonly ILogger<TaskExecutionService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskExecutionService"/> class.
        /// </summary>
        /// <param name="languageModel">The language model service for task processing.</param>
        /// <param name="automationService">The automation service for executing tasks.</param>
        /// <param name="logger">The logger for recording service operations.</param>
        public TaskExecutionService(
            ILanguageModelService languageModel,
            IAutomationService automationService,
            ILogger<TaskExecutionService> logger)
        {
            this._languageModel = languageModel;
            this._automationService = automationService;
            this._logger = logger;
            this._activeTasks = new ConcurrentDictionary<string, TaskDefinition>();
            this._taskHistory = new ConcurrentDictionary<string, TaskDefinition>();
        }

        /// <summary>
        /// Executes a task based on the provided task description.
        /// </summary>
        /// <param name="taskDescription">The description of the task to execute.</param>
        /// <returns>A boolean indicating whether the task was executed successfully.</returns>
        public async Task<bool> ExecuteTaskAsync(string taskDescription)
        {
            this._logger.LogInformation("Starting task execution: {Description}", taskDescription);

            // 检查是否是命令格式
            if (taskDescription.StartsWith("COMMAND:", StringComparison.OrdinalIgnoreCase))
            {
                var command = taskDescription.Substring("COMMAND:".Length).Trim();
                return await this.ExecuteCommandAsync(command);
            }

            try
            {
                // 确定任务类型
                var taskType = await this.DetermineTaskType(taskDescription);
                var taskId = Guid.NewGuid().ToString();

                var taskDef = new TaskDefinition
                {
                    Id = taskId,
                    Description = taskDescription,
                    Type = taskType,
                    Status = "Running",
                    StartedAt = DateTime.UtcNow,
                };

                this._activeTasks.TryAdd(taskId, taskDef);

                // 根据任务类型执行相应的操作
                bool success = false;
                switch (taskType.ToLower())
                {
                    case "browser":
                        success = await this._automationService.ExecuteBrowserTaskAsync(taskDescription);
                        break;
                    case "system":
                        success = await this._automationService.ExecuteSystemTaskAsync(taskDescription);
                        break;
                    case "file":
                        success = await this._automationService.ExecuteFileTaskAsync(taskDescription);
                        break;
                    default:
                        this._logger.LogWarning("Unknown task type: {Type}", taskType);
                        success = false;
                        break;
                }

                if (success)
                {
                    taskDef.Status = "Completed";
                    taskDef.CompletedAt = DateTime.UtcNow;
                    taskDef.Result = "Task completed successfully";

                    if (this._activeTasks.TryRemove(taskDef.Id, out _))
                    {
                        this._taskHistory.TryAdd(taskDef.Id, taskDef);
                    }
                }
                else
                {
                    taskDef.Status = "Failed";
                    taskDef.CompletedAt = DateTime.UtcNow;
                    taskDef.Result = "Task execution failed";
                }

                return success;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error executing task: {Description}", taskDescription);
                return false;
            }
        }

        /// <summary>
        /// Executes a command based on the provided command string.
        /// </summary>
        /// <param name="command">The command string to execute.</param>
        /// <returns>A boolean indicating whether the command was executed successfully.</returns>
        private async Task<bool> ExecuteCommandAsync(string command)
        {
            try
            {
                this._logger.LogInformation("Analyzing command intent: {Command}", command);

                // 使用语言模型分析用户意图
                var taskAnalysis = await this._languageModel.AnalyzeTaskAsync(command);
                if (taskAnalysis == null || !taskAnalysis.Any())
                {
                    this._logger.LogWarning("Language model could not analyze the command: {Command}", command);
                    return false;
                }

                // 执行拆分后的每个子任务
                bool overallSuccess = true;
                foreach (var subtask in taskAnalysis)
                {
                    this._logger.LogInformation("Executing subtask: {SubtaskDescription}", subtask);
                    var success = await ExecuteTaskAsync(subtask);
                    
                    if (!success)
                    {
                        this._logger.LogWarning("Subtask failed: {SubtaskDescription}", subtask);
                        overallSuccess = false;
                    }
                }

                return overallSuccess;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error executing command: {Command}", command);
                return false;
            }
        }

        /// <summary>
        /// Determines the type of task based on the provided task description.
        /// </summary>
        /// <param name="taskDescription">The description of the task to determine the type for.</param>
        /// <returns>The type of task as a string.</returns>
        private async Task<string> DetermineTaskType(string taskDescription)
        {
            try
            {
                var prompt = @"请分析以下任务，并返回任务类型。
                                如果任务涉及浏览器操作（如打开网站），返回 browser。
                                如果任务涉及系统操作（如调整音量、亮度），返回 system。
                                如果任务涉及文件操作，返回 file。
                                任务: " + taskDescription;

                var response = await this._languageModel.GenerateResponseAsync(prompt);
                var taskType = response.Trim().ToLower();

                this._logger.LogDebug("Task type determined: {Type} for task: {Description}", taskType, taskDescription);

                return taskType;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error determining task type for: {Description}", taskDescription);
                throw;
            }
        }

        /// <summary>
        /// Checks if the task description contains browser-related keywords.
        /// </summary>
        /// <param name="task">The task description to check.</param>
        /// <returns>A boolean indicating whether the task description contains browser-related keywords.</returns>
        private bool ContainsBrowserKeywords(string task)
        {
            var keywords = new[] { "browser", "website", "url", "web", "chrome", "firefox", "edge" };
            return keywords.Any(k => task.Contains(k, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Checks if the task description contains system-related keywords.
        /// </summary>
        /// <param name="task">The task description to check.</param>
        /// <returns>A boolean indicating whether the task description contains system-related keywords.</returns>
        private bool ContainsSystemKeywords(string task)
        {
            var keywords = new[] { "volume", "brightness", "system", "settings", "audio", "sound" };
            return keywords.Any(k => task.Contains(k, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Checks if the task description contains file-related keywords.
        /// </summary>
        /// <param name="task">The task description to check.</param>
        /// <returns>A boolean indicating whether the task description contains file-related keywords.</returns>
        private bool ContainsFileKeywords(string task)
        {
            var keywords = new[] { "file", "folder", "directory", "copy", "move", "delete", "create" };
            return keywords.Any(k => task.Contains(k, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Retrieves the status of a task based on the provided task ID.
        /// </summary>
        /// <param name="taskId">The ID of the task to retrieve the status for.</param>
        /// <returns>The status of the task as a string.</returns>
        public Task<string> GetTaskStatusAsync(string taskId)
        {
            this._logger.LogDebug("Getting status for task {TaskId}", taskId);

            if (this._activeTasks.TryGetValue(taskId, out var activeTask))
            {
                return Task.FromResult(activeTask.Status);
            }

            if (this._taskHistory.TryGetValue(taskId, out var historicTask))
            {
                return Task.FromResult(historicTask.Status);
            }

            return Task.FromResult("NotFound");
        }

        /// <summary>
        /// Retrieves the history of tasks.
        /// </summary>
        /// <returns>The history of tasks as a JSON string.</returns>
        public Task<string> GetTaskHistoryAsync()
        {
            var history = this._taskHistory.Values.ToList();
            return Task.FromResult(JsonConvert.SerializeObject(history, Formatting.Indented));
        }

        /// <summary>
        /// Cancels a task based on the provided task ID.
        /// </summary>
        /// <param name="taskId">The ID of the task to cancel.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task CancelTaskAsync(string taskId)
        {
            if (this._activeTasks.TryGetValue(taskId, out var task))
            {
                task.Status = "Cancelled";
                task.CompletedAt = DateTime.UtcNow;

                if (this._activeTasks.TryRemove(taskId, out _))
                {
                    this._taskHistory.TryAdd(taskId, task);
                }

                this._logger.LogInformation("Task {TaskId} cancelled", taskId);
            }
            else
            {
                this._logger.LogWarning("Task {TaskId} not found for cancellation", taskId);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Retrieves the active tasks.
        /// </summary>
        /// <returns>A collection of active tasks.</returns>
        private IEnumerable<TaskDefinition> GetActiveTasksInternal()
        {
            return this._activeTasks.Values;
        }
    }
}
