// <copyright file="TaskDefinition.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core.Models
{
    using System;

    /// <summary>
    /// Defines a task to be executed by the smart assistant.
    /// Contains information about the task's status, execution details, and results.
    /// </summary>
    public class TaskDefinition
    {
        /// <summary>
        /// Gets or sets the unique identifier for the task.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the description of the task to be executed.
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// Gets or sets the type of the task (e.g., browser, system, file).
        /// </summary>
        public required string Type { get; set; }

        /// <summary>
        /// Gets or sets the current status of the task (e.g., pending, running, completed).
        /// </summary>
        public required string Status { get; set; }

        /// <summary>
        /// Gets or sets the UTC timestamp when the task was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the UTC timestamp when the task was started.
        /// </summary>
        public DateTime? StartedAt { get; set; }

        /// <summary>
        /// Gets or sets the UTC timestamp when the task was completed.
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// Gets or sets the result message of the task execution.
        /// </summary>
        public string Result { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the error message if the task execution failed.
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
