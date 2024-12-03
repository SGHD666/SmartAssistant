// <copyright file="PythonScriptExecutor.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core.Services
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Service for executing Python scripts and managing Python dependencies.
    /// </summary>
    public class PythonScriptExecutor : IPythonScriptExecutor
    {
        private readonly string _pythonPath;
        private readonly string _scriptsDirectory;
        private readonly ILogger<PythonScriptExecutor> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PythonScriptExecutor"/> class.
        /// </summary>
        /// <param name="pythonPath">Path to the Python executable.</param>
        /// <param name="scriptsDirectory">Directory to store temporary Python scripts.</param>
        /// <param name="logger">Logger instance for logging execution details.</param>
        public PythonScriptExecutor(string pythonPath, string scriptsDirectory, ILogger<PythonScriptExecutor> logger)
        {
            this._pythonPath = pythonPath;
            this._scriptsDirectory = scriptsDirectory;
            this._logger = logger;

            // Ensure scripts directory exists
            Directory.CreateDirectory(this._scriptsDirectory);
        }

        /// <summary>
        /// Executes a Python script asynchronously.
        /// </summary>
        /// <param name="scriptContent">Content of the Python script to execute.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="Exception">Thrown when the Python script execution fails.</exception>
        public async Task ExecuteScriptAsync(string scriptContent)
        {
            var scriptPath = await this.SaveScriptToFileAsync(scriptContent);
            this._logger.LogDebug("Created temporary script at: {ScriptPath}", scriptPath);

            try
            {
                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = this._pythonPath,
                        Arguments = $"-u \"{scriptPath}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        WorkingDirectory = Path.GetDirectoryName(this._pythonPath),
                    },
                };

                process.StartInfo.Environment["PYTHONUNBUFFERED"] = "1";
                process.StartInfo.Environment["PYTHONIOENCODING"] = "utf-8";

                var output = string.Empty;
                var error = string.Empty;

                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        output += e.Data + Environment.NewLine;
                        this._logger.LogDebug("Python output: {Output}", e.Data);
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        error += e.Data + Environment.NewLine;
                        this._logger.LogWarning("Python error: {Error}", e.Data);
                    }
                };

                this._logger.LogDebug("Starting Python process with command: {Command} {Args}", this._pythonPath, process.StartInfo.Arguments);
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                using var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(30));
                try
                {
                    await process.WaitForExitAsync(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    process.Kill(true);
                    throw new TimeoutException("Python script execution timed out after 30 seconds");
                }

                if (process.ExitCode != 0)
                {
                    this._logger.LogError("Python script failed with exit code {ExitCode}: {Error}", process.ExitCode, error);
                    throw new Exception($"Python script execution failed: {error}");
                }

                if (!string.IsNullOrEmpty(error))
                {
                    this._logger.LogWarning("Python script completed with warnings: {Error}", error);
                }

                if (!string.IsNullOrEmpty(output))
                {
                    this._logger.LogDebug("Python script completed with output: {Output}", output);
                }
            }
            finally
            {
                try
                {
                    File.Delete(scriptPath);
                    this._logger.LogDebug("Deleted temporary script: {ScriptPath}", scriptPath);
                }
                catch (Exception ex)
                {
                    this._logger.LogWarning(ex, "Failed to delete temporary script: {ScriptPath}", scriptPath);
                }
            }
        }

        /// <summary>
        /// Executes a Python script asynchronously and returns the output.
        /// </summary>
        /// <param name="scriptContent">Content of the Python script to execute.</param>
        /// <returns>Output of the Python script.</returns>
        /// <exception cref="Exception">Thrown when the Python script execution fails.</exception>
        public async Task<string> ExecuteScriptWithOutputAsync(string scriptContent)
        {
            var scriptPath = await this.SaveScriptToFileAsync(scriptContent);
            this._logger.LogDebug("Created temporary script at: {ScriptPath}", scriptPath);

            try
            {
                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = this._pythonPath,
                        Arguments = $"-u \"{scriptPath}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        WorkingDirectory = Path.GetDirectoryName(this._pythonPath),
                    },
                };

                process.StartInfo.Environment["PYTHONUNBUFFERED"] = "1";
                process.StartInfo.Environment["PYTHONIOENCODING"] = "utf-8";

                var output = string.Empty;
                var error = string.Empty;

                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        output += e.Data + Environment.NewLine;
                        this._logger.LogDebug("Python output: {Output}", e.Data);
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        error += e.Data + Environment.NewLine;
                        this._logger.LogWarning("Python error: {Error}", e.Data);
                    }
                };

                this._logger.LogDebug("Starting Python process with command: {Command} {Args}", this._pythonPath, process.StartInfo.Arguments);
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                using var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(30));
                try
                {
                    await process.WaitForExitAsync(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    process.Kill(true);
                    throw new TimeoutException("Python script execution timed out after 30 seconds");
                }

                if (process.ExitCode != 0)
                {
                    this._logger.LogError("Python script failed with exit code {ExitCode}: {Error}", process.ExitCode, error);
                    throw new Exception($"Python script execution failed: {error}");
                }

                if (!string.IsNullOrEmpty(error))
                {
                    this._logger.LogWarning("Python script completed with warnings: {Error}", error);
                }

                return output;
            }
            finally
            {
                try
                {
                    File.Delete(scriptPath);
                    this._logger.LogDebug("Deleted temporary script: {ScriptPath}", scriptPath);
                }
                catch (Exception ex)
                {
                    this._logger.LogWarning(ex, "Failed to delete temporary script: {ScriptPath}", scriptPath);
                }
            }
        }

        /// <summary>
        /// Installs Python dependencies asynchronously.
        /// </summary>
        /// <param name="packages">Array of package names to install.</param>
        /// <exception cref="Exception">Thrown when the package installation fails.</exception>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InstallDependenciesAsync(string[] packages)
        {
            const int maxRetries = 3;
            const int retryDelayMs = 1000;

            foreach (var package in packages)
            {
                var retryCount = 0;
                while (retryCount < maxRetries)
                {
                    try
                    {
                        this._logger.LogDebug("Installing package: {Package} (Attempt {Attempt}/{MaxAttempts})", package, retryCount + 1, maxRetries);
                        using var process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = this._pythonPath,
                                Arguments = $"-m pip install --no-cache-dir --disable-pip-version-check {package}",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                CreateNoWindow = true,
                                WorkingDirectory = Path.GetDirectoryName(this._pythonPath),
                            },
                        };

                        // 添加必要的环境变量
                        process.StartInfo.Environment["PYTHONUNBUFFERED"] = "1";
                        process.StartInfo.Environment["PYTHONIOENCODING"] = "utf-8";
                        process.StartInfo.Environment["PIP_DISABLE_PIP_VERSION_CHECK"] = "1";

                        var output = string.Empty;
                        var error = string.Empty;

                        process.OutputDataReceived += (sender, e) =>
                        {
                            if (e.Data != null)
                            {
                                output += e.Data + Environment.NewLine;
                                this._logger.LogDebug("Pip output: {Output}", e.Data);
                            }
                        };

                        process.ErrorDataReceived += (sender, e) =>
                        {
                            if (e.Data != null)
                            {
                                error += e.Data + Environment.NewLine;
                                this._logger.LogWarning("Pip error: {Error}", e.Data);
                            }
                        };

                        process.Start();
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        // 添加超时机制
                        using var cts = new CancellationTokenSource();
                        cts.CancelAfter(TimeSpan.FromMinutes(5)); // pip 安装可能需要更长时间
                        try
                        {
                            await process.WaitForExitAsync(cts.Token);
                        }
                        catch (OperationCanceledException)
                        {
                            process.Kill(true);
                            throw new TimeoutException($"Package installation timed out after 5 minutes: {package}");
                        }

                        if (process.ExitCode != 0)
                        {
                            this._logger.LogError("Package installation failed with exit code {ExitCode}: {Error}", process.ExitCode, error);
                            throw new Exception($"Package installation failed: {error}");
                        }

                        // 如果安装成功，跳出重试循环
                        this._logger.LogInformation("Successfully installed package: {Package}", package);
                        break;
                    }
                    catch (Exception ex)
                    {
                        retryCount++;
                        this._logger.LogWarning(ex, "Failed to install package: {Package} (Attempt {Attempt}/{MaxAttempts})", package, retryCount, maxRetries);

                        if (retryCount >= maxRetries)
                        {
                            this._logger.LogError(ex, "Failed to install package after {MaxAttempts} attempts: {Package}", maxRetries, package);
                            throw;
                        }

                        await Task.Delay(retryDelayMs * retryCount);
                    }
                }
            }
        }

        /// <summary>
        /// Saves a Python script to a temporary file asynchronously.
        /// </summary>
        /// <param name="scriptContent">Content of the Python script to save.</param>
        /// <returns>Path to the temporary script file.</returns>
        private async Task<string> SaveScriptToFileAsync(string scriptContent)
        {
            var scriptPath = Path.Combine(this._scriptsDirectory, $"{Guid.NewGuid()}.py");
            await File.WriteAllTextAsync(scriptPath, scriptContent);
            return scriptPath;
        }
    }
}
