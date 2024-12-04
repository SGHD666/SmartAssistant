# Quick Start Guide

This guide will help you get SmartAssistant up and running quickly.

## Prerequisites

- Windows 10/11 or Linux/macOS
- .NET 8.0 SDK
- Git
- Visual Studio 2022 or JetBrains Rider (recommended)

## Installation Steps

1. Clone the repository:
   ```bash
   git clone https://github.com/SGHD666/SmartAssistant.git
   cd SmartAssistant
   ```

2. Install dependencies:
   ```bash
   dotnet restore
   ```

3. Follow initialization steps:
   - See [initialization.txt](./initialization.txt) for detailed setup instructions

4. Build the project:
   ```bash
   dotnet build
   ```

5. Run the tests:
   ```bash
   dotnet test
   ```

6. Run the application:
   ```bash
   cd src/SmartAssistant.UI
   dotnet run
   ```

## Configuration

1. API Keys
   - Create a copy of `appsettings.json.template` as `appsettings.json`
   - Add your OpenAI API key
   - Configure other optional settings

2. Development Settings
   - Set up your IDE according to [Development Environment Setup](./dev-environment.md)
   - Install recommended extensions

## Next Steps

- Read the [Development Guidelines](../guides/development-guidelines.md)
- Check out the [Architecture Overview](../architecture/system-architecture.md)
- Review the [API Documentation](../api/core-services.md)

## Troubleshooting

Common issues and their solutions:

1. Build Errors
   - Ensure .NET 8.0 SDK is installed
   - Run `dotnet restore` to refresh dependencies
   - Clear the build cache: `dotnet clean`

2. Runtime Errors
   - Check API key configuration
   - Verify system requirements
   - Review application logs

For more detailed information, see the [Configuration Guide](./configuration.md).
