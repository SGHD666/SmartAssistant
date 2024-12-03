// <copyright file="IAutomationService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core.Services
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface defining automation services for executing various types of tasks.
    /// </summary>
    public interface IAutomationService
    {
        /// <summary>
        /// Executes browser-related tasks such as opening websites or navigating URLs.
        /// </summary>
        /// <param name="taskDescription">The description of the browser task to execute.</param>
        /// <returns>A task representing the asynchronous operation, returning true if successful.</returns>
        Task<bool> ExecuteBrowserTaskAsync(string taskDescription);

        /// <summary>
        /// Executes system-related tasks such as adjusting volume or brightness.
        /// </summary>
        /// <param name="taskDescription">The description of the system task to execute.</param>
        /// <returns>A task representing the asynchronous operation, returning true if successful.</returns>
        Task<bool> ExecuteSystemTaskAsync(string taskDescription);

        /// <summary>
        /// Executes file-related tasks such as copying, moving, or deleting files.
        /// </summary>
        /// <param name="taskDescription">The description of the file task to execute.</param>
        /// <returns>A task representing the asynchronous operation, returning true if successful.</returns>
        Task<bool> ExecuteFileTaskAsync(string taskDescription);

        /// <summary>
        /// Validates a task description.
        /// </summary>
        /// <param name="taskDescription">The task description to validate.</param>
        /// <returns>True if the task description is valid, false otherwise.</returns>
        bool ValidateTask(string taskDescription);
    }
}
