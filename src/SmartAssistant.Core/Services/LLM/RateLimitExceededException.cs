// <copyright file="RateLimitExceededException.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core.Services.LLM
{
    using System;

    /// <summary>
    /// Exception thrown when a rate limit is exceeded.
    /// </summary>
    public class RateLimitExceededException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RateLimitExceededException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="retryAfter">The time to wait before retrying.</param>
        /// <param name="innerException">The inner exception.</param>
        public RateLimitExceededException(string message, TimeSpan retryAfter, Exception? innerException = null)
            : base(message, innerException)
        {
            this.RetryAfter = retryAfter;
        }

        /// <summary>
        /// Gets the time to wait before retrying.
        /// </summary>
        public TimeSpan RetryAfter { get; }
    }
}
