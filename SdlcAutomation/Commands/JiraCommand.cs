using System.CommandLine;
using SdlcAutomation.AzureDevOps;
using SdlcAutomation.AzureDevOps.Models;
using SdlcAutomation.Jira;
using SdlcAutomation.Jira.Models;
using Spectre.Console;

namespace SdlcAutomation.Commands;

public class JiraCommand : BaseCommand
{
    public JiraCommand() : base("jira", "JIRA integration commands")
    {
        var createCommand = new Command("create", "Create a JIRA work item");
        
        var projectOption = new Option<string>(
            "--project",
            "The JIRA project key (e.g., PROJ)") { IsRequired = true };
        
        var typeOption = new Option<string>(
            "--type",
            () => "Story",
            "The issue type (Story, Test, Bug, etc.)");
        
        var summaryOption = new Option<string>(
            "--summary",
            "The issue summary/title") { IsRequired = true };
        
        var descriptionOption = new Option<string?>(
            "--description",
            "The issue description");

        createCommand.AddOption(projectOption);
        createCommand.AddOption(typeOption);
        createCommand.AddOption(summaryOption);
        createCommand.AddOption(descriptionOption);

        createCommand.SetHandler(CreateIssue, projectOption, typeOption, summaryOption, descriptionOption);
        
        AddCommand(createCommand);

        // Add ado-to-jira command
        var adoToJiraCommand = new Command("ado-to-jira", "Convert an Azure DevOps work item to a JIRA issue");
        
        var adoOrgOption = new Option<string>(
            "--ado-organization",
            "Azure DevOps organization URL (e.g., https://dev.azure.com/your-org)") { IsRequired = true };
        
        var adoProjectOption = new Option<string>(
            "--ado-project",
            "Azure DevOps project name") { IsRequired = true };
        
        var workItemIdOption = new Option<int>(
            "--work-item-id",
            "Azure DevOps work item ID") { IsRequired = true };
        
        var jiraProjectOption = new Option<string>(
            "--jira-project",
            "JIRA project key (e.g., PROJ)") { IsRequired = true };
        
        var jiraIssueTypeOption = new Option<string>(
            "--jira-issue-type",
            () => "Story",
            "JIRA issue type (Story, Task, Bug, etc.)");
        
        adoToJiraCommand.AddOption(adoOrgOption);
        adoToJiraCommand.AddOption(adoProjectOption);
        adoToJiraCommand.AddOption(workItemIdOption);
        adoToJiraCommand.AddOption(jiraProjectOption);
        adoToJiraCommand.AddOption(jiraIssueTypeOption);
        
        adoToJiraCommand.SetHandler(
            ConvertAdoToJira, 
            adoOrgOption, 
            adoProjectOption, 
            workItemIdOption, 
            jiraProjectOption, 
            jiraIssueTypeOption);
        
        AddCommand(adoToJiraCommand);
    }

    private async Task CreateIssue(string project, string type, string summary, string? description)
    {
        try
        {
            // Create issue using the unified model
            var issue = new Issue
            {
                Fields = new IssueFields
                {
                    Project = new Project { Key = project },
                    IssueType = new IssueType { Name = type },
                    Summary = summary,
                    Description = description
                }
            };

            // Validate the issue
            var validationResults = issue.Validate();
            if (validationResults.Any())
            {
                WriteError("Validation failed:");
                foreach (var error in validationResults)
                {
                    WriteError($"  • {error.ErrorMessage}");
                }
                return;
            }

            var baseUrl = Environment.GetEnvironmentVariable("JIRA_BASE_URL");
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                WriteError("JIRA_BASE_URL environment variable is not set.");
                WriteInfo("Set it to your JIRA instance URL (e.g., https://jira.example.com)");
                return;
            }

            WriteInfo($"Connecting to JIRA at: {baseUrl}");
            
            using var client = JiraApiClient.CreateFromEnvironment(baseUrl);
            
            WriteInfo($"Creating {type} in project {project}...");
            
            var createdIssue = await client.CreateIssueAsync(issue);

            WriteSuccess($"Created {type}: {createdIssue.Key}");
            if (!string.IsNullOrEmpty(createdIssue.Self))
            {
                WriteInfo($"URL: {createdIssue.Self}");
            }
        }
        catch (InvalidOperationException ex)
        {
            WriteError($"Configuration error: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            WriteError($"API error: {ex.Message}");
        }
        catch (Exception ex)
        {
            WriteError($"Unexpected error: {ex.Message}");
        }
    }

