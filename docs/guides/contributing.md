# Contributing Guide

## Getting Started

Thank you for considering contributing to SmartAssistant! This guide will help you understand our development process.

## Code of Conduct

- Be respectful and inclusive
- Focus on constructive feedback
- Help others learn and grow
- Follow project guidelines

## Development Process

### 1. Setting Up Development Environment

1. Fork the repository
2. Clone your fork:
   ```bash
   git clone https://github.com/YOUR-USERNAME/SmartAssistant.git
   cd SmartAssistant
   ```
3. Set up development environment following [dev-environment.md](../setup/dev-environment.md)

### 2. Creating a Branch

```bash
# Update your main
git checkout develop
git pull origin develop

# Create a branch
git checkout -b feature/your-feature-name
```

Branch naming conventions:
- `feature/*` - New features
- `bugfix/*` - Bug fixes
- `docs/*` - Documentation changes
- `refactor/*` - Code refactoring
- `test/*` - Test additions or changes

### 3. Making Changes

1. Follow our [Code Style Guide](./code-style.md)
2. Write tests for new features
3. Update documentation as needed
4. Keep commits atomic and focused

### 4. Commit Messages

Format:
```
type(scope): short description

Long description if needed

Closes #123
```

Types:
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation
- `style`: Formatting
- `refactor`: Code restructure
- `test`: Testing
- `chore`: Maintenance

Example:
```
feat(ui): add message type indicator

- Add icon color converter
- Implement message type enum
- Update UI to show different colors

Closes #123
```

### 5. Pull Request Process

1. Update your branch with latest changes:
   ```bash
   git checkout develop
   git pull origin develop
   git checkout your-branch
   git rebase develop
   ```

2. Push your changes:
   ```bash
   git push origin your-branch
   ```

3. Create Pull Request:
   - Use clear title and description
   - Reference related issues
   - Fill out PR template
   - Request reviews

4. Address Review Comments:
   - Make requested changes
   - Push updates
   - Respond to comments
   - Request re-review

### 6. Review Process

Your PR will be reviewed for:
- Code quality
- Test coverage
- Documentation
- Performance
- Security

Requirements for merge:
- Passing CI checks
- Code review approval
- No merge conflicts
- Up-to-date with develop

## Testing

### Running Tests
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/SmartAssistant.Tests

# Run with coverage
dotnet test /p:CollectCoverage=true
```

### Writing Tests
- Follow [Testing Guide](./testing.md)
- Maintain high coverage
- Test edge cases
- Write clear test names

## Documentation

### Types of Documentation
1. **Code Documentation**
   - XML comments
   - Method documentation
   - Class documentation

2. **Technical Documentation**
   - API documentation
   - Architecture guides
   - Setup instructions

3. **User Documentation**
   - User guides
   - Feature documentation
   - FAQ

### Documentation Guidelines
- Keep it clear and concise
- Include examples
- Update when changing code
- Check for accuracy

## Issue Reports

### Creating Issues
1. Check existing issues
2. Use issue templates
3. Provide clear description
4. Include reproduction steps
5. Add relevant labels

### Issue Template
```markdown
### Description
[Clear description of the issue]

### Steps to Reproduce
1. [First Step]
2. [Second Step]
3. [Additional Steps...]

### Expected Behavior
[What you expected to happen]

### Actual Behavior
[What actually happened]

### Environment
- OS: [e.g., Windows 11]
- .NET Version: [e.g., 8.0]
- App Version: [e.g., 1.0.0]
```

## Release Process

### Version Numbers
We use Semantic Versioning:
- MAJOR.MINOR.PATCH
- Major: Breaking changes
- Minor: New features
- Patch: Bug fixes

### Release Steps
1. Update version numbers
2. Update CHANGELOG.md
3. Create release branch
4. Run full test suite
5. Create release tag
6. Deploy to production

## Getting Help

- Check documentation first
- Search existing issues
- Ask in discussions
- Join developer chat

## Recognition

Contributors are recognized in:
- CONTRIBUTORS.md
- Release notes
- Project documentation

## Related Documentation
- [Development Guidelines](./development-guidelines.md)
- [Code Style Guide](./code-style.md)
- [Testing Guide](./testing.md)
