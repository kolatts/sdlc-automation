# JiraClient Quick Start Guide

## Installation

Add a project reference to the JiraClient library:

```bash
dotnet add reference path/to/JiraClient/JiraClient.csproj
```

## Setup

Set your JIRA credentials as environment variables:

```bash
export JIRA_BASE_URL="https://your-jira-instance.com"
export JIRA_PAT="your-personal-access-token"
```

### Getting a Personal Access Token (PAT)

In JIRA Data Center:
1. Go to your profile settings
2. Navigate to "Personal Access Tokens"
3. Click "Create token"
4. Give it a name and set appropriate permissions
5. Copy the generated token (you won't see it again!)

## Basic Usage

### 1. Create a Client

```csharp
using JiraClient;

// Use default environment variable (JIRA_PAT)
using var client = JiraApiClient.CreateFromEnvironment(
    Environment.GetEnvironmentVariable("JIRA_BASE_URL")!);
```

### 2. Create a User Story

```csharp
var story = await client.CreateUserStoryAsync(
    projectKey: "MYPROJ",
    summary: "As a user, I want to login to the system",
    description: "Users should be able to login with email and password"
);

Console.WriteLine($"Created story: {story.Key}");
// Output: Created story: MYPROJ-123
```

### 3. Create a Test Item

```csharp
var test = await client.CreateTestItemAsync(
    projectKey: "MYPROJ",
    summary: "Verify login with valid credentials",
    description: "Test that users can login with correct email and password"
);

Console.WriteLine($"Created test: {test.Key}");
// Output: Created test: MYPROJ-124
```

### 4. Create Any Issue Type

```csharp
var bug = await client.CreateIssueAsync(
    projectKey: "MYPROJ",
    issueTypeName: "Bug",
    summary: "Login button not responding",
    description: "The login button does not respond to clicks on mobile"
);

Console.WriteLine($"Created bug: {bug.Key}");
// Output: Created bug: MYPROJ-125
```

### 5. Add Custom Fields

```csharp
var customFields = new Dictionary<string, object>
{
    ["customfield_10001"] = "High",
    ["customfield_10002"] = new[] { "sprint-1", "backend" }
};

var story = await client.CreateUserStoryAsync(
    projectKey: "MYPROJ",
    summary: "Story with custom fields",
    description: "This story includes custom field values",
    additionalFields: customFields
);
```

### 6. Retrieve an Issue

```csharp
var issue = await client.GetIssueAsync("MYPROJ-123");

Console.WriteLine($"Summary: {issue.Fields?.Summary}");
Console.WriteLine($"Type: {issue.Fields?.IssueType?.Name}");
Console.WriteLine($"Description: {issue.Fields?.Description}");
```

## Error Handling

```csharp
try
{
    var story = await client.CreateUserStoryAsync(
        "MYPROJ",
        "My Story"
    );
}
catch (HttpRequestException ex)
{
    // API errors (e.g., invalid project, authentication failed)
    Console.WriteLine($"API Error: {ex.Message}");
}
catch (InvalidOperationException ex)
{
    // Configuration errors (e.g., missing environment variable)
    Console.WriteLine($"Config Error: {ex.Message}");
}
```

## Complete Example

```csharp
using JiraClient;

// Setup
var baseUrl = Environment.GetEnvironmentVariable("JIRA_BASE_URL");
if (string.IsNullOrWhiteSpace(baseUrl))
{
    Console.WriteLine("Please set JIRA_BASE_URL environment variable");
    return;
}

try
{
    // Create client
    using var client = JiraApiClient.CreateFromEnvironment(baseUrl);
    
    // Create a user story
    var story = await client.CreateUserStoryAsync(
        projectKey: "MYPROJ",
        summary: "Implement user authentication",
        description: "Users need to be able to login and logout"
    );
    
    Console.WriteLine($"✓ Created story: {story.Key}");
    Console.WriteLine($"  View at: {story.Self}");
    
    // Create associated test
    var test = await client.CreateTestItemAsync(
        projectKey: "MYPROJ",
        summary: "Test user authentication flow",
        description: "Verify login and logout functionality"
    );
    
    Console.WriteLine($"✓ Created test: {test.Key}");
    
    // Retrieve and display the story
    var retrievedStory = await client.GetIssueAsync(story.Key!);
    Console.WriteLine($"\nStory details:");
    Console.WriteLine($"  Summary: {retrievedStory.Fields?.Summary}");
    Console.WriteLine($"  Type: {retrievedStory.Fields?.IssueType?.Name}");
}
catch (Exception ex)
{
    Console.WriteLine($"✗ Error: {ex.Message}");
}
```

## Tips

1. **Reuse the Client**: Create one `JiraApiClient` instance and reuse it for multiple operations
2. **Use `using` Statement**: Always dispose of the client properly with `using`
3. **Check Environment Variables**: Validate environment variables are set before creating the client
4. **Handle Errors**: Always wrap API calls in try-catch blocks
5. **Use Async/Await**: All methods are async - use `await` properly
6. **Custom Fields**: Find custom field IDs in JIRA admin or by inspecting existing issues

## Next Steps

- Read the full [README.md](README.md) for detailed API documentation
- Check [ARCHITECTURE.md](ARCHITECTURE.md) for design and implementation details
- See [JiraClient.Example](../JiraClient.Example/) for a complete working example
