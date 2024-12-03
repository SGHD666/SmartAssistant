// <copyright file="IPythonScriptExecutor.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core.Services
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for executing Python scripts and managing Python dependencies.
    /// </summary>
    public interface IPythonScriptExecutor
    {
        /// <summary>
        /// Executes a Python script asynchronously.
        /// </summary>
        /// <param name="scriptContent">The content of the Python script to execute.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ExecuteScriptAsync(string scriptContent);

        /// <summary>
        /// Executes a Python script and returns its output asynchronously.
        /// </summary>
        /// <param name="scriptContent">The content of the Python script to execute.</param>
        /// <returns>A task representing the asynchronous operation, containing the script's output.</returns>
        Task<string> ExecuteScriptWithOutputAsync(string scriptContent);

        /// <summary>
        /// Installs Python package dependencies using pip.
        /// </summary>
        /// <param name="packages">Array of package names to install.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task InstallDependenciesAsync(string[] packages);
    }
}
