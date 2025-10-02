# Example Usage

This document provides complete examples for using the Azure DevOps client.

## Setting Up Environment

First, set your Personal Access Token (PAT):

```bash
export AZURE_DEVOPS_PAT="your-pat-token-here"
```

## Complete Console Application Example

```csharp
using SdlcAutomation.AzureDevOps;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace MyApp;

class Program
{
    static async Task Main(string[] args)
    {
        const string orgUrl = "https://dev.azure.com/your-organization";
        const string project = "YourProjectName";
        
        try
        {
            using var client = new AzureDevOpsClient(orgUrl, project);
            
            // Example 1: Get a single work item
            Console.WriteLine("=== Getting Work Item 123 ===");
            var workItem = await client.GetWorkItemAsync(123);
            PrintWorkItem(workItem);
            
            // Example 2: Query all features
            Console.WriteLine("\n=== Querying Features ===");
            var features = await client.QueryWorkItemsByTypeAsync(WorkItemType.Feature);
            Console.WriteLine($"Found {features.Count()} features");
            foreach (var feature in features.Take(5))
            {
                PrintWorkItem(feature);
            }
            
            // Example 3: Query all epics
            Console.WriteLine("\n=== Querying Epics ===");
            var epics = await client.QueryWorkItemsByTypeAsync(WorkItemType.Epic);
            Console.WriteLine($"Found {epics.Count()} epics");
            
            // Example 4: Query stories
            Console.WriteLine("\n=== Querying User Stories ===");
            var stories = await client.QueryWorkItemsByTypeAsync(WorkItemType.Story);
            Console.WriteLine($"Found {stories.Count()} stories");
            
            // Example 5: Query tasks
            Console.WriteLine("\n=== Querying Tasks ===");
            var tasks = await client.QueryWorkItemsByTypeAsync(WorkItemType.Task);
            Console.WriteLine($"Found {tasks.Count()} tasks");
            
            // Example 6: Get multiple work items
            Console.WriteLine("\n=== Getting Multiple Work Items ===");
            var ids = new[] { 123, 456, 789 };
            var workItems = await client.GetWorkItemsAsync(ids);
            foreach (var wi in workItems)
            {
                PrintWorkItem(wi);
            }
            
            // Example 7: Update a work item
            Console.WriteLine("\n=== Updating Work Item ===");
            var fieldsToUpdate = new Dictionary<string, object>
            {
                { "System.State", "Active" },
                { "System.Tags", "automated-update" }
            };
            
            var updated = await client.UpdateWorkItemAsync(123, fieldsToUpdate);
            Console.WriteLine($"Updated work item {updated.Id}");
            PrintWorkItem(updated);
        }
        catch (InvalidOperationException ex)
        {
            Console.Error.WriteLine($"Configuration error: {ex.Message}");
            Console.Error.WriteLine("Please set the AZURE_DEVOPS_PAT environment variable.");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
        }
    }
    
    static void PrintWorkItem(WorkItem wi)
    {
        Console.WriteLine($"  ID: {wi.Id}");
        
        if (wi.Fields != null)
        {
            if (wi.Fields.ContainsKey("System.Title"))
                Console.WriteLine($"  Title: {wi.Fields["System.Title"]}");
            
            if (wi.Fields.ContainsKey("System.WorkItemType"))
                Console.WriteLine($"  Type: {wi.Fields["System.WorkItemType"]}");
            
            if (wi.Fields.ContainsKey("System.State"))
                Console.WriteLine($"  State: {wi.Fields["System.State"]}");
            
            if (wi.Fields.ContainsKey("System.AssignedTo"))
                Console.WriteLine($"  Assigned To: {wi.Fields["System.AssignedTo"]}");
        }
        
        Console.WriteLine();
    }
}
```

## Working with Different Work Item Types

```csharp
// Features
var features = await client.QueryWorkItemsByTypeAsync(WorkItemType.Feature);

// Epics
var epics = await client.QueryWorkItemsByTypeAsync(WorkItemType.Epic);

// User Stories
var stories = await client.QueryWorkItemsByTypeAsync(WorkItemType.Story);

// Tasks
var tasks = await client.QueryWorkItemsByTypeAsync(WorkItemType.Task);

// Custom types (e.g., Bug)
var bugs = await client.QueryWorkItemsByTypeAsync("Bug");
```

## Working with Simplified Work Item Models

