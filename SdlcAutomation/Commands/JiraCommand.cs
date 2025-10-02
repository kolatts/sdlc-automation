using System.CommandLine;
using SdlcAutomation.Clients.Jira;
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
    }

    private async Task CreateIssue(string project, string type, string summary, string? description)
    {
        try
        {
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
            
            var response = await client.CreateIssueAsync(
                projectKey: project,
                issueTypeName: type,
                summary: summary,
                description: description);

            WriteSuccess($"Created {type}: {response.Key}");
            if (!string.IsNullOrEmpty(response.Self))
            {
                WriteInfo($"URL: {response.Self}");
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
}
