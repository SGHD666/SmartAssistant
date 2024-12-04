// <copyright file="ChatMessage.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.UI.ViewModels
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using SmartAssistant.UI.Common;

    /// <summary>
    /// 表示一条聊天消息
    /// </summary>
    public class ChatMessage
    {
        private DateTime _timestamp;
        private string _content = string.Empty;

        /// <summary>
        /// Gets or sets 获取或设置消息内容.
        /// </summary>
        public string Content
        {
            get => _content;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Content cannot be null or whitespace.", nameof(Content));
                }
                _content = value;
            }
        }

        /// <summary>
        /// Gets or sets 获取或设置消息时间戳.
        /// </summary>
        public DateTime Timestamp
        {
            get => _timestamp;
            set
            {
                if (value == default)
                {
                    throw new ArgumentException("Timestamp cannot be the default value.", nameof(Timestamp));
                }
                _timestamp = value;
            }
        }

        /// <summary>
        /// Gets or sets 获取或设置消息类型.
        /// </summary>
        public MessageType Type { get; set; } = MessageType.User;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatMessage"/> class.
        /// </summary>
        public ChatMessage()
        {
            _timestamp = DateTime.MinValue;
        }

        [SetsRequiredMembers]
        /// <summary>
        /// Initializes a new instance of the <see cref="ChatMessage"/> class.
        /// </summary>
        /// <param name="content">The content of the message.</param>
        /// <param name="timestamp">The timestamp of the message.</param>
        /// <param name="type">The type of the message.</param>
        public ChatMessage(string content, DateTime timestamp, MessageType type)
        {
            if (timestamp == default)
            {
                throw new ArgumentException("Timestamp cannot be the default value.", nameof(timestamp));
            }

            this.Content = content;
            this.Timestamp = timestamp;
            this.Type = type;
        }

        /// <summary>
        /// Gets a value indicating whether 获取消息是否来自用户.
        /// </summary>
        public bool IsUser => this.Type == MessageType.User;

        /// <summary>
        /// 获取消息是否为系统消息
        /// </summary>
        public bool IsSystem => this.Type == MessageType.System;

        /// <summary>
        /// 获取消息是否为错误消息
        /// </summary>
        public bool IsError => this.Type == MessageType.Error;

        /// <summary>
        /// 获取消息是否为助手消息
        /// </summary>
        public bool IsAssistant => this.Type == MessageType.Assistant;

        /// <summary>
        /// 获取消息的背景色
        /// </summary>
        public string BackgroundColor => this.Type switch
        {
            MessageType.User => "#E3F2FD",
            MessageType.Assistant => "#FFFFFF",
            MessageType.System => "#F5F5F5",
            MessageType.Error => "#FFEBEE",
            _ => "#FFFFFF"
        };

        /// <summary>
        /// 获取消息的前景色
        /// </summary>
        public string ForegroundColor => this.Type switch
        {
            MessageType.User => "#1976D2",
            MessageType.Assistant => "#000000",
            MessageType.System => "#757575",
            MessageType.Error => "#D32F2F",
            _ => "#000000"
        };

        /// <summary>
        /// 获取消息的图标
        /// </summary>
        public string Icon => this.Type switch
        {
            MessageType.User => "User",
            MessageType.Assistant => "Assistant",
            MessageType.System => "Info",
            MessageType.Error => "Error",
            _ => string.Empty
        };
    }
}
