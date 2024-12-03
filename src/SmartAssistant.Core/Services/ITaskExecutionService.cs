// <copyright file="ITaskExecutionService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core.Services
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for managing and executing tasks in the smart assistant.
    /// </summary>
    public interface ITaskExecutionService
    {
        /// <summary>
        /// Executes a task based on the provided description.
        /// </summary>
        /// <param name="taskDescription">The description of the task to execute.</param>
        /// <returns>A task representing the asynchronous operation, with a boolean indicating success.</returns>
        Task<bool> ExecuteTaskAsync(string taskDescription);

        /// <summary>
        /// Gets the current status of a task.
        /// </summary>
        /// <param name="taskId">The unique identifier of the task.</param>
        /// <returns>A task representing the asynchronous operation, containing the task status.</returns>
        Task<string> GetTaskStatusAsync(string taskId);

        /// <summary>
        /// Cancels a running task.
        /// </summary>
        /// <param name="taskId">The unique identifier of the task to cancel.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task CancelTaskAsync(string taskId);

        /// <summary>
        /// Retrieves the execution history of all tasks.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing the task history.</returns>
        Task<string> GetTaskHistoryAsync();
    }
}
