// <copyright file="AutomationException.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.Core.Services
{
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using Python.Runtime;
    using SmartAssistant.Core.Models;
    using SmartAssistant.Core.Services.LLM;

    /// <summary>
    /// Exception thrown when an automation task fails.
    /// </summary>
    public class AutomationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutomationException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public AutomationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomationException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public AutomationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
