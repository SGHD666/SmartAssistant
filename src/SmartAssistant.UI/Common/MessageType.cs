// <copyright file="MessageType.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.UI.Common
{
    /// <summary>
    /// 表示聊天消息的类型.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// 用户消息.
        /// </summary>
        User,

        /// <summary>
        /// 助手消息.
        /// </summary>
        Assistant,

        /// <summary>
        /// 系统消息.
        /// </summary>
        System,

        /// <summary>
        /// 错误消息.
        /// </summary>
        Error,
    }
}
