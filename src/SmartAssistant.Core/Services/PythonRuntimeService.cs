// <copyright file="PythonRuntimeService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core.Services
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Python.Runtime;

    /// <summary>
    /// Service for managing Python.NET runtime initialization and execution.
    /// </summary>
    public class PythonRuntimeService : IPythonRuntimeService
    {
        private readonly string _pythonPath;
        private readonly ILogger<PythonRuntimeService> _logger;
        private bool _isInitialized;
        private readonly object _lockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="PythonRuntimeService"/> class.
        /// </summary>
        /// <param name="pythonPath">Path to Python installation.</param>
        /// <param name="logger">Logger instance.</param>
        public PythonRuntimeService(string pythonPath, ILogger<PythonRuntimeService> logger)
        {
            _pythonPath = pythonPath;
            _logger = logger;
        }

        /// <summary>
        /// Ensures Python.NET runtime is initialized.
        /// </summary>
        public void EnsureInitialized()
        {
            if (_isInitialized)
            {
                return;
            }

            lock (_lockObject)
            {
                if (_isInitialized)
                {
                    return;
                }

                try
                {
                    _logger.LogInformation("Starting Python.NET runtime initialization...");

                    // 1. 验证Python环境
                    if (!ValidatePythonEnvironment())
                    {
                        throw new InvalidOperationException("Python environment validation failed");
                    }

                    // 2. 获取Python DLL路径
                    var pythonDll = GetPythonDll(_pythonPath);
                    if (string.IsNullOrEmpty(pythonDll))
                    {
                        throw new FileNotFoundException("Python DLL not found in the Python directory");
                    }

                    // 3. 设置Runtime.PythonDLL（必须在初始化之前设置）
                    Runtime.PythonDLL = pythonDll;
                    _logger.LogDebug("Set Runtime.PythonDLL to: {PythonDll}", pythonDll);

                    // 4. 配置环境变量
                    ConfigurePythonEnvironment();

                    // 5. 初始化Python引擎
                    _logger.LogDebug("Initializing Python engine...");
                    PythonEngine.Initialize();

                    // 6. 验证初始化
                    if (!PythonEngine.IsInitialized)
                    {
                        throw new InvalidOperationException("Python engine initialization failed");
                    }

                    _isInitialized = true;
                    _logger.LogInformation("Python.NET runtime initialized successfully");
                }
                catch (DllNotFoundException ex)
                {
                    _logger.LogError(ex, "Failed to load Python DLL. Please ensure Python {Version} is installed", "3.9");
                    throw;
                }
                catch (BadImageFormatException ex)
                {
                    _logger.LogError(ex, "Python DLL architecture mismatch. Ensure using correct 32/64-bit version");
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to initialize Python.NET runtime: {Error}", ex.Message);
                    throw;
                }
            }
        }

        private bool ValidatePythonEnvironment()
        {
            try
            {
                _logger.LogDebug("Validating Python environment...");
                
                // 验证Python可执行文件
                if (!File.Exists(_pythonPath))
                {
                    _logger.LogError("Python executable not found at: {PythonPath}", _pythonPath);
                    return false;
                }

                var pythonDir = Path.GetDirectoryName(_pythonPath);
                if (string.IsNullOrEmpty(pythonDir))
                {
                    _logger.LogError("Invalid Python path: {PythonPath}", _pythonPath);
                    return false;
                }

                // 验证环境变量
                var path = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
                if (!path.Contains(pythonDir))
                {
                    _logger.LogWarning("Python directory not in PATH: {PythonDir}", pythonDir);
                }

                _logger.LogInformation("Python environment validation successful");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Python environment");
                return false;
            }
        }

        private void ConfigurePythonEnvironment()
        {
            _logger.LogDebug("Configuring Python environment...");
            
            var pythonDir = Path.GetDirectoryName(_pythonPath);
            if (string.IsNullOrEmpty(pythonDir))
            {
                throw new InvalidOperationException($"Invalid Python path: {_pythonPath}");
            }

            // 更新PATH环境变量
            var path = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            if (!path.Contains(pythonDir))
            {
                path = $"{pythonDir};{path}";
                Environment.SetEnvironmentVariable("PATH", path);
                _logger.LogDebug("Added Python directory to PATH: {PythonDir}", pythonDir);
            }

            // 设置PYTHONNET_PYDLL
            var pythonDll = GetPythonDll(_pythonPath);
            Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", pythonDll);
            _logger.LogDebug("Set PYTHONNET_PYDLL environment variable to: {PythonDll}", pythonDll);
        }

        /// <summary>
        /// Executes Python code using Python.NET runtime.
        /// </summary>
        /// <param name="code">Python code to execute.</param>
        /// <returns>Result of the execution.</returns>
        public async Task<string> ExecuteCodeAsync(string code)
        {
            EnsureInitialized();

            return await Task.Run(() =>
            {
                using (Py.GIL())
                {
                    try
                    {
                        using dynamic scope = Py.CreateScope();
                        scope.exec(code);
                        return "Success";
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error executing Python code");
                        throw;
                    }
                }
            });
        }

        private string GetPythonDll(string pythonPath)
        {
            var pythonDir = Path.GetDirectoryName(pythonPath);
            if (string.IsNullOrEmpty(pythonDir))
            {
                throw new InvalidOperationException($"Invalid Python path: {pythonPath}");
            }

            _logger.LogDebug("Searching for Python DLL in: {PythonDir}", pythonDir);

            // 检查可能的DLL文件
            var possibleDlls = new[]
            {
                Path.Combine(pythonDir, "python39.dll"),
                Path.Combine(pythonDir, "python3.dll"),
                Path.Combine(pythonDir, "python.dll"),
            };

            foreach (var dll in possibleDlls)
            {
                if (File.Exists(dll))
                {
                    _logger.LogDebug("Found Python DLL: {DllPath}", dll);
                    return dll;
                }
                else
                {
                    _logger.LogTrace("DLL not found at: {DllPath}", dll);
                }
            }

            _logger.LogError("No suitable Python DLL found in: {PythonDir}", pythonDir);
            throw new FileNotFoundException($"No suitable Python DLL found in: {pythonDir}");
        }

        /// <summary>
        /// Finalizes the Python.NET runtime.
        /// </summary>
        public void Shutdown()
        {
            if (_isInitialized)
            {
                try
                {
                    PythonEngine.Shutdown();
                    _isInitialized = false;
                    _logger.LogInformation("Python.NET runtime shut down successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error shutting down Python.NET runtime");
                }
            }
        }
    }
}
