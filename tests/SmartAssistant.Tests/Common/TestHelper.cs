using System;
using System.IO;
using System.Threading.Tasks;

namespace SmartAssistant.Tests.Common
{
    /// <summary>
    /// 提供测试中常用的辅助方法
    /// </summary>
    public static class TestHelper
    {
        /// <summary>
        /// 获取测试数据目录的路径
        /// </summary>
        public static string GetTestDataPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData");
        }

        /// <summary>
        /// 创建临时测试文件
        /// </summary>
        public static async Task<string> CreateTempTestFileAsync(string content)
        {
            var tempFile = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.tmp");
            await File.WriteAllTextAsync(tempFile, content);
            return tempFile;
        }

        /// <summary>
        /// 清理临时测试文件
        /// </summary>
        public static void CleanupTempFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// 创建测试用的随机字符串
        /// </summary>
        public static string GenerateRandomString(int length = 10)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];

            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }
    }
}
