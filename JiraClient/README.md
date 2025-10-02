# JiraClient

A .NET 8 client library for interacting with JIRA Data Center REST API.

## Features

- **Authentication**: Supports Personal Access Token (PAT) authentication via environment variable
- **Work Item Creation**: Create user stories and test-related work items
- **No Third-Party Dependencies**: Built using only .NET standard libraries
- **Async/Await**: Modern async patterns throughout

## Authentication

The client authenticates using a Personal Access Token (PAT) stored in an environment variable. By default, it looks for the `JIRA_PAT` environment variable.

```bash
# Set the environment variable
export JIRA_PAT="your-personal-access-token"
```

## Usage

### Creating a Client

```csharp
using JiraClient;

// Create client from environment variable (default: JIRA_PAT)
var client = JiraApiClient.CreateFromEnvironment("https://jira.example.com");

// Or specify a custom environment variable name
var client = JiraApiClient.CreateFromEnvironment("https://jira.example.com", "MY_JIRA_TOKEN");

// Or create directly with a token
var client = new JiraApiClient("https://jira.example.com", "your-token");
```

### Creating a User Story

```csharp
var response = await client.CreateUserStoryAsync(
    projectKey: "PROJ",
    summary: "As a user, I want to be able to login",
    description: "Detailed description of the story");

Console.WriteLine($"Created story: {response.Key}");
```

### Creating a Test Item

```csharp
var response = await client.CreateTestItemAsync(
    projectKey: "PROJ",
    summary: "Test login functionality",
    description: "Verify that users can successfully login");

Console.WriteLine($"Created test: {response.Key}");
```

### Creating a Custom Issue Type

```csharp
var response = await client.CreateIssueAsync(
    projectKey: "PROJ",
    issueTypeName: "Bug",
    summary: "Login button not working",
    description: "The login button does not respond to clicks");

Console.WriteLine($"Created issue: {response.Key}");
```

### Adding Custom Fields

```csharp
var customFields = new Dictionary<string, object>
{
    { "customfield_10001", "High" },
    { "customfield_10002", new[] { "tag1", "tag2" } }
};

var response = await client.CreateUserStoryAsync(
    projectKey: "PROJ",
    summary: "Story with custom fields",
    description: "This story has custom fields",
    additionalFields: customFields);
```

### Getting an Issue

```csharp
var issue = await client.GetIssueAsync("PROJ-123");

Console.WriteLine($"Issue: {issue.Key}");
Console.WriteLine($"Summary: {issue.Fields?.Summary}");
Console.WriteLine($"Description: {issue.Fields?.Description}");
```

## Models

The library includes the following models:

- `Issue`: Represents a JIRA issue/work item
- `IssueFields`: Contains all fields of an issue
- `IssueType`: Represents issue types (Story, Bug, Test, etc.)
- `Project`: Represents a JIRA project
- `User`: Represents a JIRA user
- `Priority`: Represents issue priority
- `CreateIssueRequest`: Request model for creating issues
- `CreateIssueResponse`: Response model after creating issues
- `ErrorResponse`: Error response from JIRA API

## Error Handling

The client throws `HttpRequestException` when API calls fail, including detailed error messages from JIRA:

```csharp
try
{
    var response = await client.CreateUserStoryAsync("PROJ", "My Story");
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Failed to create story: {ex.Message}");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Configuration error: {ex.Message}");
}
```

## Disposal

The client implements `IDisposable`. Ensure proper disposal:

```csharp
using var client = JiraApiClient.CreateFromEnvironment("https://jira.example.com");
// Use client...
```

## Supported JIRA Versions

This library targets JIRA Data Center and uses the JIRA REST API v2.

## License

Apache License 2.0
