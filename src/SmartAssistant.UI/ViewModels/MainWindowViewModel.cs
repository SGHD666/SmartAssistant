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
        private static readonly Dictionary<string, LLMType> _modelTypeMap = new()
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

        public ObservableCollection<ChatMessage> Messages { get; } = new();
        public ObservableCollection<string> AvailableModels { get; } = new();

        public MainWindowViewModel(
            ModelManager modelManager, 
            ModelSettings modelSettings, 
            AssistantController assistantController,
            ILogger<MainWindowViewModel> logger)
        {
            _modelManager = modelManager;
            _modelSettings = modelSettings;
            _assistantController = assistantController;
            _logger = logger;
            _logger.LogDebug("MainWindowViewModel constructor completed");
        }

        public async Task InitializeAsync()
        {
            try
            {
                IsInitializing = true;
                IsBusy = true;
                _logger.LogDebug("Starting async initialization");

                // 初始化可用模型列表
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    foreach (var (display, type) in _modelTypeMap)
                    {
                        if (_modelSettings.ModelConfigs.Values.Any(c => c.Type == type))
                        {
                            AvailableModels.Add(display);
                            _logger.LogDebug("Added available model: {ModelDisplay}", display);
                        }
                    }

                    // 设置当前选中的模型
                    SelectedModelDisplay = _modelTypeMap
                        .FirstOrDefault(x => x.Value == _modelSettings.CurrentModel)
                        .Key ?? AvailableModels.FirstOrDefault() ?? "GPT-3.5";
                    
                    _logger.LogInformation("Selected model: {SelectedModel}", SelectedModelDisplay);
                });

                // 验证服务状态
                try 
                {
                    // 尝试发送一个简单的测试消息来验证服务状态
                    var response = await _assistantController.ProcessUserInputAsync("COMMAND:test_connection");
                    if (response != null)
                    {
                        _logger.LogInformation("Core services verified successfully");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to verify core services");
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        Messages.Add(new ChatMessage
                        {
                            Content = "Failed to initialize core services. Please check your configuration and try again.",
                            Type = Common.MessageType.Error,
                            Timestamp = DateTime.Now
                        });
                    });
                    return;
                }

                _logger.LogDebug("Async initialization completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during async initialization");
                throw;
            }
            finally
            {
                IsInitializing = false;
                IsBusy = false;
            }
        }

        partial void OnSelectedModelDisplayChanged(string value)
        {
            if (IsInitializing) return;
            
            _logger.LogInformation("Model selection changed to: {NewModel}", value);
            if (_modelTypeMap.TryGetValue(value, out var type))
            {
                _modelManager.SwitchModel(type);
                _logger.LogDebug("Model switched to type: {ModelType}", type);
            }
        }

        [RelayCommand]
        private async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(UserInput))
            {
                _logger.LogDebug("Empty user input, skipping message send");
                return;
            }

            _logger.LogDebug("Processing user message: {UserInput}", UserInput);
            IsBusy = true;
            try
            {
                // 添加用户消息
                var userMessage = new ChatMessage
                {
                    Content = UserInput,
                    Timestamp = DateTime.Now,
                    Type = MessageType.User
                };
                Messages.Add(userMessage);

                // 清空输入
                UserInput = string.Empty;

                // 获取助手响应
                var response = await _assistantController.GetResponseAsync(userMessage.Content);
                
                // 添加助手消息
                var assistantMessage = new ChatMessage
                {
                    Content = response,
                    Timestamp = DateTime.Now,
                    Type = MessageType.Assistant
                };
                Messages.Add(assistantMessage);
            }
            catch (RateLimitExceededException ex)
            {
                await HandleRateLimitExceptionAsync(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting response");
                var errorMessage = new ChatMessage
                {
                    Content = "Sorry, an error occurred while processing your request. Please try again.",
                    Timestamp = DateTime.Now,
                    Type = MessageType.Error
                };
                Messages.Add(errorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task HandleRateLimitExceptionAsync(RateLimitExceededException ex)
        {
            try
            {
                IsRateLimited = true;
                RateLimitStatus = $"Rate limit exceeded. Please wait {ex.RetryAfter.TotalMinutes:F0} minutes.";
                
                Messages.Add(new ChatMessage 
                { 
                    Content = $"Rate limit exceeded. The system will automatically retry in {ex.RetryAfter.TotalMinutes:F0} minutes.", 
                    Timestamp = DateTime.Now,
                    Type = MessageType.Error
                });

                _logger.LogWarning("Rate limit exceeded. Waiting for {Minutes} minutes", ex.RetryAfter.TotalMinutes);
                await Task.Delay(TimeSpan.FromSeconds(1)); // Give UI time to update

                // Start countdown
                var countdown = (int)ex.RetryAfter.TotalMinutes;
                while (countdown > 0 && IsRateLimited)
                {
                    RateLimitStatus = $"Rate limit exceeded. Please wait {countdown} minutes.";
                    await Task.Delay(TimeSpan.FromMinutes(1));
                    countdown--;
                    _logger.LogDebug("Rate limit countdown: {Minutes} minutes remaining", countdown);
                }

                if (IsRateLimited) // If user hasn't manually cancelled
                {
                    IsRateLimited = false;
                    RateLimitStatus = string.Empty;
                    Messages.Add(new ChatMessage 
                    { 
                        Content = "Rate limit reset. You can continue now.", 
                        Timestamp = DateTime.Now,
                        Type = MessageType.System
                    });
                    _logger.LogInformation("Rate limit reset completed");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error handling rate limit exception");
                IsRateLimited = false;
                RateLimitStatus = string.Empty;
                Messages.Add(new ChatMessage 
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
            _logger.LogInformation("User cancelled rate limit wait");
            IsRateLimited = false;
            RateLimitStatus = string.Empty;
            Messages.Add(new ChatMessage 
            { 
                Content = "Rate limit wait cancelled by user.", 
                Timestamp = DateTime.Now,
                Type = MessageType.System
            });
        }
    }
}
