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

    /// <summary>
    /// Service responsible for managing and executing tasks within the SmartAssistant system.
    /// Handles task lifecycle, execution, and history tracking.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="TaskExecutionService"/> class.
    /// </remarks>
    /// <param name="languageModel">The language model service for task processing.</param>
    /// <param name="automationService">The automation service for executing tasks.</param>
    /// <param name="logger">The logger for recording service operations.</param>
    public class TaskExecutionService(
        ILanguageModelService languageModel,
        IAutomationService automationService,
        ILogger<TaskExecutionService> logger) : ITaskExecutionService
    {
        private readonly ConcurrentDictionary<string, TaskDefinition> activeTasks = new();
        private readonly ConcurrentDictionary<string, TaskDefinition> taskHistory = new();

        /// <summary>
        /// Executes a task based on the provided task description.
        /// </summary>
        /// <param name="taskDescription">The description of the task to execute.</param>
        /// <returns>A boolean indicating whether the task was executed successfully.</returns>
        public async Task<bool> ExecuteTaskAsync(string taskDescription)
        {
            logger.LogInformation("Starting task execution: {Description}", taskDescription);

            // Check if it's a command format
            if (taskDescription.StartsWith("COMMAND:", StringComparison.OrdinalIgnoreCase))
            {
                var command = taskDescription["COMMAND:".Length..].Trim();
                return await this.ExecuteCommandAsync(command);
            }

            try
            {
                // Determine task type
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

                this.activeTasks.TryAdd(taskId, taskDef);

                // Execute operations based on task type
                bool success = false;
                switch (taskType.ToLower())
                {
                    case "browser":
                        success = await automationService.ExecuteBrowserTaskAsync(taskDescription);
                        break;
                    case "system":
                        success = await automationService.ExecuteSystemTaskAsync(taskDescription);
                        break;
                    case "file":
                        success = await automationService.ExecuteFileTaskAsync(taskDescription);
                        break;
                    default:
                        logger.LogWarning("Unknown task type: {Type}", taskType);
                        success = false;
                        break;
                }

                if (success)
                {
                    taskDef.Status = "Completed";
                    taskDef.CompletedAt = DateTime.UtcNow;
                    taskDef.Result = "Task completed successfully";

                    if (this.activeTasks.TryRemove(taskDef.Id, out _))
                    {
                        this.taskHistory.TryAdd(taskDef.Id, taskDef);
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
                logger.LogError(ex, "Error executing task: {Description}", taskDescription);
                return false;
            }
        }

        /// <summary>
        /// Executes a command based on the provided command string.
        /// </summary>
        /// <param name="command">The command string to execute.</param>
        /// <returns>A boolean indicating whether the command was executed successfully.</returns>
        public async Task<bool> ExecuteCommandAsync(string command)
        {
            try
            {
                logger.LogInformation("Analyzing command intent: {Command}", command);

                // Use language model to analyze user intent
                var taskAnalysis = await languageModel.AnalyzeTaskAsync(command);
                if (taskAnalysis == null || !taskAnalysis.Any())
                {
                    logger.LogWarning("Language model could not analyze the command: {Command}", command);
                    return false;
                }

                // Execute each subtask
                bool overallSuccess = true;
                foreach (var subtask in taskAnalysis)
                {
                    logger.LogInformation("Executing subtask: {SubtaskDescription}", subtask);
                    var success = await this.ExecuteTaskAsync(subtask);

                    if (!success)
                    {
                        logger.LogWarning("Subtask failed: {SubtaskDescription}", subtask);
                        overallSuccess = false;
                    }
                }

                return overallSuccess;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing command: {Command}", command);
                return false;
            }
        }

        /// <summary>
        /// Determines the type of task based on the provided task description.
        /// </summary>
        /// <param name="taskDescription">The description of the task to determine the type for.</param>
        /// <returns>The type of task as a string.</returns>
        public async Task<string> DetermineTaskType(string taskDescription)
        {
            try
            {
                var prompt = @"Please analyze the following task and return the task type.
                                If the task involves browser operations (such as opening a website), return browser.
                                If the task involves system operations (such as adjusting volume or brightness), return system.
                                If the task involves file operations, return file.
                                Task: " + taskDescription;

                var response = await languageModel.GenerateResponseAsync(prompt);
                var taskType = response.Trim().ToLower();

                logger.LogDebug("Task type determined: {Type} for task: {Description}", taskType, taskDescription);

                return taskType;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error determining task type for: {Description}", taskDescription);
                throw;
            }
        }

        /// <summary>
        /// Retrieves the status of a task based on the provided task ID.
        /// </summary>
        /// <param name="taskId">The ID of the task to retrieve the status for.</param>
        /// <returns>The status of the task as a string.</returns>
        public Task<string> GetTaskStatusAsync(string taskId)
        {
            logger.LogDebug("Getting status for task {TaskId}", taskId);

            if (this.activeTasks.TryGetValue(taskId, out var activeTask))
            {
                return Task.FromResult(activeTask.Status);
            }

            if (this.taskHistory.TryGetValue(taskId, out var historicTask))
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
            var history = this.taskHistory.Values.ToList();
            return Task.FromResult(JsonConvert.SerializeObject(history, Formatting.Indented));
        }

        /// <summary>
        /// Cancels a task based on the provided task ID.
        /// </summary>
        /// <param name="taskId">The ID of the task to cancel.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task CancelTaskAsync(string taskId)
        {
            if (this.activeTasks.TryGetValue(taskId, out var task))
            {
                task.Status = "Cancelled";
                task.CompletedAt = DateTime.UtcNow;

                if (this.activeTasks.TryRemove(taskId, out _))
                {
                    this.taskHistory.TryAdd(taskId, task);
                }

                logger.LogInformation("Task {TaskId} cancelled", taskId);
            }
            else
            {
                logger.LogWarning("Task {TaskId} not found for cancellation", taskId);
            }

            return Task.CompletedTask;
        }
    }
}
