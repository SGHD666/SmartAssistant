# Development Environment Setup

This guide covers the setup of your development environment for SmartAssistant.

## Required Software

### 1. .NET Development
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- One of the following IDEs:
  - [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) (Community Edition or higher)
  - [JetBrains Rider](https://www.jetbrains.com/rider/)
  - [Visual Studio Code](https://code.visualstudio.com/) with C# extensions

### 2. Version Control
- [Git](https://git-scm.com/downloads)
- Git LFS for large file storage

### 3. Testing Tools
- xUnit (included in project)
- Moq (included in project)
- FluentAssertions (included in project)

## IDE Setup

### Visual Studio 2022
1. Required Extensions:
   - Avalonia for Visual Studio
   - CodeLens
   - Git Extensions
   - ReSharper (recommended)

2. Recommended Settings:
   ```json
   {
     "editor.formatOnSave": true,
     "editor.formatOnType": true,
     "omnisharp.enableRoslynAnalyzers": true
   }
   ```

### JetBrains Rider
1. Required Plugins:
   - Avalonia XAML Support
   - .NET Core User Secrets

2. Recommended Settings:
   - Enable "Format on Save"
   - Enable "Cleanup Code on Save"
   - Configure Code Style from `.editorconfig`

### Visual Studio Code
1. Required Extensions:
   - C# Dev Kit
   - .NET Core Test Explorer
   - GitLens
   - Avalonia for VS Code

2. Recommended Settings:
   ```json
   {
     "omnisharp.enableRoslynAnalyzers": true,
     "csharp.format.enable": true,
     "editor.formatOnSave": true
   }
   ```

## Code Style Configuration

1. EditorConfig Setup
   - Use the provided `.editorconfig` file
   - Ensure your IDE is configured to use EditorConfig

2. Code Analysis Rules
   - StyleCop.Analyzers is included in the project
   - Review rules in `.ruleset` files

## Git Configuration

1. Basic Setup
   ```bash
   git config --global user.name "Your Name"
   git config --global user.email "your.email@example.com"
   ```

2. Git LFS Setup
   ```bash
   git lfs install
   ```

3. Recommended Git Settings
   ```bash
   git config --global core.autocrlf true  # On Windows
   git config --global core.autocrlf input # On Linux/macOS
   ```

## Project Setup

1. Clone and Build
   ```bash
   git clone https://github.com/SGHD666/SmartAssistant.git
   cd SmartAssistant
   dotnet restore
   dotnet build
   ```

2. Configure User Secrets
   ```bash
   cd src/SmartAssistant.UI
   dotnet user-secrets init
   dotnet user-secrets set "OpenAI:ApiKey" "your-api-key"
   ```

## Verification

Run the following commands to verify your setup:

```bash
# Build the solution
dotnet build

# Run tests
dotnet test

# Run the application
cd src/SmartAssistant.UI
dotnet run
```

## Next Steps

- Review the [Development Guidelines](../guides/development-guidelines.md)
- Set up your [Git Workflow](../guides/git-workflow.md)
- Check the [Testing Guide](../guides/testing.md)