```csharp
using SdlcAutomation.AzureDevOps.Models;

// Get a single work item as a model
var model = await client.GetWorkItemModelAsync(123);
Console.WriteLine($"Title: {model.Title}");
Console.WriteLine($"Description: {model.Description}");
Console.WriteLine($"Acceptance Criteria: {model.AcceptanceCriteria}");
Console.WriteLine($"Assigned To: {model.AssignedTo}");
Console.WriteLine($"Created: {model.CreatedDate}");
Console.WriteLine($"Changed: {model.ChangedDate}");

// Load with children and parents
var options = new WorkItemLoadOptions
{
    LoadChildren = true,
    LoadParents = true
};

var modelWithRelations = await client.GetWorkItemModelAsync(123, options);

if (modelWithRelations.Children != null)
{
    Console.WriteLine($"Children ({modelWithRelations.Children.Count}):");
    foreach (var child in modelWithRelations.Children)
    {
        Console.WriteLine($"  - [{child.Id}] {child.Title}");
    }
}

if (modelWithRelations.Parents != null)
{
    Console.WriteLine($"Parents ({modelWithRelations.Parents.Count}):");
    foreach (var parent in modelWithRelations.Parents)
    {
        Console.WriteLine($"  - [{parent.Id}] {parent.Title}");
    }
}

// Load with commits and pull requests
var fullOptions = new WorkItemLoadOptions
{
    LoadChildren = true,
    LoadParents = true,
    LoadCommits = true,
    LoadPullRequests = true
};

var fullModel = await client.GetWorkItemModelAsync(123, fullOptions);

if (fullModel.Commits != null)
{
    Console.WriteLine($"Commits ({fullModel.Commits.Count}):");
    foreach (var commit in fullModel.Commits)
    {
        Console.WriteLine($"  - {commit.CommitId}: {commit.Comment}");
    }
}

if (fullModel.PullRequests != null)
{
    Console.WriteLine($"Pull Requests ({fullModel.PullRequests.Count}):");
    foreach (var pr in fullModel.PullRequests)
    {
        Console.WriteLine($"  - [{pr.PullRequestId}] {pr.Title}");
    }
}

// Or use the convenience property for all options
var allData = await client.GetWorkItemModelAsync(123, WorkItemLoadOptions.All);

// Query multiple work items as models
var featureModels = await client.QueryWorkItemModelsByTypeAsync(WorkItemType.Feature, options);
foreach (var feature in featureModels)
{
    Console.WriteLine($"Feature: {feature.Title}");
    Console.WriteLine($"  State: {feature.State}");
    Console.WriteLine($"  Children: {feature.Children?.Count ?? 0}");
}
```

## Updating Work Items

### Update Title and State

```csharp
var fields = new Dictionary<string, object>
{
    { "System.Title", "New Title" },
    { "System.State", "Active" }
};

var updated = await client.UpdateWorkItemAsync(123, fields);
```

### Update Assignment

```csharp
var fields = new Dictionary<string, object>
{
    { "System.AssignedTo", "user@example.com" }
};

var updated = await client.UpdateWorkItemAsync(123, fields);
```

### Update Multiple Fields

```csharp
var fields = new Dictionary<string, object>
{
    { "System.Title", "Updated Feature Title" },
    { "System.State", "Resolved" },
    { "System.AssignedTo", "user@example.com" },
    { "System.Tags", "important;automated" },
    { "Microsoft.VSTS.Common.Priority", 1 }
};

var updated = await client.UpdateWorkItemAsync(123, fields);
```

## Error Handling

```csharp
try
{
    using var client = new AzureDevOpsClient(orgUrl, project);
    var workItem = await client.GetWorkItemAsync(123);
}
catch (InvalidOperationException ex) when (ex.Message.Contains("Personal Access Token"))
{
    // PAT not configured
    Console.Error.WriteLine("Please set AZURE_DEVOPS_PAT environment variable");
}
catch (Microsoft.VisualStudio.Services.WebApi.VssServiceException ex) when (ex.Message.Contains("404"))
{
    // Work item not found
    Console.Error.WriteLine($"Work item not found: {ex.Message}");
}
catch (Microsoft.VisualStudio.Services.WebApi.VssServiceException ex) when (ex.Message.Contains("401"))
{
    // Authentication failed
    Console.Error.WriteLine("Authentication failed. Check your PAT.");
}
catch (Exception ex)
{
    // Other errors
    Console.Error.WriteLine($"Unexpected error: {ex.Message}");
}
```

## Custom Environment Variable

If you want to use a different environment variable for the PAT:

```csharp
// Set custom env var
Environment.SetEnvironmentVariable("MY_CUSTOM_PAT", "your-pat-here");

// Create client with custom env var
using var client = new AzureDevOpsClient(
    "https://dev.azure.com/your-org",
    "YourProject",
    "MY_CUSTOM_PAT"
);
```

Or from command line:

```bash
export MY_CUSTOM_PAT="your-pat-token-here"
```

## Common Work Item Fields

Here are some commonly used work item fields:

- `System.Title` - Work item title
- `System.Description` - Work item description
- `System.State` - Current state (New, Active, Resolved, Closed, etc.)
- `System.WorkItemType` - Type of work item
- `System.AssignedTo` - Person assigned to the work item
- `System.Tags` - Semicolon-separated tags
- `System.AreaPath` - Area path
- `System.IterationPath` - Iteration path
- `Microsoft.VSTS.Common.Priority` - Priority (1-4)
- `Microsoft.VSTS.Common.Severity` - Severity (1-4)
- `Microsoft.VSTS.Scheduling.Effort` - Effort/story points

## Integration with Main Console App

To use this in your main SdlcAutomation console app:

1. Add project reference:
   ```bash
   cd SdlcAutomation
   dotnet add reference ../SdlcAutomation.AzureDevOps/SdlcAutomation.AzureDevOps.csproj
   ```

2. Create a command (e.g., `AzureDevOpsCommand.cs`):
   ```csharp
   using SdlcAutomation.AzureDevOps;
   
   public class AzureDevOpsCommand : BaseCommand
   {
       public AzureDevOpsCommand() : base("ado", "Azure DevOps operations")
       {
           // Add your options and handlers
       }
   }
   ```

3. Add to RootCommand:
   ```csharp
   public RootCommand() : base("sdlc", "SDLC Automation Tool")
   {
       AddCommand(new AzureDevOpsCommand());
   }
   ```
