using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SmartAssistant.Core.Controllers;
using SmartAssistant.Core.Models;
using SmartAssistant.Core.Services.LLM;
using SmartAssistant.UI.Common;
using System.Windows.Input;

namespace SmartAssistant.UI.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly ModelManager _modelManager;
        private readonly ModelSettings _modelSettings;
        private readonly AssistantController _assistantController;
        private readonly ILogger<MainWindowViewModel> _logger;
        private readonly Dictionary<string, LLMType> _modelTypeMap = new()
        {
            { "GPT-3.5", LLMType.OpenAI_GPT35 },
            { "GPT-4", LLMType.OpenAI_GPT4 },
            { "Claude", LLMType.Claude },
            { "QianWen", LLMType.QianWen }
        };

        [ObservableProperty]
        private string _userInput = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _isInitializing;

        [ObservableProperty]
        private string _selectedModelDisplay = "Loading...";

        [ObservableProperty]
        private string _rateLimitStatus = string.Empty;

        [ObservableProperty]
        private bool _isRateLimited;

        [ObservableProperty]
        private bool _isRunning;

        [ObservableProperty]
        private string _statusMessage = "Assistant is stopped";

        public ObservableCollection<ChatMessage> Messages { get; } = new();
        public ObservableCollection<string> AvailableModels { get; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        /// <param name="modelManager"></param>
        /// <param name="modelSettings"></param>
        /// <param name="assistantController"></param>
        /// <param name="logger"></param>
        public MainWindowViewModel(
            ModelManager modelManager, 
            ModelSettings modelSettings, 
            AssistantController assistantController,
            ILogger<MainWindowViewModel> logger)
        {
            this._modelManager = modelManager;
            this._modelSettings = modelSettings;
            this._assistantController = assistantController;
            this._logger = logger;
            this._logger.LogDebug("MainWindowViewModel constructor completed");
        }

        public async Task InitializeAsync()
        {
            try
            {
                this.IsInitializing = true;
                this.IsBusy = true;
                this._logger.LogDebug("Starting async initialization");

                // 初始化可用模型列表
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    foreach (var (display, type) in this._modelTypeMap)
                    {
                        if (this._modelSettings.ModelConfigs.Values.Any(c => c.Type == type))
                        {
                            this.AvailableModels.Add(display);
                            this._logger.LogDebug("Added available model: {ModelDisplay}", display);
                        }
                    }

                    // 设置当前选中的模型
                    this.SelectedModelDisplay = this._modelTypeMap
                        .FirstOrDefault(x => x.Value == this._modelSettings.CurrentModel)
                        .Key ?? this.AvailableModels.FirstOrDefault() ?? "GPT-3.5";
                    
                    this._logger.LogInformation("Selected model: {SelectedModel}", this.SelectedModelDisplay);
                });

                // 验证服务状态
                try 
                {
                    // 尝试发送一个简单的测试消息来验证服务状态
                    var response = await this._assistantController.ProcessUserInputAsync("COMMAND:test_connection");
                    if (response != null)
                    {
                        this._logger.LogInformation("Core services verified successfully");
                    }
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "Failed to verify core services");
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        this.Messages.Add(new ChatMessage
                        {
                            Content = "Failed to initialize core services. Please check your configuration and try again.",
                            Type = Common.MessageType.Error,
                            Timestamp = DateTime.Now
                        });
                    });
                    return;
                }

                this._logger.LogDebug("Async initialization completed");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error during async initialization");
                throw;
            }
            finally
            {
                this.IsInitializing = false;
                this.IsBusy = false;
            }
        }

        partial void OnSelectedModelDisplayChanged(string value)
        {
            if (this.IsInitializing) return;

            this._logger.LogInformation("Model selection changed to: {NewModel}", value);

            if (this._modelTypeMap.TryGetValue(value, out var type))
            {
                this._modelManager.SwitchModel(type);
                this._logger.LogDebug("Model switched to type: {ModelType}", type);
            }
        }

        [RelayCommand]
        private async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(this.UserInput))
            {
                this._logger.LogDebug("Empty user input, skipping message send");
                return;
            }

            this._logger.LogDebug("Processing user message: {UserInput}", this.UserInput);
            this.IsBusy = true;
            try
            {
                // 添加用户消息
                var userMessage = new ChatMessage(this.UserInput, DateTime.Now, MessageType.User);
                this.Messages.Add(userMessage);

                // 清空输入
                this.UserInput = string.Empty;

                // 获取助手响应
                var response = await this._assistantController.GetResponseAsync(userMessage.Content);

                // 添加助手消息
                var assistantMessage = new ChatMessage(
                    content: response,
                    timestamp: DateTime.Now,
                    type: MessageType.Assistant
                );
                this.Messages.Add(assistantMessage);
            }
            catch (RateLimitExceededException ex)
            {
                await this.HandleRateLimitExceptionAsync(ex);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error getting response");
                var errorMessage = new ChatMessage(ex.Message, DateTime.Now, MessageType.Error);
                this.Messages.Add(errorMessage);
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task HandleRateLimitExceptionAsync(RateLimitExceededException ex)
        {
            try
            {
                this.IsRateLimited = true;
                this.RateLimitStatus = $"Rate limit exceeded. Please wait {ex.RetryAfter.TotalMinutes:F0} minutes.";
                
                this.Messages.Add(new ChatMessage 
                { 
                    Content = $"Rate limit exceeded. The system will automatically retry in {ex.RetryAfter.TotalMinutes:F0} minutes.", 
                    Timestamp = DateTime.Now,
                    Type = MessageType.Error
                });

                this._logger.LogWarning("Rate limit exceeded. Waiting for {Minutes} minutes", ex.RetryAfter.TotalMinutes);
                await Task.Delay(TimeSpan.FromSeconds(1)); // Give UI time to update

                // Start countdown
                var countdown = (int)ex.RetryAfter.TotalMinutes;
                while (countdown > 0 && this.IsRateLimited)
                {
                    this.RateLimitStatus = $"Rate limit exceeded. Please wait {countdown} minutes.";
                    await Task.Delay(TimeSpan.FromMinutes(1));
                    countdown--;
                    this._logger.LogDebug("Rate limit countdown: {Minutes} minutes remaining", countdown);
                }

                if (this.IsRateLimited) // If user hasn't manually cancelled
                {
                    this.IsRateLimited = false;
                    this.RateLimitStatus = string.Empty;
                    this.Messages.Add(new ChatMessage 
                    { 
                        Content = "Rate limit reset. You can continue now.", 
                        Timestamp = DateTime.Now,
                        Type = MessageType.System
                    });
                    this._logger.LogInformation("Rate limit reset completed");
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(e, "Error handling rate limit exception");
                this.IsRateLimited = false;
                this.RateLimitStatus = string.Empty;
                this.Messages.Add(new ChatMessage 
                { 
                    Content = "An error occurred while handling rate limit. Please try again.", 
                    Timestamp = DateTime.Now,
                    Type = MessageType.Error
                });
            }
        }

        [RelayCommand]
        private void CancelRateLimit()
        {
            this._logger.LogInformation("User cancelled rate limit wait");
            this.IsRateLimited = false;
            this.RateLimitStatus = string.Empty;
            this.Messages.Add(new ChatMessage 
            { 
                Content = "Rate limit wait cancelled by user.", 
                Timestamp = DateTime.Now,
                Type = MessageType.System
            });
        }

        [RelayCommand(CanExecute = nameof(CanStart))]
        private void Start()
        {
            this.IsRunning = true;
            this.StatusMessage = "Assistant is running";
        }

        [RelayCommand(CanExecute = nameof(CanStop))]
        private void Stop()
        {
            this.IsRunning = false;
            this.StatusMessage = "Assistant is stopped";
        }

        private bool CanStart() => !this.IsRunning;
        private bool CanStop() => this.IsRunning;
    }
}
