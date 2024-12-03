# Smart Assistant

![Version](https://img.shields.io/badge/version-v0.1.0-blue.svg)

> ⚠️ **开发者注意**: 在开始任何开发工作之前，请务必先阅读 [开发准则](DEVELOPMENT_GUIDELINES.md)。

An intelligent cross-platform AI assistant powered by large language models that can understand and execute complex computer operations.

## Features

- Natural Language Understanding using GPT models
- Voice Recognition and Text-to-Speech
- Automated Task Execution
- Cross-platform UI using Avalonia
- Secure Local Data Storage
- User Habit Learning

## Project Structure

- `src/SmartAssistant.Core` - Core business logic and services
- `src/SmartAssistant.UI` - Avalonia-based cross-platform UI
- `src/SmartAssistant.API` - ASP.NET Core Web API
- `src/SmartAssistant.Automation` - Task automation implementations
- `src/SmartAssistant.Common` - Shared utilities and models

## Requirements

- .NET 7.0 or later
- Python 3.8 or later
- Visual Studio 2022 or later / VS Code
- SQLite

## Getting Started

1. Clone the repository
2. Install the required .NET SDK
3. Install Python dependencies: `pip install -r requirements.txt`
4. Open the solution in Visual Studio
5. Build and run the project

## Security

- All sensitive data is encrypted at rest
- API keys and credentials must be stored securely
- Local-only data storage by default
