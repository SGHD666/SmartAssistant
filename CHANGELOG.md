# Changelog

All notable changes to this project will be documented in this file.

## [v0.1.1] - 2024-02-07

### Added
- 项目初始化配置和文档
  - 添加了 `.cascade/init.md` 初始化指南
  - 完善了开发准则文档
  - 更新了需求规格说明
  - 细化了项目路线图
  - 补充了基础 README 说明

### Technical Details
- 建立了完整的文档体系，包括：
  - 开发准则 (DEVELOPMENT_GUIDELINES.md)
  - 需求文档 (REQUIREMENTS.md)
  - 路线图 (ROADMAP.md)
  - 变更日志 (CHANGELOG.md)
  - 项目说明 (README.md)
- 规范了文档更新和维护流程

### Dependencies
- 保持与 v0.1.0 相同

## [v0.1.0] - 2024-01-31

### Added
- 基础的智能助手功能框架
- 多语言模型支持 (OpenAI GPT-3.5/4, Claude, QianWen)
- 语言模型服务的依赖注入和工厂模式实现
- 基础的用户界面，支持聊天对话
- 命令执行系统
  - 支持打开 YouTube 命令
  - 支持网页导航命令

### Technical Details
- 实现了语言模型服务的依赖注入配置
- 添加了 ModelManager 用于管理和切换不同的语言模型
- 实现了 LLMFactory 用于创建语言模型服务实例
- 添加了 AssistantController 处理用户输入和命令执行
- 使用 MVVM 模式实现了用户界面
- 实现了服务的重试和错误处理机制

### Dependencies
- Avalonia UI for cross-platform GUI
- CommunityToolkit.Mvvm for MVVM implementation
- Polly for resilience and transient-fault-handling
