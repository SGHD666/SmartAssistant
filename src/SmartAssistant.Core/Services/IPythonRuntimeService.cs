// <copyright file="IPythonRuntimeService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core.Services
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for Python.NET runtime service.
    /// </summary>
    public interface IPythonRuntimeService
    {
        /// <summary>
        /// Ensures Python.NET runtime is initialized.
        /// </summary>
        void EnsureInitialized();

        /// <summary>
        /// Executes Python code using Python.NET runtime.
        /// </summary>
        /// <param name="code">Python code to execute.</param>
        /// <returns>Result of the execution.</returns>
        Task<string> ExecuteCodeAsync(string code);

        /// <summary>
        /// Finalizes the Python.NET runtime.
        /// </summary>
        void Shutdown();
    }
}
