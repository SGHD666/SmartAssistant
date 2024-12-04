# Configuration Guide

## Overview

This guide covers the configuration settings for SmartAssistant, including application settings, API keys, and development options.

## Configuration Files

### 1. appsettings.json

The main configuration file for the application.

```json
{
  "LLMServices": {
    "OpenAI": {
      "ApiKey": "your-api-key",
      "Model": "gpt-4",
      "Temperature": 0.7,
      "MaxTokens": 2000
    },
    "Claude": {
      "ApiKey": "your-api-key",
      "Model": "claude-2"
    },
    "QianWen": {
      "ApiKey": "your-api-key"
    }
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "UI": {
    "Theme": "Light",
    "AccentColor": "#4A90E2",
    "FontSize": "Normal"
  }
}
```

### 2. User Secrets

For development, sensitive information should be stored in user secrets:

```bash
# Initialize user secrets
dotnet user-secrets init --project src/SmartAssistant.UI

# Set OpenAI API key
dotnet user-secrets set "LLMServices:OpenAI:ApiKey" "your-api-key"
```

### 3. Environment Variables

Production settings can be configured through environment variables:

```bash
# Windows
set SMARTASSISTANT_OPENAI_APIKEY=your-api-key

# Linux/macOS
export SMARTASSISTANT_OPENAI_APIKEY=your-api-key
```

## Settings Reference

### LLM Service Settings

#### OpenAI Configuration
```json
{
  "OpenAI": {
    "ApiKey": "string",          // Your OpenAI API key
    "Model": "string",           // Model name (e.g., "gpt-4")
    "Temperature": "number",      // 0.0 to 1.0
    "MaxTokens": "number",       // Maximum tokens per request
    "TopP": "number",            // 0.0 to 1.0
    "FrequencyPenalty": "number" // -2.0 to 2.0
  }
}
```

#### Claude Configuration
```json
{
  "Claude": {
    "ApiKey": "string",     // Your Claude API key
    "Model": "string",      // Model name
    "MaxTokens": "number"   // Maximum tokens per request
  }
}
```

### Logging Configuration

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "string",      // Minimum level to log
      "Microsoft": "string",    // Override for Microsoft namespaces
      "System": "string"        // Override for System namespaces
    },
    "File": {
      "Path": "string",         // Log file path
      "RollingInterval": "Day"  // Log rotation interval
    }
  }
}
```

### UI Configuration

```json
{
  "UI": {
    "Theme": "string",          // "Light" or "Dark"
    "AccentColor": "string",    // Hex color code
    "FontSize": "string",       // "Small", "Normal", "Large"
    "FontFamily": "string",     // Font family name
    "Animations": "boolean"     // Enable/disable animations
  }
}
```

## Environment-Specific Configuration

### Development
```json
{
  "Environment": "Development",
  "DetailedErrors": true,
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

### Production
```json
{
  "Environment": "Production",
  "DetailedErrors": false,
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

## Configuration Providers

Priority order (highest to lowest):
1. Command-line arguments
2. Environment variables
3. User secrets (Development)
4. appsettings.{Environment}.json
5. appsettings.json

## Best Practices

1. **Security**
   - Never commit API keys
   - Use user secrets in development
   - Use secure key management in production

2. **Environment Separation**
   - Use different settings per environment
   - Keep production settings minimal
   - Enable detailed logging in development

3. **Maintenance**
   - Document all configuration options
   - Version control configuration templates
   - Regular security audits of settings

## Troubleshooting

### Common Issues

1. **Missing API Keys**
   ```
   Error: API key not found
   Solution: Set API key in user secrets or environment variables
   ```

2. **Configuration Not Loading**
   ```
   Error: Configuration section missing
   Solution: Check file names and JSON structure
   ```

3. **Invalid Settings**
   ```
   Error: Invalid configuration value
   Solution: Verify setting types and allowed values
   ```

## Related Documentation
- [Quick Start Guide](./quickstart.md)
- [Development Environment Setup](./dev-environment.md)
- [Security Guidelines](../guides/security.md)
