# JiraClient Architecture

## Overview

The JiraClient library is a lightweight .NET 8 client for interacting with JIRA Data Center REST API v2. It follows clean architecture principles and uses no third-party NuGet packages.

## Design Decisions

### Authentication
- **Bearer Token (PAT)**: Uses Personal Access Token authentication compatible with JIRA Data Center
- **Environment Variable**: Token is sourced from environment variables (default: `JIRA_PAT`) for security
- **Custom Handler**: Implements `DelegatingHandler` to inject authentication headers into all HTTP requests

### No Third-Party Dependencies
The library uses only .NET standard libraries:
- `System.Net.Http` for HTTP communication
- `System.Text.Json` for JSON serialization/deserialization
- `System.Net.Http.Json` for simplified JSON HTTP operations

### Project Structure

```
JiraClient/
├── Auth/                           # Authentication handlers
│   └── JiraAuthenticationHandler.cs
├── Models/                         # Data models
│   ├── Issue.cs                    # Main issue representation
│   ├── IssueFields.cs             # Issue fields with custom field support
│   ├── IssueType.cs               # Issue type (Story, Test, Bug, etc.)
│   ├── Project.cs                 # JIRA project
│   ├── User.cs                    # JIRA user
│   ├── Priority.cs                # Priority levels
│   ├── CreateIssueRequest.cs      # Request model for issue creation
│   ├── CreateIssueResponse.cs     # Response model for issue creation
│   └── ErrorResponse.cs           # Error response handling
├── JiraApiClient.cs               # Main API client
└── JiraClient.csproj              # Project file
```

## Key Components

### JiraAuthenticationHandler
- Extends `DelegatingHandler` to intercept HTTP requests
- Adds `Authorization: Bearer <token>` header to all requests
- Compatible with JIRA Data Center PAT authentication

### JiraApiClient
The main entry point for API operations.

**Key Methods:**
- `CreateFromEnvironment(baseUrl, tokenEnvVar)` - Factory method using environment variables
- `CreateIssueAsync(...)` - Generic issue creation
- `CreateUserStoryAsync(...)` - Specialized method for user stories
- `CreateTestItemAsync(...)` - Specialized method for test items
- `GetIssueAsync(issueKey)` - Retrieve an issue by key

**Features:**
- Async/await pattern throughout
- Proper error handling with detailed error messages
- Support for custom fields via dictionary
- IDisposable implementation for proper resource cleanup

### Models
- Use `System.Text.Json` attributes for serialization
- Support nullable reference types for better null safety
- `IssueFields` includes `JsonExtensionData` for custom fields

## API Design

### Fluent Configuration
```csharp
var client = JiraApiClient.CreateFromEnvironment("https://jira.example.com");
```

### Method Overloads
- Generic `CreateIssueAsync` for any issue type
- Specialized methods (`CreateUserStoryAsync`, `CreateTestItemAsync`) for common types
- Optional parameters for flexibility

### Error Handling
- Throws `HttpRequestException` for API errors with detailed messages
- Throws `InvalidOperationException` for configuration errors
- Parses JIRA error responses when available

## JIRA Data Center Compatibility

The library uses JIRA REST API v2 endpoints:
- `/rest/api/2/issue` - Create and retrieve issues
- Compatible with JIRA Data Center 8.x and 9.x

## Extension Points

### Custom Fields
Custom fields can be added via the `additionalFields` parameter:
```csharp
var customFields = new Dictionary<string, object>
{
    { "customfield_10001", "value" }
};
```

### New Issue Types
Any issue type can be created using `CreateIssueAsync`:
```csharp
await client.CreateIssueAsync(projectKey, "Epic", summary, description);
```

## Security Considerations

1. **No Token Hardcoding**: Token must be in environment variable
2. **HTTPS Only**: BaseURL should always use HTTPS in production
3. **Token Disposal**: HttpClient properly disposed via IDisposable
4. **No Token Logging**: Token never logged or exposed in error messages

## Performance Considerations

1. **Single HttpClient**: Reuses HttpClient instance (recommended pattern)
2. **Async Operations**: All I/O operations are async
3. **Streaming**: Uses `HttpContent.ReadFromJsonAsync` for efficient deserialization
4. **Cancellation Support**: All async methods accept `CancellationToken`

## Future Enhancements

Potential areas for extension:
- Issue update/delete operations
- Issue search and querying
- Attachment handling
- Comment operations
- Transition management
- Bulk operations
- Retry policies with exponential backoff
