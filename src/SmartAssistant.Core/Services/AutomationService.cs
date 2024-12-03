// <copyright file="AutomationService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core.Services
{
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using Python.Runtime;
    using SmartAssistant.Core.Models;
    using SmartAssistant.Core.Services.LLM;

    /// <summary>
    /// Service for handling automation tasks including browser and system operations.
    /// </summary>
    public class AutomationService : IAutomationService
    {
        private readonly ILogger<AutomationService> _logger;
        private readonly AppSettings _settings;
        private readonly IPythonRuntimeService _pythonRuntime;
        private bool _isInitialized;

        public AutomationService(
            ILogger<AutomationService> logger,
            IOptions<AppSettings> settings,
            IPythonRuntimeService pythonRuntime)
        {
            this._logger = logger;
            this._settings = settings.Value;
            this._pythonRuntime = pythonRuntime;
        }

        public void Initialize()
        {
            if (this._isInitialized)
            {
                return;
            }

            try
            {
                _logger.LogInformation("Initializing AutomationService...");

                // 确保Python运行时已初始化
                _pythonRuntime.EnsureInitialized();

                this._isInitialized = true;
                _logger.LogInformation("AutomationService initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize AutomationService");
                throw;
            }
        }

        public async Task<string> ExecutePythonScriptAsync(string script)
        {
            try
            {
                if (!this._isInitialized)
                {
                    Initialize();
                }

                _logger.LogDebug("Executing Python script...");
                return await _pythonRuntime.ExecuteCodeAsync(script);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute Python script");
                throw;
            }
        }

        /// <summary>
        /// Executes a browser task based on the provided task description.
        /// </summary>
        /// <param name="taskDescription">The description of the browser task to execute.</param>
        /// <returns>A task representing the asynchronous operation, returning true if successful.</returns>
        public async Task<bool> ExecuteBrowserTaskAsync(string taskDescription)
        {
            if (string.IsNullOrWhiteSpace(taskDescription))
            {
                throw new ArgumentException("Task description cannot be empty", nameof(taskDescription));
            }

            try
            {
                this.ValidateTaskDescription(taskDescription);
                this._logger.LogInformation("Executing browser task: {Description}", taskDescription);

                var taskType = this._taskHandlers.Keys
                    .FirstOrDefault(k => taskDescription.Contains(k, StringComparison.OrdinalIgnoreCase));

                if (taskType == null)
                {
                    throw new AutomationException($"No matching task type found for description: {taskDescription}");
                }

                await this.InitializeBrowserIfNeededAsync();
                await this._taskHandlers[taskType](taskDescription);
                return true;
            }
            catch (Exception ex) when (ex is not AutomationException)
            {
                this._logger.LogError(ex, "Failed to execute browser task: {Description}", taskDescription);
                return false;
            }
        }

        /// <summary>
        /// Executes a system task based on the provided task description.
        /// </summary>
        /// <param name="taskDescription">The description of the system task to execute.</param>
        /// <returns>A task representing the asynchronous operation, returning true if successful.</returns>
        public async Task<bool> ExecuteSystemTaskAsync(string taskDescription)
        {
            if (string.IsNullOrWhiteSpace(taskDescription))
            {
                throw new ArgumentException("Task description cannot be empty", nameof(taskDescription));
            }

            try
            {
                this.ValidateTaskDescription(taskDescription);
                this._logger.LogInformation("Executing system task: {Description}", taskDescription);

                var taskType = this._taskHandlers.Keys
                    .FirstOrDefault(k => taskDescription.Contains(k, StringComparison.OrdinalIgnoreCase));

                if (taskType != null && this._taskHandlers.TryGetValue(taskType, out var handler))
                {
                    await handler(taskDescription);
                    return true;
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported system task: {taskDescription}");
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "System task failed: {Error}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Executes a file task based on the provided task description.
        /// </summary>
        /// <param name="taskDescription">The description of the file task to execute.</param>
        /// <returns>A task representing the asynchronous operation, returning true if successful.</returns>
        public async Task<bool> ExecuteFileTaskAsync(string taskDescription)
        {
            if (string.IsNullOrWhiteSpace(taskDescription))
            {
                throw new ArgumentException("Task description cannot be empty", nameof(taskDescription));
            }

            try
            {
                this.ValidateTaskDescription(taskDescription);
                this._logger.LogInformation("Executing file task: {Description}", taskDescription);

                var taskType = this._taskHandlers.Keys
                    .FirstOrDefault(k => taskDescription.Contains(k, StringComparison.OrdinalIgnoreCase));

                if (taskType != null && this._taskHandlers.TryGetValue(taskType, out var handler))
                {
                    await handler(taskDescription);
                    return true;
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported file task: {taskDescription}");
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "File task failed: {Error}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Validates a task description and checks if it can be executed.
        /// </summary>
        /// <returns><see cref="bool"/> indicating whether the task can be executed.</returns>
        public bool ValidateTask(string taskDescription)
        {
            if (string.IsNullOrWhiteSpace(taskDescription))
            {
                throw new ArgumentException("Task description cannot be empty", nameof(taskDescription));
            }

            try
            {
                this.ValidateTaskDescription(taskDescription);
                var taskType = this._taskHandlers.Keys
                    .FirstOrDefault(k => taskDescription.Contains(k, StringComparison.OrdinalIgnoreCase));

                if (taskType == null)
                {
                    this._logger.LogWarning("Unsupported task type in description: {Description}", taskDescription);
                    return false;
                }

                // Additional platform-specific validation for system tasks
                if (taskType is "volume" or "brightness")
                {
                    return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ||
                           RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
                }

                return true;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Task validation failed: {Error}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Initializes the browser if it has not been initialized yet.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InitializeBrowserIfNeededAsync()
        {
            if (this._driver != null)
            {
                return;
            }

            try
            {
                this._driver = await Task.Run(() =>
                {
                    var options = new ChromeOptions();
                    options.AddArgument("--start-maximized");
                    options.AddArgument("--disable-notifications");

                    return new ChromeDriver(options);
                });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Failed to initialize browser");
                throw;
            }
        }

        /// <summary>
        /// Navigates to a URL based on the provided task description.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task NavigateToUrlAsync(string taskDescription)
        {
            if (string.IsNullOrWhiteSpace(taskDescription))
            {
                throw new ArgumentException("Task description cannot be empty", nameof(taskDescription));
            }

            await Task.Run(() =>
            {
                var url = this.ExtractPattern(taskDescription, @"https?://[\w\-\.]+\.\w+[\w\-\._~:/?#\[\]@!\$&'\(\)\*\+,;=]*");
                this._driver?.Navigate().GoToUrl(url);
                this._logger.LogInformation("Navigated to URL: {Url}", url);
            });
        }

        /// <summary>
        /// Clicks an element based on the provided task description.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task ClickElementAsync(string taskDescription)
        {
            if (string.IsNullOrWhiteSpace(taskDescription))
            {
                throw new ArgumentException("Task description cannot be empty", nameof(taskDescription));
            }

            await Task.Run(() =>
            {
                var elementText = this.ExtractElementText(taskDescription);
                var element = this._driver?.FindElement(By.XPath($"//*[contains(text(), '{elementText}')"));
                element?.Click();
                this._logger.LogInformation("Clicked element with text: {Text}", elementText);
            });
        }

        /// <summary>
        /// Types text into an element based on the provided task description.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task TypeTextAsync(string taskDescription)
        {
            if (string.IsNullOrWhiteSpace(taskDescription))
            {
                throw new ArgumentException("Task description cannot be empty", nameof(taskDescription));
            }

            var (elementText, typeInfo) = this.ExtractTypeInfo(taskDescription);

            // Use FindElementAsync if available, or wrap synchronous FindElement in Task.Run
            var element = await Task.Run(() => this._driver?.FindElement(By.XPath($"//*[contains(text(), '{elementText}')]")));

            if (element != null)
            {
                // Use SendKeysAsync if available, or wrap synchronous SendKeys in Task.Run
                await Task.Run(() => element.SendKeys(typeInfo));
                this._logger.LogInformation("Typed '{Text}' into element with text: {Element}", typeInfo, elementText);
            }
            else
            {
                this._logger.LogWarning("Could not find element with text: {Text}", elementText);
            }
        }

        /// <summary>
        /// Adjusts the system volume based on the provided task description.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task AdjustVolumeAsync(string taskDescription)
        {
            if (string.IsNullOrWhiteSpace(taskDescription))
            {
                throw new ArgumentException("Task description cannot be empty", nameof(taskDescription));
            }

            var level = this.ExtractNumericValue(taskDescription);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                await this._pythonRuntime.ExecuteCodeAsync($@"
from ctypes import cast, POINTER
from comtypes import CLSCTX_ALL
from pycaw.pycaw import AudioUtilities, IAudioEndpointVolume

devices = AudioUtilities.GetSpeakers()
interface = devices.Activate(IAudioEndpointVolume._iid_, CLSCTX_ALL, None)
volume = cast(interface, POINTER(IAudioEndpointVolume))
volume.SetMasterVolumeLevelScalar({level / 100.0}, None)
");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                await this._pythonRuntime.ExecuteCodeAsync($"import os; os.system('amixer -D pulse sset Master {level}%')");
            }

            this._logger.LogInformation("Set system volume to: {Level}%", level);
        }

        /// <summary>
        /// Adjusts the monitor brightness based on the provided task description.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task AdjustBrightnessAsync(string taskDescription)
        {
            if (string.IsNullOrWhiteSpace(taskDescription))
            {
                throw new ArgumentException("Task description cannot be empty", nameof(taskDescription));
            }

            var level = this.ExtractNumericValue(taskDescription);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                await this._pythonRuntime.ExecuteCodeAsync($@"
import wmi
c = wmi.WMI(namespace='root\\WMI')
methods = c.WmiMonitorBrightnessMethods()[0]
methods.WmiSetBrightness(Brightness={level}, Timeout=500)
");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                await this._pythonRuntime.ExecuteCodeAsync($@"
import os
display = os.popen('xrandr | grep ""connected"" | cut -f1 -d "" ""').read().strip()
os.system('xrandr --output ' + display + ' --brightness ' + str({level / 100.0}))");
            }

            this._logger.LogInformation("Set monitor brightness to: {Level}%", level);
        }

        /// <summary>
        /// Opens a file based on the provided task description.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task OpenFileAsync(string taskDescription)
        {
            if (string.IsNullOrWhiteSpace(taskDescription))
            {
                throw new ArgumentException("Task description cannot be empty", nameof(taskDescription));
            }

            var filePath = this.ExtractFilePath(taskDescription);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            await Task.Run(() =>
            {
                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo(filePath)
                    {
                        UseShellExecute = true,
                    },
                };
                process.Start();
            });
            this._logger.LogInformation("Opened file: {Path}", filePath);
        }

        /// <summary>
        /// Copies a file based on the provided task description.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task CopyFileAsync(string taskDescription)
        {
            if (string.IsNullOrWhiteSpace(taskDescription))
            {
                throw new ArgumentException("Task description cannot be empty", nameof(taskDescription));
            }

            var (sourcePath, destinationPath) = this.ExtractFilePaths(taskDescription);
            if (!File.Exists(sourcePath))
            {
                throw new FileNotFoundException($"Source file not found: {sourcePath}");
            }

            var destinationDir = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(destinationDir) && !Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            await Task.Run(() => File.Copy(sourcePath, destinationPath, true));
            this._logger.LogInformation("Copied file from {Source} to {Destination}", sourcePath, destinationPath);
        }

        /// <summary>
        /// Deletes a file based on the provided task description.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task DeleteFileAsync(string taskDescription)
        {
            if (string.IsNullOrWhiteSpace(taskDescription))
            {
                throw new ArgumentException("Task description cannot be empty", nameof(taskDescription));
            }

            var filePath = this.ExtractFilePath(taskDescription);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            await Task.Run(() => File.Delete(filePath));
            this._logger.LogInformation("Deleted file: {Path}", filePath);
        }

        /// <summary>
        /// Opens YouTube based on the provided task description.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task OpenYouTubeAsync(string taskDescription)
        {
            if (string.IsNullOrWhiteSpace(taskDescription))
            {
                throw new ArgumentException("Task description cannot be empty", nameof(taskDescription));
            }

            try
            {
                await this.InitializeBrowserIfNeededAsync();
                this._driver?.Navigate().GoToUrl("https://www.youtube.com");
                this._logger.LogInformation("Successfully opened YouTube");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Failed to open YouTube");
                throw new AutomationException("Failed to open YouTube", ex);
            }
        }

        private readonly Dictionary<string, Func<string, Task>> _taskHandlers;

        private IWebDriver? _driver;

        /// <summary>
        /// Initializes required Python dependencies.
        /// </summary>
        private async Task InitializePythonDependencies()
        {
            try
            {
                await this._pythonRuntime.ExecuteCodeAsync(
                "import sys\n" +
                "import subprocess\n" +
                "subprocess.check_call([sys.executable, '-m', 'pip', 'install', 'pycaw', 'comtypes', 'wmi'])");
                this._logger.LogInformation("Successfully installed Python dependencies");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Failed to install Python dependencies");
                throw new AutomationException("Failed to initialize Python dependencies", ex);
            }
        }

        /// <summary>
        /// Validates a task description.
        /// </summary>
        private void ValidateTaskDescription(string taskDescription)
        {
            if (string.IsNullOrWhiteSpace(taskDescription))
            {
                throw new ArgumentException("Task description cannot be empty", nameof(taskDescription));
            }
        }

        /// <summary>
        /// Extracts a pattern from a string.
        /// </summary>
        private string ExtractPattern(string input, string pattern)
        {
            var match = Regex.Match(input, pattern);
            return match.Success
                ? match.Value
                : throw new ArgumentException($"Could not extract pattern '{pattern}' from input");
        }

        /// <summary>
        /// Extracts the text of an element from a task description.
        /// </summary>
        private string ExtractElementText(string taskDescription)
        {
            var match = Regex.Match(taskDescription, @"click\s+(?:on\s+)?['""]?([^'""]+)['""]?", RegexOptions.IgnoreCase);
            return match.Success
                ? match.Groups[1].Value.Trim()
                : throw new ArgumentException("No element text found in task description");
        }

        /// <summary>
        /// Extracts type information from a task description.
        /// </summary>
        private (string ElementText, string TypeInfo) ExtractTypeInfo(string taskDescription)
        {
            var match = Regex.Match(taskDescription, @"type\s+['""]?([^'""]+)['""]?\s+into\s+['""]?([^'""]+)['""]?", RegexOptions.IgnoreCase);
            return match.Success
                ? (match.Groups[2].Value.Trim(), match.Groups[1].Value.Trim())
                : throw new ArgumentException("No type information found in task description");
        }

        /// <summary>
        /// Extracts a file path from a task description.
        /// </summary>
        private string ExtractFilePath(string taskDescription)
        {
            var match = Regex.Match(taskDescription, @"['""]?((?:[a-zA-Z]:|[\\/])[^\s'""]+)['""]?");
            return match.Success
                ? match.Groups[1].Value
                : throw new ArgumentException("No file path found in task description");
        }

        /// <summary>
        /// Extracts source and destination file paths from a task description.
        /// </summary>
        private (string Source, string Destination) ExtractFilePaths(string taskDescription)
        {
            var matches = Regex.Matches(taskDescription, @"['""]?((?:[a-zA-Z]:|[\\/])[^\s'""]+)['""]?");
            return matches.Count >= 2
                ? (matches[0].Groups[1].Value, matches[1].Groups[1].Value)
                : throw new ArgumentException("Source and destination paths not found in task description");
        }

        /// <summary>
        /// Extracts a numeric value from a task description.
        /// </summary>
        private int ExtractNumericValue(string taskDescription)
        {
            var match = Regex.Match(taskDescription, @"(\d+)%?");
            if (!match.Success || !int.TryParse(match.Groups[1].Value, out var value) || value < 0 || value > 100)
            {
                throw new ArgumentException("Invalid numeric value in task description (must be between 0 and 100)");
            }
            return value;
        }
    }

    /// <summary>
    /// Exception thrown when an automation task fails.
    /// </summary>
    public class AutomationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutomationException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public AutomationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomationException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public AutomationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
