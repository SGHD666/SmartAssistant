// <copyright file="ChatMessage.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.UI.ViewModels
{
    using System;
    using SmartAssistant.UI.Common;
    
    /// <summary>
    /// 表示一条聊天消息
    /// </summary>
    public class ChatMessage
    {
        /// <summary>
        /// 获取或设置消息内容
        /// </summary>
        public required string Content { get; init; }

        /// <summary>
        /// 获取或设置消息时间戳
        /// </summary>
        public required DateTime Timestamp { get; init; }

        /// <summary>
        /// 获取或设置消息类型
        /// </summary>
        public MessageType Type { get; init; }

        /// <summary>
        /// 获取消息是否来自用户
        /// </summary>
        public bool IsUser => Type == MessageType.User;

        /// <summary>
        /// 获取消息是否为系统消息
        /// </summary>
        public bool IsSystem => Type == MessageType.System;

        /// <summary>
        /// 获取消息是否为错误消息
        /// </summary>
        public bool IsError => Type == MessageType.Error;

        /// <summary>
        /// 获取消息是否为助手消息
        /// </summary>
        public bool IsAssistant => Type == MessageType.Assistant;

        /// <summary>
        /// 获取消息的背景色
        /// </summary>
        public string BackgroundColor => Type switch
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
        public string ForegroundColor => Type switch
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
        public string Icon => Type switch
        {
            MessageType.User => "User",
            MessageType.Assistant => "Assistant",
            MessageType.System => "Info",
            MessageType.Error => "Error",
            _ => string.Empty
        };
    }
}
