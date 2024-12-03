// <copyright file="RateLimiter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core.Services.LLM
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Provides rate limiting functionality for API calls.
    /// </summary>
    public class RateLimiter
    {
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _modelSemaphores = new();
        private readonly ConcurrentDictionary<string, DateTime> _lastRequestTimes = new();
        private readonly ConcurrentDictionary<string, int> _requestCounts = new();
        private readonly TimeSpan _resetInterval = TimeSpan.FromHours(1);
        
        // 默认限制：每小时50个请求
        private const int DefaultRequestsPerHour = 50;
        private readonly ConcurrentDictionary<string, int> _modelLimits = new();

        /// <summary>
        /// Executes an action with rate limiting applied.
        /// </summary>
        /// <typeparam name="T">The return type of the action.</typeparam>
        /// <param name="modelId">The model identifier for rate limiting.</param>
        /// <param name="action">The action to execute.</param>
        /// <returns>The result of the action.</returns>
        public async Task<T> ExecuteWithRateLimitingAsync<T>(string modelId, Func<Task<T>> action)
        {
            var semaphore = _modelSemaphores.GetOrAdd(modelId, _ => new SemaphoreSlim(1, 1));
            await semaphore.WaitAsync();

            try
            {
                var now = DateTime.UtcNow;
                var lastRequestTime = _lastRequestTimes.GetOrAdd(modelId, now);
                var requestCount = _requestCounts.GetOrAdd(modelId, 0);
                var limit = _modelLimits.GetOrAdd(modelId, DefaultRequestsPerHour);

                // 检查是否需要重置计数器
                if (now - lastRequestTime > _resetInterval)
                {
                    _requestCounts.TryUpdate(modelId, 0, requestCount);
                    _lastRequestTimes.TryUpdate(modelId, now, lastRequestTime);
                    requestCount = 0;
                }

                // 检查是否超过限制
                if (requestCount >= limit)
                {
                    var waitTime = lastRequestTime.Add(_resetInterval) - now;
                    throw new RateLimitExceededException(
                        $"Rate limit exceeded for model {modelId}. Please try again in {waitTime.TotalMinutes:F0} minutes.",
                        waitTime);
                }

                // 更新请求计数
                _requestCounts.TryUpdate(modelId, requestCount + 1, requestCount);
                _lastRequestTimes.TryUpdate(modelId, now, lastRequestTime);

                try
                {
                    // 执行实际的API调用
                    return await action();
                }
                catch (Exception ex) when (ex.Message.Contains("rate limit exceeded"))
                {
                    // 从错误消息中提取等待时间
                    var waitTime = ExtractWaitTime(ex.Message);
                    throw new RateLimitExceededException(
                        $"Rate limit exceeded for model {modelId}. {ex.Message}",
                        waitTime,
                        ex);
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        private TimeSpan ExtractWaitTime(string errorMessage)
        {
            // 默认等待时间为1小时
            var waitTime = TimeSpan.FromHours(1);

            // 尝试从错误消息中提取等待时间
            var match = Regex.Match(errorMessage.ToLower(), @"try again in (?:about )?(\d+) (\w+)");
            if (match.Success)
            {
                var value = int.Parse(match.Groups[1].Value);
                var unit = match.Groups[2].Value;

                waitTime = unit switch
                {
                    "minute" or "minutes" => TimeSpan.FromMinutes(value),
                    "hour" or "hours" => TimeSpan.FromHours(value),
                    "second" or "seconds" => TimeSpan.FromSeconds(value),
                    _ => TimeSpan.FromHours(1)
                };
            }

            return waitTime;
        }

        /// <summary>
        /// Sets the rate limit for a specific model.
        /// </summary>
        /// <param name="modelId">The model identifier.</param>
        /// <param name="requestsPerHour">The number of requests allowed per hour.</param>
        public void SetModelLimit(string modelId, int requestsPerHour)
        {
            _modelLimits.AddOrUpdate(modelId, requestsPerHour, (_, _) => requestsPerHour);
        }

        /// <summary>
        /// Gets the remaining requests for a specific model.
        /// </summary>
        /// <param name="modelId">The model identifier.</param>
        /// <returns>The number of remaining requests in the current hour.</returns>
        public int GetRemainingRequests(string modelId)
        {
            var requestCount = _requestCounts.GetOrAdd(modelId, 0);
            var limit = _modelLimits.GetOrAdd(modelId, DefaultRequestsPerHour);
            return Math.Max(0, limit - requestCount);
        }

        /// <summary>
        /// Gets the time until rate limit reset for a specific model.
        /// </summary>
        /// <param name="modelId">The model identifier.</param>
        /// <returns>The time remaining until the rate limit resets.</returns>
        public TimeSpan GetTimeUntilReset(string modelId)
        {
            var lastRequestTime = _lastRequestTimes.GetOrAdd(modelId, DateTime.UtcNow);
            var resetTime = lastRequestTime.Add(_resetInterval);
            return resetTime - DateTime.UtcNow;
        }
    }

}
