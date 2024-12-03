# Smart Assistant

![Version](https://img.shields.io/badge/version-v0.1.0-blue.svg)
![Tests](https://img.shields.io/badge/tests-in%20progress-yellow.svg)
![Coverage](https://img.shields.io/badge/coverage-in%20progress-yellow.svg)

An intelligent cross-platform AI assistant powered by large language models that can understand and execute complex computer operations.

## Development Status

### Completed
- Initial project setup with .NET 8.0 and Avalonia UI
- Basic UI components implementation
- Git repository setup with proper .gitignore
- Main and development branch structure

### In Progress
- Unit testing framework setup
- UI component testing
- Test coverage implementation

### Planned
- Continuous Integration setup
- Code quality checks
- Automated test running in CI/CD

## Features

- Natural Language Understanding using GPT models
- Voice Recognition and Text-to-Speech
- Automated Task Execution
- Cross-platform UI using Avalonia
- Secure Local Data Storage
- User Habit Learning

## Project Structure

- `src/SmartAssistant.Core` - Core business logic and services
- `src/SmartAssistant.UI` - Avalonia-based user interface
  - `Common/` - Shared types and utilities
  - `Converters/` - UI value converters
  - `Views/` - UI components and windows
  - `ViewModels/` - View models and business logic
- `tests/SmartAssistant.Tests` - Unit and integration tests
  - `Common/` - Core functionality tests
  - `UI/` - UI component tests

## Development

⚠️ **Developer Note**: Please read the [Development Guidelines](DEVELOPMENT_GUIDELINES.md) before starting any development work.

### Prerequisites

- .NET 8.0 SDK
- Visual Studio 2022 or JetBrains Rider
- Git

### Building

```bash
dotnet build
```

### Testing

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# Generate coverage report
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

### Quality Standards

- Unit Test Coverage: Minimum 80%
- Code Review Required: All PRs
- Continuous Integration: All branches
- Static Code Analysis: Enabled

### Branch Structure

- `main` - Production-ready code
  - Protected branch
  - Requires PR approval
  - Must pass all tests
- `develop` - Development branch
  - Protected branch
  - Requires PR approval
  - Integration testing
- `feature/*` - Feature branches
  - Created from `develop`
  - Requires up-to-date with `develop`
- `bugfix/*` - Bug fix branches
  - Created from `develop`
  - High priority fixes
- `hotfix/*` - Production hot fixes
  - Created from `main`
  - Emergency fixes only

### Commit Messages

Format:
```
<type>(<scope>): <subject>

<body>

<footer>
```

Types:
- feat: New feature
- fix: Bug fix
- docs: Documentation
- style: Formatting
- refactor: Code restructure
- test: Testing
- chore: Maintenance

Example:
```
feat(ui): add message type indicator

- Add icon color converter
- Implement message type enum
- Update UI to show different colors

Closes #123
```

## Contributing

1. Fork the repository
2. Create your feature branch from `develop`
   ```bash
   git checkout develop
   git pull origin develop
   git checkout -b feature/your-feature
   ```
3. Implement your changes
   - Follow coding standards
   - Write unit tests (80% coverage)
   - Update documentation
4. Commit your changes
   - Follow commit message format
   - Keep commits atomic
5. Push to your branch
   ```bash
   git push origin feature/your-feature
   ```
6. Create a Pull Request
   - Target the `develop` branch
   - Fill out PR template
   - Request code review
   - Address review comments
7. Maintain your PR
   - Keep up-to-date with `develop`
   - Fix failing tests
   - Update based on feedback

## Code Review Process

1. Automated Checks
   - Build success
   - Tests passing
   - Coverage >= 80%
   - Static analysis
   - Style compliance

2. Manual Review
   - Code quality
   - Design patterns
   - Performance
   - Security
   - Documentation

3. Approval Requirements
   - 2 approving reviews
   - All automated checks pass
   - No unresolved comments

## CI/CD Pipeline

### Continuous Integration
- Triggered on:
  - Pull requests
  - Push to protected branches
- Steps:
  1. Build
  2. Unit tests
  3. Integration tests
  4. Code coverage
  5. Static analysis

### Continuous Deployment
- Environments:
  - Development (automatic)
  - Staging (manual)
  - Production (manual)
- Steps:
  1. Environment validation
  2. Dependency audit
  3. Deployment
  4. Smoke tests
  5. Monitoring

## License

MIT License
