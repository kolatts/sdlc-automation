# SdlcAutomation.AzureDevOps

A .NET 8 class library providing a client layer for Azure DevOps operations.

## Features

- Authenticate using Personal Access Token (PAT) from environment variable
- Retrieve work items by ID or type (Feature, Epic, User Story, Task)
- Update work items with field changes
- Query work items by type

## Installation

Add a reference to this project in your application:

```bash
dotnet add reference path/to/SdlcAutomation.AzureDevOps/SdlcAutomation.AzureDevOps.csproj
```

## Configuration

Set your Azure DevOps Personal Access Token as an environment variable:

```bash
# Default environment variable name
export AZURE_DEVOPS_PAT="your-pat-token-here"

# Or use a custom environment variable name
export MY_CUSTOM_PAT="your-pat-token-here"
```

## Usage

### Basic Usage

```csharp
using SdlcAutomation.AzureDevOps;

// Create client (uses AZURE_DEVOPS_PAT by default)
using var client = new AzureDevOpsClient(
    "https://dev.azure.com/your-organization",
    "YourProjectName"
);

// Or specify a custom environment variable
using var customClient = new AzureDevOpsClient(
    "https://dev.azure.com/your-organization",
    "YourProjectName",
    "MY_CUSTOM_PAT"
);
```

### Get a Work Item

```csharp
var workItem = await client.GetWorkItemAsync(123);
Console.WriteLine($"Title: {workItem.Fields["System.Title"]}");
```

### Get Multiple Work Items

```csharp
var ids = new[] { 123, 456, 789 };
var workItems = await client.GetWorkItemsAsync(ids);
```

### Query Work Items by Type

```csharp
// Using constants
var features = await client.QueryWorkItemsByTypeAsync(WorkItemTypes.Feature);
var epics = await client.QueryWorkItemsByTypeAsync(WorkItemTypes.Epic);
var stories = await client.QueryWorkItemsByTypeAsync(WorkItemTypes.Story);
var tasks = await client.QueryWorkItemsByTypeAsync(WorkItemTypes.Task);

// Or using string literals
var customItems = await client.QueryWorkItemsByTypeAsync("Bug");
```

### Update a Work Item

```csharp
var fields = new Dictionary<string, object>
{
    { "System.Title", "Updated Title" },
    { "System.State", "Active" },
    { "System.AssignedTo", "user@example.com" }
};

var updated = await client.UpdateWorkItemAsync(123, fields);
```

## Work Item Types

The library provides constants for common work item types:

- `WorkItemTypes.Feature` - "Feature"
- `WorkItemTypes.Epic` - "Epic"
- `WorkItemTypes.Story` - "User Story"
- `WorkItemTypes.Task` - "Task"

## Error Handling

```csharp
try
{
    using var client = new AzureDevOpsClient(
        "https://dev.azure.com/your-org",
        "YourProject"
    );
    
    var workItem = await client.GetWorkItemAsync(123);
}
catch (InvalidOperationException ex)
{
    // PAT not found in environment variable
    Console.Error.WriteLine($"Configuration error: {ex.Message}");
}
catch (Exception ex)
{
    // Other errors (network, API errors, etc.)
    Console.Error.WriteLine($"Error: {ex.Message}");
}
```

## Dependencies

- Microsoft.TeamFoundationServer.Client (v19.225.1)

## License

Licensed under the Apache License, Version 2.0. See the LICENSE file in the repository root for details.
