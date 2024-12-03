using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;
using Avalonia.Platform;
using Avalonia.Threading;
using SmartAssistant.Tests.Common;
using Xunit;

namespace SmartAssistant.Tests.UI
{
    /// <summary>
    /// UI测试的基类，提供Avalonia UI测试环境
    /// </summary>
    public abstract class UITestBase : TestBase, IAsyncLifetime
    {
        static UITestBase()
        {
            // 初始化Avalonia无头测试环境
            AppBuilder.Configure<Application>()
                     .UseHeadless(new AvaloniaHeadlessPlatformOptions())
                     .UseSkia()
                     .SetupWithoutStarting();
        }

        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 在UI线程上运行操作
        /// </summary>
        protected async Task OnUIThread(Action action)
        {
            await Dispatcher.UIThread.InvokeAsync(action);
        }

        /// <summary>
        /// 在UI线程上运行异步操作
        /// </summary>
        protected async Task OnUIThread(Func<Task> action)
        {
            await Dispatcher.UIThread.InvokeAsync(action);
        }

        /// <summary>
        /// 等待UI更新
        /// </summary>
        protected async Task WaitForUIUpdate(int milliseconds = 50)
        {
            await Task.Delay(milliseconds);
        }
    }
}
