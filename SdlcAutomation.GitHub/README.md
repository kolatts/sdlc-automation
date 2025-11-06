# SdlcAutomation.GitHub

GitHub REST API client library for creating and managing issues.

## Features

- Create GitHub issues with title, body, labels, and assignees
- Get issue details
- Personal Access Token (PAT) authentication
- Strongly-typed models with validation

## Authentication

Uses GitHub Personal Access Token (PAT) for authentication. The token should have `repo` scope to create issues.

## Usage

```csharp
using SdlcAutomation.GitHub;
using SdlcAutomation.GitHub.Models;

// Create client from environment variable
var client = GitHubApiClient.CreateFromEnvironment(); // Uses GITHUB_PAT env var

// Or create client directly with token
var client = new GitHubApiClient("your-github-token");

// Create an issue
var issue = await client.CreateIssueAsync(
    owner: "username",
    repository: "repo-name",
    title: "Issue title",
    body: "Issue description",
    labels: new List<string> { "bug", "enhancement" },
    assignees: new List<string> { "username" }
);

Console.WriteLine($"Created issue #{issue.Number}: {issue.HtmlUrl}");
```

## Models

- **Issue**: Represents a GitHub issue
- **Repository**: Represents repository owner and name
- **Label**: Represents a GitHub label
- **ErrorResponse**: Represents API error responses
