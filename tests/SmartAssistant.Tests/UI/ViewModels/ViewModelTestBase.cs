using System;
using System.Reactive;
using System.Reactive.Concurrency;
using Microsoft.Reactive.Testing;
using SmartAssistant.Tests.Common;

namespace SmartAssistant.Tests.UI.ViewModels
{
    /// <summary>
    /// ViewModel测试的基类，提供Rx测试支持
    /// </summary>
    public abstract class ViewModelTestBase : TestBase
    {
        protected TestScheduler TestScheduler { get; }

        protected ViewModelTestBase()
        {
            TestScheduler = new TestScheduler();
        }

        /// <summary>
        /// 推进测试调度器的时间
        /// </summary>
        protected void AdvanceBy(TimeSpan time)
        {
            TestScheduler.AdvanceBy(time.Ticks);
        }

        /// <summary>
        /// 推进测试调度器到指定时间
        /// </summary>
        protected void AdvanceTo(TimeSpan time)
        {
            TestScheduler.AdvanceTo(time.Ticks);
        }

        /// <summary>
        /// 创建冷可观察序列
        /// </summary>
        protected ITestableObservable<T> CreateColdObservable<T>(params Recorded<Notification<T>>[] records)
        {
            return TestScheduler.CreateColdObservable(records);
        }

        /// <summary>
        /// 创建热可观察序列
        /// </summary>
        protected ITestableObservable<T> CreateHotObservable<T>(params Recorded<Notification<T>>[] records)
        {
            return TestScheduler.CreateHotObservable(records);
        }

        /// <summary>
        /// 创建观察者
        /// </summary>
        protected ITestableObserver<T> CreateObserver<T>()
        {
            return TestScheduler.CreateObserver<T>();
        }
    }
}
