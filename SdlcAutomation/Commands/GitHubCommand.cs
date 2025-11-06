using System.CommandLine;
using SdlcAutomation.GitHub;
using SdlcAutomation.GitHub.Models;
using Spectre.Console;

namespace SdlcAutomation.Commands;

public class GitHubCommand : BaseCommand
{
    public GitHubCommand() : base("github", "GitHub integration commands")
    {
        var createCommand = new Command("create", "Create a GitHub issue");
        
        var ownerOption = new Option<string>(
            "--owner",
            "The GitHub repository owner (username or organization)") { IsRequired = true };
        
        var repoOption = new Option<string>(
            "--repo",
            "The GitHub repository name") { IsRequired = true };
        
        var titleOption = new Option<string>(
            "--title",
            "The issue title") { IsRequired = true };
        
        var bodyOption = new Option<string?>(
            "--body",
            "The issue body/description");
        
        var labelsOption = new Option<string?>(
            "--labels",
            "Comma-separated list of labels");
        
        var assigneesOption = new Option<string?>(
            "--assignees",
            "Comma-separated list of assignees");

        createCommand.AddOption(ownerOption);
        createCommand.AddOption(repoOption);
        createCommand.AddOption(titleOption);
        createCommand.AddOption(bodyOption);
        createCommand.AddOption(labelsOption);
        createCommand.AddOption(assigneesOption);

        createCommand.SetHandler(CreateIssue, ownerOption, repoOption, titleOption, bodyOption, labelsOption, assigneesOption);
        
        AddCommand(createCommand);
    }

    private async Task CreateIssue(string owner, string repo, string title, string? body, string? labels, string? assignees)
    {
        try
        {
            // Parse labels and assignees
            var labelList = string.IsNullOrWhiteSpace(labels) 
                ? null 
                : labels.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
            
            var assigneeList = string.IsNullOrWhiteSpace(assignees) 
                ? null 
                : assignees.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

            // Create issue model
            var issue = new Issue
            {
                Title = title,
                Body = body,
                Labels = labelList,
                Assignees = assigneeList
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

            WriteInfo($"Connecting to GitHub API...");
            
            using var client = GitHubApiClient.CreateFromEnvironment();
            
            WriteInfo($"Creating issue in {owner}/{repo}...");
            
            var createdIssue = await client.CreateIssueAsync(owner, repo, issue);

            WriteSuccess($"Created issue #{createdIssue.Number}: {title}");
            if (!string.IsNullOrEmpty(createdIssue.HtmlUrl))
            {
                WriteInfo($"URL: {createdIssue.HtmlUrl}");
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
