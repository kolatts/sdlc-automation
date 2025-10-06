using System.CommandLine;
using SdlcAutomation.Jira;
using SdlcAutomation.Jira.Models;
using Spectre.Console;
using Io.Cucumber.Messages;
using Google.Protobuf;

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

        // Add import-cucumber command
        var importCucumberCommand = new Command("import-cucumber", "Import Cucumber messages file to JIRA X-ray");
        
        var fileOption = new Option<string>(
            "--file",
            "Path to the Cucumber messages file (NDJSON format)") { IsRequired = true };
        
        var workItemOption = new Option<string>(
            "--work-item",
            "JIRA work item number to link the test results to") { IsRequired = true };

        importCucumberCommand.AddOption(fileOption);
        importCucumberCommand.AddOption(workItemOption);

        importCucumberCommand.SetHandler(ImportCucumber, fileOption, workItemOption);
        
        AddCommand(importCucumberCommand);
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

    private async Task ImportCucumber(string file, string workItem)
    {
        try
        {
            // Check if file exists
            if (!File.Exists(file))
            {
                WriteError($"File not found: {file}");
                return;
            }

            WriteInfo($"Reading Cucumber messages from: {file}");

            var envelopes = new List<Envelope>();
            
            // Read NDJSON file (one JSON object per line)
            using (var reader = new StreamReader(file))
            {
                string? line;
                int lineNumber = 0;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lineNumber++;
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    try
                    {
                        var envelope = Envelope.Parser.ParseJson(line);
                        envelopes.Add(envelope);
                    }
                    catch (Exception ex)
                    {
                        WriteWarning($"Failed to parse line {lineNumber}: {ex.Message}");
                    }
                }
            }

            if (!envelopes.Any())
            {
                WriteError("No valid Cucumber messages found in file");
                return;
            }

            WriteSuccess($"Parsed {envelopes.Count} Cucumber messages");

            // Extract test results summary
            var testCases = envelopes.Where(e => e.TestCase != null).Select(e => e.TestCase).ToList();
            var testStepFinished = envelopes.Where(e => e.TestStepFinished != null).Select(e => e.TestStepFinished).ToList();
            
            WriteInfo($"Found {testCases.Count} test cases");
            WriteInfo($"Found {testStepFinished.Count} test step results");

            var baseUrl = Environment.GetEnvironmentVariable("JIRA_BASE_URL");
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                WriteError("JIRA_BASE_URL environment variable is not set.");
                WriteInfo("Set it to your JIRA instance URL (e.g., https://jira.example.com)");
                return;
            }

            WriteInfo($"Connecting to JIRA at: {baseUrl}");
            WriteInfo($"Importing test results to work item: {workItem}");

            // Note: This is a placeholder for actual X-ray API integration
            // X-ray has specific endpoints for importing test results
            // The actual implementation would require X-ray REST API client
            WriteWarning("X-ray API integration not yet implemented");
            WriteInfo("The command has successfully parsed the Cucumber messages file");
            WriteInfo($"To complete the import, you would need to:");
            WriteInfo($"  1. Use the X-ray REST API endpoint: POST /rest/raven/2.0/import/execution/cucumber");
            WriteInfo($"  2. Link the results to work item: {workItem}");
            WriteInfo($"  3. Send the parsed test results in X-ray's expected format");

            await Task.CompletedTask;
        }
        catch (FileNotFoundException ex)
        {
            WriteError($"File not found: {ex.Message}");
        }
        catch (Exception ex)
        {
            WriteError($"Unexpected error: {ex.Message}");
        }
    }
}
