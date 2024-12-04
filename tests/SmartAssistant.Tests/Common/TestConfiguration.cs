using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SmartAssistant.Tests.Common
{
    /// <summary>
    /// 测试配置管理器
    /// </summary>
    public static class TestConfiguration
    {
        private static IConfiguration? _configuration;

        /// <summary>
        /// 获取测试配置
        /// </summary>
        public static IConfiguration GetConfiguration()
        {
            if (_configuration == null)
            {
                var configBuilder = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("testsettings.json", optional: true)
                    .AddEnvironmentVariables();

                _configuration = configBuilder.Build();
            }

            return _configuration;
        }

        /// <summary>
        /// 获取配置项的值
        /// </summary>
        public static string GetValue(string key)
        {
            return GetConfiguration()[key]!;
        }

        /// <summary>
        /// 获取带默认值的配置项
        /// </summary>
        public static string GetValue(string key, string defaultValue)
        {
            return GetConfiguration()[key] ?? defaultValue;
        }
    }
}
