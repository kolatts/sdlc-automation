using System.CommandLine;
using SdlcAutomation.AzureDevOps;
using SdlcAutomation.AzureDevOps.Models;
using Spectre.Console;

namespace SdlcAutomation.Commands;

/// <summary>
/// Command for Azure DevOps operations
/// </summary>
public class AzureDevOpsCommand : BaseCommand
{
    public AzureDevOpsCommand() : base("ado", "Azure DevOps operations")
    {
        var queryCommand = new Command("query", "Query work items from Azure DevOps");
        
        var orgOption = new Option<string>(
            "--organization",
            "Azure DevOps organization URL (e.g., https://dev.azure.com/your-org)"
        ) { IsRequired = true };
        
        var projectOption = new Option<string>(
            "--project",
            "Project name"
        ) { IsRequired = true };
        
        var typeOption = new Option<string>(
            "--type",
            () => "Feature",
            "Work item type (Feature, Epic, Story, Task)"
        );
        
        var loadChildrenOption = new Option<bool>(
            "--load-children",
            () => false,
            "Load child work items"
        );
        
        var loadParentsOption = new Option<bool>(
            "--load-parents",
            () => false,
            "Load parent work items"
        );
        
        var loadCommitsOption = new Option<bool>(
            "--load-commits",
            () => false,
            "Load associated commits"
        );
        
        var loadPullRequestsOption = new Option<bool>(
            "--load-pull-requests",
            () => false,
            "Load associated pull requests"
        );
        
        queryCommand.AddOption(orgOption);
        queryCommand.AddOption(projectOption);
        queryCommand.AddOption(typeOption);
        queryCommand.AddOption(loadChildrenOption);
        queryCommand.AddOption(loadParentsOption);
        queryCommand.AddOption(loadCommitsOption);
        queryCommand.AddOption(loadPullRequestsOption);
        
        queryCommand.SetHandler(
            ExecuteQueryAsync,
            orgOption,
            projectOption,
            typeOption,
            loadChildrenOption,
            loadParentsOption,
            loadCommitsOption,
            loadPullRequestsOption
        );
        
        AddCommand(queryCommand);
    }
    
    private async Task ExecuteQueryAsync(
        string organization,
        string project,
        string type,
        bool loadChildren,
        bool loadParents,
        bool loadCommits,
        bool loadPullRequests)
    {
        try
        {
            WriteInfo($"Connecting to Azure DevOps organization: {organization}");
            WriteInfo($"Project: {project}");
            
            using var client = new AzureDevOpsClient(organization, project);
            
            // Parse work item type
            if (!Enum.TryParse<WorkItemType>(type, ignoreCase: true, out var workItemType))
            {
                WriteError($"Invalid work item type: {type}");
                WriteInfo("Valid types: Feature, Epic, Story, Task");
                return;
            }
            
            WriteInfo($"Querying {type} work items...");
            
            // Configure load options
            var options = new WorkItemLoadOptions
            {
                LoadChildren = loadChildren,
                LoadParents = loadParents,
                LoadCommits = loadCommits,
                LoadPullRequests = loadPullRequests
            };
            
            var workItems = await client.QueryWorkItemModelsByTypeAsync(workItemType, options);
            var workItemsList = workItems.ToList();
            
            if (!workItemsList.Any())
            {
                WriteWarning($"No {type} work items found.");
                return;
            }
            
            WriteSuccess($"Found {workItemsList.Count} {type} work items");
            
            // Display results in a table
            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Title");
            table.AddColumn("State");
            table.AddColumn("Assigned To");
            
            if (loadChildren)
                table.AddColumn("Children");
            
            if (loadParents)
                table.AddColumn("Parents");
            
            if (loadCommits)
                table.AddColumn("Commits");
            
            if (loadPullRequests)
                table.AddColumn("PRs");
            
            foreach (var item in workItemsList.Take(10))
            {
                var row = new List<string>
                {
                    item.Id.ToString(),
                    item.Title ?? "(no title)",
                    item.State ?? "(no state)",
                    item.AssignedTo ?? "(unassigned)"
                };
                
                if (loadChildren)
                    row.Add((item.Children?.Count ?? 0).ToString());
                
                if (loadParents)
                    row.Add((item.Parents?.Count ?? 0).ToString());
                
                if (loadCommits)
                    row.Add((item.Commits?.Count ?? 0).ToString());
                
                if (loadPullRequests)
                    row.Add((item.PullRequests?.Count ?? 0).ToString());
                
                table.AddRow(row.ToArray());
            }
            
            AnsiConsole.Write(table);
            
            if (workItemsList.Count > 10)
            {
                WriteInfo($"Showing first 10 of {workItemsList.Count} results");
            }
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Personal Access Token"))
        {
            WriteError("Azure DevOps PAT not configured.");
            WriteInfo("Set the AZURE_DEVOPS_PAT environment variable with your Personal Access Token.");
            WriteInfo("Example: export AZURE_DEVOPS_PAT=\"your-pat-token\"");
        }
        catch (Exception ex)
        {
            WriteError($"Error: {ex.Message}");
        }
    }
}