    private async Task ConvertAdoToJira(
        string adoOrganization,
        string adoProject,
        int workItemId,
        string jiraProject,
        string jiraIssueType)
    {
        try
        {
            // Get JIRA base URL
            var jiraBaseUrl = Environment.GetEnvironmentVariable("JIRA_BASE_URL");
            if (string.IsNullOrWhiteSpace(jiraBaseUrl))
            {
                WriteError("JIRA_BASE_URL environment variable is not set.");
                WriteInfo("Set it to your JIRA instance URL (e.g., https://jira.example.com)");
                return;
            }

            WriteInfo($"Connecting to Azure DevOps: {adoOrganization}");
            WriteInfo($"Project: {adoProject}");
            
            // Create Azure DevOps client
            using var adoClient = new AzureDevOpsClient(adoOrganization, adoProject);
            
            WriteInfo($"Fetching work item {workItemId}...");
            
            // Get the work item
            var workItem = await adoClient.GetWorkItemModelAsync(workItemId);
            
            if (workItem == null)
            {
                WriteError($"Work item {workItemId} not found.");
                return;
            }

            WriteSuccess($"Retrieved work item: {workItem.Title}");
            WriteInfo($"  Type: {workItem.WorkItemType}");
            WriteInfo($"  State: {workItem.State}");
            WriteInfo($"  Assigned To: {workItem.AssignedTo ?? "(unassigned)"}");

            // Connect to JIRA
            WriteInfo($"Connecting to JIRA at: {jiraBaseUrl}");
            using var jiraClient = JiraApiClient.CreateFromEnvironment(jiraBaseUrl);
            
            // Get current JIRA user
            WriteInfo("Getting current JIRA user...");
            var currentUser = await jiraClient.GetCurrentUserAsync();
            WriteInfo($"  Current user: {currentUser.DisplayName ?? currentUser.Name}");

            // Build description from ADO work item
            var description = BuildJiraDescription(workItem);

            // Create JIRA issue
            var issue = new Issue
            {
                Fields = new IssueFields
                {
                    Project = new Project { Key = jiraProject },
                    IssueType = new IssueType { Name = jiraIssueType },
                    Summary = workItem.Title ?? $"ADO Work Item {workItem.Id}",
                    Description = description,
                    Reporter = currentUser,
                    Assignee = currentUser
                }
            };

            // Validate the issue
            var validationResults = issue.Validate();
            if (validationResults.Any())
            {
                WriteError("JIRA issue validation failed:");
                foreach (var error in validationResults)
                {
                    WriteError($"  • {error.ErrorMessage}");
                }
                return;
            }

            WriteInfo($"Creating JIRA {jiraIssueType} in project {jiraProject}...");
            var createdIssue = await jiraClient.CreateIssueAsync(issue);

            WriteSuccess($"✓ Created JIRA issue: {createdIssue.Key}");
            if (!string.IsNullOrEmpty(createdIssue.Self))
            {
                WriteInfo($"  URL: {createdIssue.Self}");
            }
            WriteInfo($"  Assignee: {currentUser.DisplayName ?? currentUser.Name}");
            WriteInfo($"  Reporter: {currentUser.DisplayName ?? currentUser.Name}");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Personal Access Token") || ex.Message.Contains("JIRA_PAT"))
        {
            WriteError($"Configuration error: {ex.Message}");
            WriteInfo("Make sure both AZURE_DEVOPS_PAT and JIRA_PAT environment variables are set.");
        }
        catch (HttpRequestException ex)
        {
            WriteError($"API error: {ex.Message}");
        }
        catch (Exception ex)
        {
            WriteError($"Unexpected error: {ex.Message}");
        }
    }

    private string BuildJiraDescription(WorkItemModel workItem)
    {
        var descriptionParts = new List<string>();

        // Add original description
        if (!string.IsNullOrWhiteSpace(workItem.Description))
        {
            descriptionParts.Add(workItem.Description);
        }

        // Add acceptance criteria if available
        if (!string.IsNullOrWhiteSpace(workItem.AcceptanceCriteria))
        {
            descriptionParts.Add("");
            descriptionParts.Add("h3. Acceptance Criteria");
            descriptionParts.Add(workItem.AcceptanceCriteria);
        }

        // Add metadata
        descriptionParts.Add("");
        descriptionParts.Add("----");
        descriptionParts.Add($"*Migrated from Azure DevOps Work Item:* {workItem.Id}");
        descriptionParts.Add($"*Original Type:* {workItem.WorkItemType}");
        descriptionParts.Add($"*Original State:* {workItem.State}");
        
        if (!string.IsNullOrWhiteSpace(workItem.AssignedTo))
        {
            descriptionParts.Add($"*Originally Assigned To:* {workItem.AssignedTo}");
        }

        return string.Join("\n", descriptionParts);
    }
}
