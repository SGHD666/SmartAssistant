using System;
using Xunit;
using FluentAssertions;
using Moq;

namespace SmartAssistant.Tests.Common
{
    /// <summary>
    /// 所有测试类的基类，提供通用的测试基础设施
    /// </summary>
    public abstract class TestBase : IDisposable
    {
        protected TestBase()
        {
            // 在这里初始化测试环境
            SetUp();
        }

        /// <summary>
        /// 测试设置，在每个测试类构造时调用
        /// </summary>
        protected virtual void SetUp()
        {
            // 子类可以重写此方法以添加特定的设置
        }

        /// <summary>
        /// 测试清理，在每个测试类销毁时调用
        /// </summary>
        protected virtual void TearDown()
        {
            // 子类可以重写此方法以添加特定的清理
        }

        public void Dispose()
        {
            TearDown();
        }
    }
}
